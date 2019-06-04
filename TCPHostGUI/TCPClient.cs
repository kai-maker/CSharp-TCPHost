using System;
using System.Net.Sockets;
using System.Text;

namespace TCPHostGUI
{
    public class TCPClient
    {
        private TcpClient _client;
        private Int32 _remotePort;
        private String _remoteIp;
        
        public event Action<string, TcpClient> OnReceiveEvent;
        public event Action<TcpClient> OnConnectEvent;

        public bool Connected => _client.Connected;

        public TCPClient()
        {
            _client = new TcpClient();
        }

        public void Connect(string remoteIp, int remotePort)
        {
            _remotePort = remotePort;
            _remoteIp = remoteIp;
            _client.ConnectAsync(_remoteIp, _remotePort);
            OnConnectEvent?.Invoke(_client);
        }

        public void Send(string message)
        {
            var stream = _client.GetStream();
            var byteMessage = Encoding.UTF8.GetBytes(message);
            stream.WriteAsync(byteMessage, 0, byteMessage.Length).ContinueWith(task =>
            {
                Console.WriteLine("Sent(Client)");
            });
        }

        public void Receive()
        {
            var stream = _client.GetStream();
            var byteMessage = new Byte[256];
            stream.ReadAsync(byteMessage, 0, byteMessage.Length).ContinueWith(task =>
            {
                var message = Encoding.UTF8.GetString(byteMessage, 0, task.Result);
                Console.Write($"Receive : {message} (Server)");
                OnReceiveEvent?.Invoke(message, _client);
            });
        }
    }
}