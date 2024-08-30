using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WakeOnLanDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TestButton_OnClick(object sender, RoutedEventArgs e)
        {
            string ip = "192.168.2.9";
            string macAddress = "EC:D6:8A:A6:4B:7B";
            SendWakeOnLan(ip, macAddress);
        }

        private static void SendWakeOnLan(string ipAddress, string macAddress)
        {
            try
            {
                // 将MAC地址转换为字节数组
                byte[] macBytes = macAddress.Split(':').Select(x => Convert.ToByte(x, 16)).ToArray();
                // 创建魔术包
                byte[] magicPacket = new byte[6 + 16 * macBytes.Length];
                for (int i = 0; i < 6; i++)
                {
                    magicPacket[i] = 0xFF;
                }
                for (int i = 0; i < 16; i++)
                {
                    Array.Copy(macBytes, 0, magicPacket, 6 + i * macBytes.Length, macBytes.Length);
                }
                // 使用UDP广播发送魔术包
                UdpClient client = new UdpClient();
                client.Connect(ipAddress, 9); // 9是WoL的默认端口
                client.Send(magicPacket, magicPacket.Length);
                client.Close();

                Debug.WriteLine($@"唤醒包已发送至: {ipAddress},{macAddress}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("发送唤醒包时发生错误: " + ex.Message);
            }
        }
    }
}