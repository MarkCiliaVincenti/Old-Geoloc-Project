using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using SmartFleet.Core;
using SmartFleet.Core.Contracts.Commands;
using SmartFleet.Core.Infrastructure.MassTransit;
using SmartFleet.Core.Protocols;
using SmartFleet.Core.ReverseGeoCoding;

namespace CommonServer
{
    public class Server :ICommonServer
    {
        private static IBusControl _bus;
        private static Semaphore _semaphore;
        private static Task<ISendEndpoint> _endpoint;
        private static ReverseGeoCodingService _reverseGeoCodingService;
        private static DevicesParser _devicesParser;

        public Server()
        {
            _reverseGeoCodingService = new ReverseGeoCodingService();
            _bus = MassTransitConfig.ConfigureSenderBus();
            var connextion = ConfigurationManager.AppSettings["RabbitMqConnectionString"];
            _endpoint = _bus.GetSendEndpoint(new Uri( connextion));
            _devicesParser = new DevicesParser();
    
        }
        public  void StartTeltonikaListner(IPAddress ipAddress, int port)
        {
            var listener = new TcpListener(ipAddress, port);
            listener.Start();
            Console.WriteLine($"Teltonika server starts at port N°:{port}...");
            while (true) // Add your exit flag here
            {
                var client = listener.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(new WaitCallback(HandelTeltonikaSockets), client);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        private  async void HandelTeltonikaSockets(object state)
        {
            string imei = string.Empty;
            using (var client = ((TcpClient)state))
            {
                using (NetworkStream stream = ((TcpClient)state).GetStream())
                {
                   
                    try
                    {
                        var gpsResult = new List<CreateTeltonikaGps>();
                        var buffer = new byte[client.ReceiveBufferSize];

                        while (true)
                        {
                            if (imei == string.Empty)
                            {
                                int bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize) - 2;
                                lock (stream)
                                {
                                    imei = _devicesParser.GetIMEI(buffer, bytesRead);
                                    Console.WriteLine("IMEI received : " + imei);
                                    Byte[] b = { 0x01 };
                                    stream.Write(b, 0, 1);
                                }
                              
                                await SendNewDeviceCommad(imei);
                            }
                            else if (imei!=string.Empty)
                            {
                                _semaphore = new Semaphore(1, 10, imei);
                                lock (stream)
                                {
                                   // _semaphore.WaitOne();
                                    gpsResult.AddRange(_devicesParser.Decode(new List<byte>(buffer), imei));
                                    var bytes = Convert.ToByte(gpsResult.Count);
                                    stream.Write(new byte[] { 0x00, 0x0, 0x0, bytes }, 0, 4);
                                    client.Close();
                                   // _semaphore.Release();
                                }
                                await SendDecodedData(gpsResult, imei);
                                break;
                            }
                            else break;

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        client.Close();
                        //throw;
                    }
                }

            }
        }

   
        private  async Task SendDecodedData(List<CreateTeltonikaGps> gpsResult, string imei)
        {
            if (gpsResult.Count > 0)
                foreach (var gpSdata in gpsResult.OrderBy(x => x.Timestamp).Distinct())
                {
                    var r = await _reverseGeoCodingService.ExecuteQuery(gpSdata.Lat, gpSdata.Long);
                    Thread.Sleep(1000);
                    if (r.display_name != null)
                        gpSdata.Address = r.display_name;
                    else
                    {
                        var ad = await _reverseGeoCodingService.ReverseGeoCoding(gpSdata.Lat, gpSdata.Long)
                            .ConfigureAwait(false);
                        if (ad != null)
                        {
                            //Console.WriteLine(ad);
                            gpSdata.Address = ad;
                        }

                        Thread.Sleep(1000);
                    }

                    await _bus.Publish(gpSdata);
                    var direction = SetDirection(gpSdata.Direction);
                    Console.WriteLine("IMEI: " + imei + " Date: " + gpSdata.Timestamp + " latitude : " + gpSdata.Lat +
                                      " Longitude:" + gpSdata.Long + " Speed: " + gpSdata.Speed + " Direction:" + direction +
                                      " address " + gpSdata.Address + " milage :" + gpSdata.Mileage);
                }
        }
        
        private async Task<string> SendNewDeviceCommad(string imei)
        {
           
            var command = new CreateBoxCommand();
            command.Imei = imei;
            await _endpoint.Result.Send(command);
            return imei;
        }

        private static string SetDirection(short gpSdataDirection)
        {
            if (gpSdataDirection < 4) return "East";
            else return "West";
        }
    }
}
