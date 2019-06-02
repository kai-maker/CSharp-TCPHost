using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPHostGUI
{
    public class TCPServer
    {
        private Int32 _localPort;
        private IPAddress _localAddr;
        private TcpListener _listener;
        private TcpClient _client;
        
        public event Action<string> OnReceiveEvent;

        public TCPServer(string localAddr, int localPort)
        {
            _localAddr = IPAddress.Parse(localAddr);
            _localPort = localPort;
            _listener = new TcpListener(_localAddr, _localPort);
            _listener.Start();
        }

        public void Accept()
        {
            _listener.AcceptTcpClientAsync().ContinueWith(task =>
            {
                var client = task.Result;
                _client = client;
                Console.WriteLine("Connected(Server)");
                Console.WriteLine($"Remote IP is : {_client.Client.RemoteEndPoint}");
                Accept();
            });
        }

        public void Receive()
        {
            if (_client == null)
            {
                Task.Delay(1000).ContinueWith(t =>
                {
                    Receive();
                }, TaskScheduler.FromCurrentSynchronizationContext());
                return;
            }
            var bytes = new Byte[256];
            var stream = _client.GetStream();
            stream.ReadAsync(bytes, 0, bytes.Length).ContinueWith(task =>
            {
                string message = Encoding.UTF8.GetString(bytes, 0, task.Result);
                Console.WriteLine($"Reveive : {message} (Server)");
                OnReceiveEvent?.Invoke(message);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void Send(string message)
        {
            var byteMessage = Encoding.UTF8.GetBytes(message);
            var stream = _client.GetStream();
            stream.WriteAsync(byteMessage, 0, byteMessage.Length).ContinueWith(task =>
            {
                Console.WriteLine("Sent(Server)");
            });
        }
    }
}