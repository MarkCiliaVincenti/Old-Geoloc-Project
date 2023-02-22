using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.AzureServiceBusTransport;
using Microsoft.ServiceBus;
using SmartFleet.Core.Contracts.Commands;
using SmartFleet.Core.Protocols.NewBox;
using SmartFleet.Core.Protocols.Tk1003;

namespace GT02AServer
{
    class Program
    {
        private static IBusControl _bus;
        static void Main(string[] args)
        {
    //      
            TcpListener listener = new TcpListener(IPAddress.Any, 8080);
            listener.Start();
            //_bus= Bus.Factory.CreateUsingAzureServiceBus(sbc =>
            //{
            //    var serviceUri = ServiceBusEnvironment.CreateServiceUri("sb",
            //        ConfigurationManager.AppSettings["AzureSbNamespace"],
            //        ConfigurationManager.AppSettings["AzureSbPath"]);

            //    var host = sbc.Host(serviceUri,
            //        h =>
            //        {
            //            h.TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(
            //                ConfigurationManager.AppSettings["AzureSbKeyName"],
            //                ConfigurationManager.AppSettings["AzureSbSharedAccessKey"], TimeSpan.FromDays(1),
            //                TokenScope.Namespace);
            //        });
            //});
            //_bus.Start();

            while (true) // Add your exit flag here
            {
                var client = listener.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(ThreadProc, client);
            }

        }

        private static void ThreadProc(object state)
        {
            var client = ((TcpClient) state);
            byte[] buffer = new byte[client.ReceiveBufferSize];
            NetworkStream stream = ((TcpClient) state).GetStream();
            int bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);

            string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead).Replace("\r\n", "").Replace("\r", "").Replace("\n", ""); ;
            try
            {
                if (dataReceived.Contains(","))
                {
                    // il s'agit du format de nouveaux boitiers créent par khaled
                    NewBoxParser parser = new NewBoxParser();

                    var result = parser.Parse(dataReceived.Split('\r'));
                    foreach (var r in result)
                    {
                        //   Task.Run(async () => { await SendCommand(stream, r.Value, client); });
                        _bus.Publish<CreateNewBoxGps>(r);
                    }
                }
                else
                {
                    // boitier GT02A
                    Tk1003Parser parser = new Tk1003Parser();
                    var result = parser.Parse(dataReceived.Split('\r'));
                    foreach (var r in result)
                    {
                        Task.Run(async () => { await SendCommand(stream, r.Value, client); });
                        //foreach (var createTk103Gpse in r.Key)
                        //{
                        //    _bus.Publish<CreateTk103Gps>(createTk103Gpse);
                        //}
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
               // client.Close();
            }


            // Console.ReadLine();
        }




        private static async Task SendCommand(NetworkStream stream, string msg, TcpClient client)
        {
            await stream.WriteAsync(Encoding.ASCII.GetBytes(msg), 0, msg.Length);
            client.Close();
        }

       

        
    }
}
