using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using TeltonikaEmulator.Models;

namespace TeltonikaEmulator.TcpClient
{
    public delegate void UpdateLogDataGird(LogVm log);


    public class Client : IDisposable
    {
        private readonly int _port;
        private readonly string _server;
        private System.Net.Sockets.TcpClient _client;
        private NetworkStream _stream;
        private int _bufferSize = 4;
        public Client(String server, Int32 port)
        {
            _port = port;
            _server = server;

        }

        public event EventHandler OnDisconnected;

        public void Disconnect()
        {
            _client?.Close();
            IsConnected = false;
        }


        private async Task CloseAsync()
        {
            await Task.Yield();
            if (_client != null)
            {

                _client.Close();
                //_client.Dispose();
                _client = null;
            }
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }
        }
        private async Task CloseIfCanceledAsync(CancellationToken token, Action onClosed = null)
        {
            if (token.IsCancellationRequested)
            {
                await CloseAsync().ConfigureAwait(false);
                onClosed?.Invoke();
                token.ThrowIfCancellationRequested();
            }
        }
        public async Task SendAsync(byte[] data, CancellationToken token = default(CancellationToken))
        {
            try
            {
                await _stream.WriteAsync(data, 0, data.Length, token).ConfigureAwait(false);
                await _stream.FlushAsync(token).ConfigureAwait(false);

            }
            catch (IOException ex)
            {
                if (ex.InnerException != null && ex.InnerException is ObjectDisposedException) // for SSL streams
                {
                }
                else OnDisconnected?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                Thread.Sleep(500);
                Console.WriteLine(e.Message);
               // await SendAsync(data, token);
            }
        }



        public async Task<Byte[]> ReceiveAsync(CancellationToken token = default(CancellationToken))
        {
            byte[] buffer = new byte[_bufferSize];
            byte[] data = new byte[4];
            try
            {
                if (!IsConnected || IsRecieving)
                    throw new InvalidOperationException();
                IsRecieving = true;
                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, token).ConfigureAwait(false);

                data = new byte[bytesRead];
                Array.Copy(buffer, data, bytesRead);
                IsRecieving = false;
                // buffer = new byte[bufferSize];
            }
            catch (ObjectDisposedException)
            {
            }
            catch (IOException ex)
            {
                var evt = OnDisconnected;
                if (ex.InnerException != null && ex.InnerException is ObjectDisposedException) { } // for SSL streams

                if (evt != null)
                    evt(this, EventArgs.Empty);
            }
            finally
            {
                IsRecieving = false;
            }
            return data;

        }
        public bool IsRecieving { get; set; }

        public bool IsConnected { get; set; }

        public async Task ConnectAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                //Connect async method
                await CloseAsync().ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                _client = new System.Net.Sockets.TcpClient();
                cancellationToken.ThrowIfCancellationRequested();
                await _client.ConnectAsync(_server, _port).ConfigureAwait(false);
                await CloseIfCanceledAsync(cancellationToken).ConfigureAwait(false);
                // get stream and do SSL handshake if applicable

                _stream = _client.GetStream();
                await CloseIfCanceledAsync(cancellationToken).ConfigureAwait(false);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await CloseIfCanceledAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }
        }
        public void CloseStream()
        {
            _stream?.Close();
        }


        public void Dispose()
        {
            ((IDisposable)_client)?.Dispose();
            _stream?.Dispose();
        }
    }
}
