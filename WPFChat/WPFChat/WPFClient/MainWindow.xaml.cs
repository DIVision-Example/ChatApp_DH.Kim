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
using System.Net;
using System.Net.Sockets;

namespace WPFClient {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window {

        TcpClient client;
        public MainWindow() {
            InitializeComponent();
        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e) {
            client = new TcpClient();
            await client.ConnectAsync("192.168.1.202", 7000);
            _ = HandleClient(client);

        }

        private void btnSend_Click(object sender, RoutedEventArgs e) {
            if(txtChat.Text.Length > 0 ) {
                NetworkStream stream = client.GetStream();

                string text = $"{txtUserName.Text}: {txtChat.Text}";
                var messageBuffer = Encoding.Default.GetBytes(text);
                stream.Write(messageBuffer);

                txtChat.Text = "";
            }
        }

        private async Task HandleClient(TcpClient client) {
            NetworkStream stream = client.GetStream();

            byte[] szText = Encoding.Default.GetBytes($"User {txtUserName.Text} Connected.");
            stream.Write(szText, 0, szText.Length);
            byte[] buffer = new byte[1024];
            int read;

            while((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0) {
                string Message = Encoding.Default.GetString(buffer, 0, read);

                LogList.Items.Add(Message);
            }
        }




    }
}
