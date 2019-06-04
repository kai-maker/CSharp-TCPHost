using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPHostGUI
{
    public class TCPServer
    {
        private TcpListener _listener;
        private List<TcpClient> _clients = new List<TcpClient>();
        
        public event Action<string, TcpClient> OnReceiveEvent;
        public event Action<TcpClient> OnConnectEvent;

        public TCPServer(string localAddr, int localPort)
        {
            _listener = new TcpListener(IPAddress.Parse(localAddr), localPort);
            _listener.Start();
        }

        public void Accept()
        {
            _listener.AcceptTcpClientAsync().ContinueWith(task =>
            {
                var client = task.Result;
                _clients.Add(client);
                Console.WriteLine("Connected(Server)");
                Console.WriteLine($"Remote IP is : {client.Client.RemoteEndPoint}");
                OnConnectEvent(client);
                Accept();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void Receive()
        {
            if (_clients.Count == 0)
            {
                Task.Delay(1000).ContinueWith(t =>
                {
                    Receive();
                }, TaskScheduler.FromCurrentSynchronizationContext());
                return;
            }
            var bytes = new Byte[256];
            foreach (var client in _clients)
            {
                var stream = client.GetStream();
                stream.ReadAsync(bytes, 0, bytes.Length).ContinueWith(task =>
                {
                    string message = Encoding.UTF8.GetString(bytes, 0, task.Result);
                    Console.WriteLine($"Reveive : {message} (Server)");
                    OnReceiveEvent?.Invoke(message, client);
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        public void Send(string message)
        {
            foreach (var client in _clients)
            {
                var byteMessage = Encoding.UTF8.GetBytes(message);
                var stream = client.GetStream();
                stream.WriteAsync(byteMessage, 0, byteMessage.Length).ContinueWith(task =>
                {
                    Console.WriteLine("Sent(Server)");
                });
            }
        }
    }
}