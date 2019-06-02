using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TCPHostGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MulticastHost _multicastHost;
        private TCPServer _tcpServer;
        private int dummyClientPort = 37000-1;

        public MainWindow()
        {
            InitializeComponent();
            _multicastHost = new MulticastHost(34000, "239.0.1.111", 37000);
            _multicastHost.OnReceiveEvent += result =>
            {
                var message = Encoding.UTF8.GetString(result.Buffer);
                Console.WriteLine($@"{result.Buffer.Length} バイト受信しました : {message}");
                tb.Text += "receive : " + message + "\n";
                Console.WriteLine($@"{result.RemoteEndPoint.Address} : {result.RemoteEndPoint.Port}");
            };
            _tcpServer = new TCPServer(GetLocalIp.Get(), 40000);
            _tcpServer.Accept();
            _tcpServer.OnReceiveEvent += message =>
            {
                tb.Text += "receive : " + message + "\n";
                _tcpServer.Receive();
            };
        }

        void OnScanButton(object sender, RoutedEventArgs e)
        {
            _multicastHost.Send("Hello!! From Server");
        }

        void OnTcpListenButton(object sender, RoutedEventArgs e)
        {
            tb.Text += "Start Listen...\n";
            _tcpServer.Receive();
        }

        void OnClickDummyClientButton(object sender, RoutedEventArgs e)
        {
            var dummyClientWindow = new DummyClientWindow(++dummyClientPort);
            dummyClientWindow.Show();
        }
        public void OnClosingWindow(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void OnIpAddrButton(object sender, RoutedEventArgs e)
        {
            string addr_ip;

            try
            {
                //ホスト名を取得
                string hostname = System.Net.Dns.GetHostName();

                //ホスト名からIPアドレスを取得
                System.Net.IPAddress[] addr_arr = System.Net.Dns.GetHostAddresses(hostname);

                //探す
                addr_ip = "";
                foreach ( System.Net.IPAddress addr in addr_arr )
                {
                    string addr_str = addr.ToString();

                    //IPv4 && localhostでない
                    if ( addr_str.IndexOf( "." ) > 0 && !addr_str.StartsWith( "127." ) )
                    {
                        addr_ip = addr_str;
                        break;
                    }
                }
                Console.WriteLine(addr_ip);
            }
            catch (Exception)
            {
                addr_ip = "";
            }
        }
    }
}
