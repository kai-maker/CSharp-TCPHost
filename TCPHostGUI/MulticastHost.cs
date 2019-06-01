using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPHostGUI
{
    public class MulticastHost
    {
        AddressFamily addressFamily = AddressFamily.InterNetwork;
        private int localPort;
        private string endPointIp;
        private int remotePort;
        private IPEndPoint multicastGroupEndPoint;
        private UdpClient multicastClient;
        public event Action<UdpReceiveResult> OnReceiveEvent;
        

        public MulticastHost(int localPort = 35000, string endPointIp = "239.0.0.0", int remotePort = 35000)
        {
            this.localPort = localPort;
            this.endPointIp = endPointIp;
            this.remotePort = remotePort;
            multicastGroupEndPoint = new IPEndPoint(IPAddress.Parse(this.endPointIp), this.remotePort);
            multicastClient = new UdpClient(this.localPort, addressFamily);
            multicastClient.JoinMulticastGroup(multicastGroupEndPoint.Address, 50);
        }

        public void Send(string message)
        {
            //Console.WriteLine("Local end point: {0}", multicastClient.Client.LocalEndPoint);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            try
            {
                multicastClient.SendAsync(messageBytes, message.Length, multicastGroupEndPoint)
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
                multicastClient.ReceiveAsync()
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