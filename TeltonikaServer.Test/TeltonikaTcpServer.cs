using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using SmartFleet.Core.Contracts.Commands;
using SmartFleet.Core.Helpers;
using SmartFleet.Core.Protocols;
using SmartFleet.Core.ReverseGeoCoding;

namespace TeltonikaServer.Test
{
    public class TeltonikaTcpServer
    {
        private readonly IBusControl _bus;
       
        private TcpListener _listener;

        public TeltonikaTcpServer(IBusControl bus, ReverseGeoCodingService reverseGeoCodingService)
        {
            _bus = bus;
         
        }

        public void Start()
        {
            _listener = new TcpListener(IPAddress.Any, 34400);
            _listener.Start();
            while (true) // Add your exit flag here
            {
                var client = _listener.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(ThreadProc, client);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        // ReSharper disable once TooManyDeclarations
        private async void  ThreadProc(object state)
        {
            try
            {
                var client = (TcpClient)state;
                byte[] buffer = new byte[client.ReceiveBufferSize];
                NetworkStream stream = ((TcpClient)state).GetStream();
                int bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize) - 2;
                string imei = Encoding.ASCII.GetString(buffer, 2, bytesRead);
                if (Commonhelper.IsValidImei(imei))
                    await ParseAvlDataAsync(client, stream, imei).ConfigureAwait(false);
            }
            catch (InvalidCastException e)
            {
                Trace.TraceWarning(e.Message);
            }
            catch (Exception e)
            {
                Trace.TraceWarning(e.Message);
                //throw;
            }
        }

        // ReSharper disable once MethodTooLong
        private async Task ParseAvlDataAsync(TcpClient client, NetworkStream stream, string imei)
        {
            Console.WriteLine("IMEI received : " + imei);
            Console.WriteLine("--------------------------------------------");
            Byte[] b = { 0x01 };
            await stream.WriteAsync(b, 0, 1).ConfigureAwait(false);
            var command = new CreateBoxCommand();
            command.Imei = imei;
            try
            {
                await _bus.Publish(command).ConfigureAwait(false);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            while (true)
            {
                stream = client.GetStream();
                byte[] buffer = new byte[client.ReceiveBufferSize];
                var readAsync = await stream.ReadAsync(buffer, 0, client.ReceiveBufferSize).ConfigureAwait(false);
                List<byte> list = new List<byte>();
                foreach (var b1 in buffer.Skip(9).Take(1)) list.Add(b1);
                int dataCount = Convert.ToInt32(list[0]);
                var bytes = Convert.ToByte(dataCount);
                if (client.Connected)
                    await stream.WriteAsync(new byte[] { 0x00, 0x00, 0x00, bytes }, 0, 4).ConfigureAwait(false);
                var gpsResult = ParseAvlData(imei, buffer);
                if (!gpsResult.Any() && imei.Any()) continue;
                var events = new TLGpsDataEvents
                {
                    Id = Guid.NewGuid(),
                    Events = gpsResult
                };
                await _bus.Publish(events).ConfigureAwait(false);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private List<CreateTeltonikaGps> ParseAvlData(string imei, byte[] buffer)
        {
            List<CreateTeltonikaGps> gpsResult = new List<CreateTeltonikaGps>();
            var parser = new DevicesParser();
            gpsResult.AddRange(parser.Decode(new List<byte>(buffer), imei));
           // await GeoReverseCodeGpsData(gpsResult);
            LogAvlData(gpsResult);
            return gpsResult;
        }

       
        private static void LogAvlData(List<CreateTeltonikaGps> gpsResult)
        {
            foreach (var gpsData in gpsResult.OrderBy(x => x.DateTimeUtc))
            {
                Console.WriteLine("Date:" + gpsData.DateTimeUtc + " Latitude: " + gpsData.Lat + " Longitude" +
                                       gpsData.Long + " Speed :" + gpsData.Speed + "Direction: " + gpsData.Direction);
                Console.WriteLine("--------------------------------------------");
                foreach (var io in gpsData.IoElements_1B)
                    Console.WriteLine("Propriété IO (1 byte) : " + (TNIoProperty) io.Key + " Valeur:" + io.Value);
                foreach (var io in gpsData.IoElements_2B)
                    Console.WriteLine("Propriété IO (2 byte) : " + (TNIoProperty) io.Key + " Valeur:" + io.Value);
                foreach (var io in gpsData.IoElements_4B)
                    Console.WriteLine("Propriété IO (4 byte) : " + (TNIoProperty) io.Key + " Valeur:" + io.Value);
                foreach (var io in gpsData.IoElements_8B)
                    Console.WriteLine("Propriété IO (8 byte) : " + (TNIoProperty) io.Key + " Valeur:" + io.Value);
                Console.WriteLine("--------------------------------------------");
            }
        }
    }
}
