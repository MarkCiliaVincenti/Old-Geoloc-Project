using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Serilog;
using SmartFleet.Core.Contracts.Commands;
using SmartFleet.Core.Helpers;
using SmartFleet.Core.Protocols;
using SmartFleet.Data;

namespace TeltonikaListner
{
    public class TeltonikaTcpServer
    {
        private readonly IBusControl _bus;
        private readonly IRedisCache _redisCache;
        private readonly ILogger _log;

        private TcpListener _listener;

        public TeltonikaTcpServer(
            IBusControl bus,
            IRedisCache redisCache, ILogger log)
        {
            _bus = bus;
            _redisCache = redisCache;
            _log = log;
        }

        public void Start()
        {
            try
            {
                _listener = new TcpListener(IPAddress.Any, 34400);
                _log.Information($"{DateTime.Now} -Server started");

                _listener.Start();
               
                while (true) // Add your exit flag here
                {
                    var client = _listener.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(ThreadProc, client);
                }
            }
            catch (Exception e)
            {
               _log.Error($"{DateTime.Now} -Error message : {e.Message}  details : {e.InnerException?.Message}");

                //throw;
            }
            // ReSharper disable once FunctionNeverReturns
        }

        // ReSharper disable once TooManyDeclarations
        private async void  ThreadProc(object state)
        {
            try
            {
                string imei;
                var client = (TcpClient) state;
                byte[] buffer = new byte[client.ReceiveBufferSize];
                NetworkStream stream = ((TcpClient) state).GetStream();
                stream.Read(buffer, 0, client.ReceiveBufferSize); 
                imei =CleanInput (Encoding.ASCII.GetString(buffer.ToArray()).Trim()); 
                if (Commonhelper.IsValidImei(imei))
                    await ParseAvlDataAsync(client, stream, imei).ConfigureAwait(false);
            }
            catch (InvalidCastException e)
            {
                Trace.TraceWarning(e.Message);
                _log.Error($"{DateTime.Now} -Error message : {e.Message}  details : {e.InnerException?.Message}");
            }
            catch (Exception e)
            {
                Trace.TraceWarning(e.Message);
                _log.Error($"{DateTime.Now} -Error message : {e.Message}  details : {e.InnerException?.Message}");

                //throw;
            }
        }
        static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(strIn, @"[^\w\.@-]", "",
                    RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters,
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }
        // ReSharper disable once MethodTooLong
        private async Task ParseAvlDataAsync(TcpClient client, NetworkStream stream, string imei)
        {
            try
            {
                Trace.TraceInformation("IMEI received : " + imei);

                _log.Information($"{DateTime.Now} -IMEI received: {imei}");

                Trace.TraceInformation("--------------------------------------------");
                Byte[] b = { 0x01 };
                await stream.WriteAsync(b, 0, 1).ConfigureAwait(false);
                var modem = _redisCache.Get<CreateBoxCommand>(imei);

                while (true)
                {
                    stream = client.GetStream();
                    byte[] buffer = new byte[client.ReceiveBufferSize];
                    await stream.ReadAsync(buffer, 0, client.ReceiveBufferSize).ConfigureAwait(false);
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
                    var lastGpsData = gpsResult.Last();
                    if (modem == null)
                    {
                        modem = new CreateBoxCommand();
                        modem.Imei = imei;
                    }
                    modem.Longitude = lastGpsData.Long;
                    modem.Latitude = lastGpsData.Lat;
                    modem.LastValidGpsDataUtc = lastGpsData.DateTimeUtc;
                    modem.Speed = lastGpsData.Speed;
                    await _bus.Publish(modem).ConfigureAwait(false);
                    // break;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                client.Close();
                throw;
            }
        }

        private List<CreateTeltonikaGps> ParseAvlData(string imei, byte[] buffer)
        {
            List<CreateTeltonikaGps> gpsResult = new List<CreateTeltonikaGps>();
            var parser = new DevicesParser();
            gpsResult.AddRange(parser.Decode(new List<byte>(buffer), imei));
            LogAvlData(gpsResult);
            return gpsResult;
        }

        
        private  void LogAvlData(List<CreateTeltonikaGps> gpsResult)
        {
            foreach (var gpsData in gpsResult.OrderBy(x => x.DateTimeUtc))
            {
                Trace.TraceInformation(
                    $"Date:{gpsData.DateTimeUtc} Latitude: {gpsData.Lat} Longitude{gpsData.Long} Speed :{gpsData.Speed} Direction: {gpsData.Direction}");
                _log.Information($"Date time:{gpsData.DateTimeUtc} Latitude: {gpsData.Lat} Longitude{gpsData.Long} Speed :{gpsData.Speed} Direction: {gpsData.Direction}");

                Trace.TraceInformation("--------------------------------------------");
                foreach (var io in gpsData.IoElements_1B)
                    Trace.TraceInformation("Propriété IO (1 byte) : " + (TNIoProperty) io.Key + " Valeur:" + io.Value);
                foreach (var io in gpsData.IoElements_2B)
                    Trace.TraceInformation("Propriété IO (2 byte) : " + (TNIoProperty) io.Key + " Valeur:" + io.Value);
                foreach (var io in gpsData.IoElements_4B)
                    Trace.TraceInformation("Propriété IO (4 byte) : " + (TNIoProperty) io.Key + " Valeur:" + io.Value);
                foreach (var io in gpsData.IoElements_8B)
                    Trace.TraceInformation("Propriété IO (8 byte) : " + (TNIoProperty) io.Key + " Valeur:" + io.Value);
                Trace.TraceInformation("--------------------------------------------");
            }
        }
    }
}
