using System;
using System.Net.Sockets;
using System.Threading;

namespace TeltonikaClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            var message = "865067128865064,20181117122340.000,36.276863,1.676843,183.800,0.63,309.1";
            Int32 port = 3600;
            TcpClient client = new TcpClient("127.0.0.1", port);

            // Translate the passed message into ASCII and store it as a Byte array.
            for (int i = 0; i < 10; i++)
            {
               
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);
                stream.Flush();
              //  client.Close();
                Thread.Sleep(500);
                Console.WriteLine("Sent: {0}", message);
            }
            Console.ReadKey();
        }
    }
}