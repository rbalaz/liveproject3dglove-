using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServerTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private SocketServer server;
        private string ip;
        public List<TextBox> leftHandFingers;
        public List<TextBox> rightHandFingers;

        public MainWindow()
        {
            InitializeComponent();

            ip = SocketServer.GetLocalIP();
            text_ip.Text = ip;

            leftHandFingers = new List<TextBox>();
            rightHandFingers = new List<TextBox> { tb_thumb, tb_index, tb_middle, tb_ring, tb_pinky };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (server != null && server.Running)
            {
                server.Stop();
            }

            
            server = new SocketServer(ip, 7999, this);
            server.Run();
        }
    }
}
