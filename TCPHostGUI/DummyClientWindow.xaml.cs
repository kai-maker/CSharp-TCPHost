using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace TCPHostGUI
{
    public partial class DummyClientWindow : Window
    {
        private MulticastHost _multicastHost;
        private TCPClient _tcpClient;
        public DummyClientWindow(int localPort)
        {
            InitializeComponent();
            _multicastHost = new MulticastHost(localPort, "239.0.1.111", 34000);
            _multicastHost.OnReceiveEvent += result =>
            {
                var message = Encoding.UTF8.GetString(result.Buffer);
                //Console.WriteLine($@"{result.Buffer.Length} バイト受信しました : {message}");
                tb.Text += $"from [{result.RemoteEndPoint}] (UDP)\n\t=> {message}\n";
                Console.WriteLine($@"{result.RemoteEndPoint.Address} : {result.RemoteEndPoint.Port}");
                _multicastHost.Receive();
                if (!_tcpClient.Connected)
                {
                    _tcpClient.Connect(result.RemoteEndPoint.Address.ToString(), 40000);
                    _tcpClient.Receive();
                }

                _tcpClient.Send("Echo from client");
            };
            _tcpClient = new TCPClient();
            _tcpClient.OnConnectEvent += client => { tb.Text += $"Connected to [{client.Client.RemoteEndPoint}] (TCP)\n"; };
            _tcpClient.OnReceiveEvent += (message, client) =>
            {
                tb.Text += $"From [{client.Client.RemoteEndPoint}]\n\t=> : {message} (TCP)\n";
                _tcpClient.Receive();
            };
            OnMulticastStartButton(null, null);
        }

        public void OnMulticastStartButton(object sender, RoutedEventArgs e)
        {
            tb.Text += "Start Waiting...\n";
            _multicastHost.Receive();
        }

        public void OnIpAddrButton(object sender, RoutedEventArgs e)
        {
        }

        public void OnClosingWindow(object sender, CancelEventArgs e)
        {
            _multicastHost.Close();
        }
        
        void OnSendButton(object sender, RoutedEventArgs e)
        {
            _tcpClient.Send(SendTb.Text);
            SendTb.Text = "";
        }
    }
}