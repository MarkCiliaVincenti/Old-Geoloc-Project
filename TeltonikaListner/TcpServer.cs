using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MassTransit;
using Serilog;
using SmartFleet.Core.Contracts.Commands;
using SmartFleet.Core.Protocols;
using SmartFleet.Data;

namespace TeltonikaListner
{
    public class TcpServer
    {
        private readonly IBusControl _bus;
        private readonly IRedisCache _redisCache;
        private readonly ILogger _log;
        private Socket _serverSocket;
        private readonly ObservableCollection<ConnectionInfo> _connections ;

        public TcpServer()
        {
            
        }

        void _connections_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Console.Title = _connections.Count + " devices connected";
        }

        private void SetupServerSocket()
        {
            string ip = IPAddress.Any.ToString();
            IPEndPoint myEndpoint = new IPEndPoint(IPAddress.Parse(ip), 34400);

            _serverSocket = new Socket(myEndpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
           
           _serverSocket.Bind(myEndpoint);
            Console.WriteLine($"starts listening : 0.0.0.0:{34400}");
            _serverSocket.Listen((int)SocketOptionName.MaxConnections);
        }
        private class ConnectionInfo
        {
            public Socket Socket;
            public byte[] Buffer;
            public bool IsPartialLoaded;
            public List<byte> TotalBuffer;

            public string Imei;
        }

        public  void Start()
        {
            try
            {
                SetupServerSocket();
                // number of simultaneous connections can be accepted
                for (int i = 0; i < 2000; i++)
                    _serverSocket.BeginAccept(AcceptCallback, _serverSocket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _serverSocket.Close();
                //throw;
            }
        }

        private void AcceptCallback(IAsyncResult result)
        {
            ConnectionInfo connection = new ConnectionInfo();
            try
            {
                // Finish Accept operation
                Socket s = (Socket)result.AsyncState;
                connection.Socket = s.EndAccept(result);
                connection.Buffer = new byte[1024];
                // add connection to connection list
                lock (_connections)
                {
                    _connections.Add(connection);
                }

                // Start BeginReceive operation on connected device and make new BeginAccept operation on socket
                // for accept new connectionrequests.
                connection.Socket.BeginReceive(connection.Buffer,
                    0, connection.Buffer.Length, SocketFlags.None,
                    ReceiveCallback, connection);
                _serverSocket.BeginAccept(AcceptCallback, result.AsyncState);
            }
            catch (SocketException exc)
            {
                CloseConnection(connection);
                Console.WriteLine("Socket exception: " +
                    exc.SocketErrorCode);
            }
            catch (Exception exc)
            {
                CloseConnection(connection);
                Console.WriteLine("Exception: " + exc);
            }
        }

        // ReSharper disable once ExcessiveIndentation
        private void ReceiveCallback(IAsyncResult result)
        {
            ConnectionInfo connection = (ConnectionInfo)result.AsyncState;
            try
            {
                //get a number of received bytes
                int bytesRead = connection.Socket.EndReceive(result);
                if (bytesRead > 0)
                {
                    //because device sends data with portions we need summary all portions to total buffer
                    if (connection.IsPartialLoaded)
                    {
                        connection.TotalBuffer.AddRange(connection.Buffer.Take(bytesRead).ToList());
                    }
                    else
                    {
                        if (connection.TotalBuffer != null)
                            connection.TotalBuffer.Clear();
                        connection.TotalBuffer = connection.Buffer.Take(bytesRead).ToList();
                    }
                    //-------- Get Length of current received data ----------
                    string hexDataLength = string.Empty;

                    //Skip four zero bytes an take next four bytes with value of AVL data array length
                    connection.TotalBuffer.Skip(4).Take(4).ToList().ForEach(delegate (byte b) { hexDataLength += String.Format("{0:X2}", b); });

                    int dataLength = Convert.ToInt32(hexDataLength, 16);
                    //
                    //bytesRead = 17 when parser receive IMEI  from device
                    //if datalength encoded in data > then total buffer then is a partial data a device will send next part
                    //we send confirmation and wait next portion of data
                    // ReSharper disable once ComplexConditionExpression
                    if (dataLength + 12 > connection.TotalBuffer.Count && bytesRead != 17)
                    {
                        connection.IsPartialLoaded = true;
                        connection.Socket.Send(new byte[] { 0x01 });
                        connection.Socket.BeginReceive(connection.Buffer, 0, connection.Buffer.Length, SocketFlags.None,
                        ReceiveCallback, connection);
                        return;
                    }

                    bool isDataPacket = true;

                    //when device send AVL data first 4 bytes is 0
                    string firstRourBytes = string.Empty;
                    connection.TotalBuffer.Take(4).ToList().ForEach(delegate (byte b) { firstRourBytes += String.Format("{0:X2}", b); });
                    if (Convert.ToInt32(firstRourBytes, 16) > 0)
                        isDataPacket = false;

                    // if is true then is AVL data packet
                    // else that a IMEI sended
                    if (isDataPacket)
                    {
                        if (true)
                        {
                            //all data we convert this to string in hex format only for diagnostic
                            StringBuilder data = new StringBuilder();
                            connection.TotalBuffer.ForEach(delegate (byte b) { data.AppendFormat("{0:X2}", b); });
                            Console.WriteLine("<" + data);
                        }
                        

                        var decAvl = new DevicesParser();
                        decAvl.OnDataReceive += decAVL_OnDataReceive;
                        //if CRC not correct number of data returned by AVL parser = 0;
                        var avlData = decAvl.Decode(connection.TotalBuffer, connection.Imei);
                        if (!connection.IsPartialLoaded)
                        {
                            // send to device number of received data for confirmation.
                            if (avlData.Count > 0)
                            {
                                connection.Socket.Send(new byte[] {0x00, 0x00, 0x00, Convert.ToByte(avlData.Count)});
                                LogAvlData(avlData);
                                var events = new TLGpsDataEvents
                                {
                                    Id = Guid.NewGuid(),
                                    Events = avlData
                                };
                                var lastGpsData = events.Events.Last();

                                var command = new CreateBoxCommand();
                                command.Imei = connection.Imei;
                                command.Longitude = lastGpsData.Long;
                                command.Latitude = lastGpsData.Lat;
                                command.LastValidGpsDataUtc = lastGpsData.DateTimeUtc;
                                command.Speed = lastGpsData.Speed;
                                _bus.Publish(command).ConfigureAwait(false);
                                Thread.Sleep(1000);
                                _bus.Publish(events).ConfigureAwait(false);
                            }
                            else
                                //send 0 number of data if CRC not correct for resend data from device
                                connection.Socket.Send(new byte[] { 0x00, 0x00, 0x00, 0x00 });

                        }

                        decAvl.OnDataReceive -= decAVL_OnDataReceive;
                        Console.WriteLine("Modem ID: " + connection.Imei + " send data");
                    }
                    else
                    {
                        //if is not data packet then is it IMEI info send from device
                        connection.Imei = Encoding.ASCII.GetString(connection.TotalBuffer.Skip(2).ToArray());
                        connection.Socket.Send(new byte[] { 0x01 });
                        Console.WriteLine("Modem ID: " + connection.Imei + " connected");
                    }
                    // Get next data portion from device
                    connection.Socket.BeginReceive(connection.Buffer, 0, connection.Buffer.Length, SocketFlags.None,
                        ReceiveCallback, connection);
                }//if all data received then close connection                    
                else CloseConnection(connection);
            }
            catch (SocketException exc)
            {
                CloseConnection(connection);
                Console.WriteLine("Socket exception: " + exc.SocketErrorCode);
            }
            catch (Exception exc)
            {
                CloseConnection(connection);
                Console.WriteLine("Exception: " + exc);
            }
        }

        void decAVL_OnDataReceive(string obj)
        {
            Console.WriteLine(obj);
        }

        private void CloseConnection(ConnectionInfo ci)
        {
            ci.Socket.Close();
            lock (_connections)
            {
                _connections.Remove(ci);
                Console.WriteLine("Modem ID: " + ci.Imei + " disconnected");
            }
        }
        private static void LogAvlData(List<CreateTeltonikaGps> gpsResult)
        {
            foreach (var gpsData in gpsResult.OrderBy(x => x.DateTimeUtc))
            {
                Console.WriteLine("Date:" + gpsData.DateTimeUtc + " Latitude: " + gpsData.Lat + " Longitude" +
                                       gpsData.Long + " Speed :" + gpsData.Speed + "Direction: " + gpsData.Direction);
                Console.WriteLine("--------------------------------------------");
                foreach (var io in gpsData.IoElements_1B)
                    Console.WriteLine("Propriété IO (1 byte) : " + (TNIoProperty)io.Key + " Valeur:" + io.Value);
                foreach (var io in gpsData.IoElements_2B)
                    Console.WriteLine("Propriété IO (2 byte) : " + (TNIoProperty)io.Key + " Valeur:" + io.Value);
                foreach (var io in gpsData.IoElements_4B)
                    Console.WriteLine("Propriété IO (4 byte) : " + (TNIoProperty)io.Key + " Valeur:" + io.Value);
                foreach (var io in gpsData.IoElements_8B)
                    Console.WriteLine("Propriété IO (8 byte) : " + (TNIoProperty)io.Key + " Valeur:" + io.Value);
                Console.WriteLine("--------------------------------------------");
            }
        }
    }
}
