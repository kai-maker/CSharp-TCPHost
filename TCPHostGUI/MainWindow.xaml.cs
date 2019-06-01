using System;
using System.Text;
using System.Windows;

namespace TCPHostGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MulticastHost _multicastHost;
        public MainWindow()
        {
            InitializeComponent();
            _multicastHost = new MulticastHost();
            _multicastHost.OnReceiveEvent += result =>
            {
                var message = Encoding.UTF8.GetString(result.Buffer);
                Console.WriteLine($@"{result.Buffer.Length} バイト受信しました : {message}");
                tb.Text += "receive : " + message + "\n";
                Console.WriteLine($@"{result.RemoteEndPoint.Address} : {result.RemoteEndPoint.Port}");
            };
        }
        void OnClickButton1(object sender, RoutedEventArgs e)
        {
            _multicastHost.LocalPort = 10000;
            _multicastHost.RemotePort = 10000;
            _multicastHost.Receive();
            _multicastHost.Send("aaaa");
        }
    }
}
