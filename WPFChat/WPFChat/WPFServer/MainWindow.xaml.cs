using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFServer {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private TcpListener server;
        public MainWindow() {
            InitializeComponent();

            
        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e) {
            server = new TcpListener(IPAddress.Parse("192.168.1.202"), 7000);
            server.Start();

            while(true) {
                TcpClient client = await server.AcceptTcpClientAsync();

                _ = HandleClient(client);
            }
        }

        private async Task HandleClient(TcpClient client) {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            int read;

            while((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0) { 
                string Message = Encoding.Default.GetString(buffer, 0, read);

                LogList.Items.Add(Message);

                var messageBuffer = Encoding.Default.GetBytes($"{Message}");
                stream.Write(messageBuffer);
            }
        }




    }
}
