using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPHostGUI
{
    public class MulticastHost
    {
        AddressFamily addressFamily = AddressFamily.InterNetwork;

        public int LocalPort
        {
            set { _localPort = value; Configure(); }
            get => _localPort;
        }
        public string EndPointIp
        {
            set { _endPointIp = value; Configure(); }
            get => _endPointIp;
        }

        public int RemotePort
        {
            set { _remotePort = value; Configure(); }
            get => _remotePort;
        }

        private int _localPort;
        private string _endPointIp;
        private int _remotePort;
        private IPEndPoint _multicastGroupEndPoint;
        private UdpClient _multicastClient;
        public event Action<UdpReceiveResult> OnReceiveEvent;
        

        public MulticastHost(int localPort = 35000, string endPointIp = "239.0.0.0", int remotePort = 35000)
        {
            _localPort = localPort;
            _endPointIp = endPointIp;
            _remotePort = remotePort;
            _multicastGroupEndPoint = new IPEndPoint(IPAddress.Parse(_endPointIp), _remotePort);
            _multicastClient = new UdpClient(_localPort, addressFamily);
            _multicastClient.JoinMulticastGroup(_multicastGroupEndPoint.Address, 50);
        }

        void Configure()
        {
            _multicastClient.Close();
            _multicastClient = new UdpClient(_localPort, addressFamily);
            _multicastGroupEndPoint = new IPEndPoint(IPAddress.Parse(_endPointIp), _remotePort);
            _multicastClient.JoinMulticastGroup(_multicastGroupEndPoint.Address, 50);
        }

        public void Send(string message)
        {
            //Console.WriteLine("Local end point: {0}", multicastClient.Client.LocalEndPoint);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            try
            {
                _multicastClient.SendAsync(messageBytes, message.Length, _multicastGroupEndPoint)
                    .ContinueWith(task =>
                    {
                        Console.WriteLine($@"{task.Result} バイト送信しました");
                    }, TaskScheduler.FromCurrentSynchronizationContext());

            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
        }

        public void Receive()
        {
            try
            {
                _multicastClient.ReceiveAsync()
                    .ContinueWith(task =>
                    {
                        OnReceiveEvent?.Invoke(task.Result);
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
        }
    }
}