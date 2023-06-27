using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WPFClient {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class UiClient : Window {
        public static string ip = "192.168.2.80";
        public static int port = 7000;
        ChatClient client = new ChatClient();

        public UiClient() {
            InitializeComponent();
            client.ChattingList = ChattingList;
        }

        private void UiClient_UnLoaded(object sender, RoutedEventArgs e) {
            client.Stop();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrEmpty(txtChat.Text)) return;
            client.SendMessage(txtChat.Text);
            txtChat.Text = "";
            txtChat.Focus();
        }

        private void clientWindow_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e) {
            if(e.Key == System.Windows.Input.Key.Enter) {
                if (string.IsNullOrEmpty(txtChat.Text)) return;
                client.SendMessage(txtChat.Text);
                txtChat.Text = "";
                txtChat.Focus();
            }
        }
    }

    class ChatClient {
        TcpClient tcpClient;
        NetworkStream stream;
        bool stopFlag = false;

        public ListView ChattingList { get; internal set; }

        public ChatClient() {
            tcpClient = new TcpClient(UiClient.ip, UiClient.port);
            stream = tcpClient.GetStream();

            Thread thread = new Thread(Run);
            // Thread thread = new Thread(Run);
            thread.Start();
        }

        private void Run() {     // Thread 코드      Run() -> ReceiveMessage()
            ReceiveMessage();
        }

        public void Stop() {
            stopFlag = true;
        }

        public void SendMessage(string message) {
            byte[] Sendbuffer = Encoding.Default.GetBytes(message);
            stream.Write(Sendbuffer, 0, Sendbuffer.Length);
        }

        public void ReceiveMessage() {
            
            while (true) {
                if (stopFlag) {
                    Debug.WriteLine("Thread 종료");
                    tcpClient.Close();
                    stream.Close();
                    return;
                }

                byte[] ReceiveBuffer = new byte[1024];
                ReceiveBuffer = Encoding.Default.GetBytes("한글테스트");

                //stream.Read(ReceiveBuffer, 0, ReceiveBuffer.Length);
                string message = Encoding.Default.GetString(ReceiveBuffer);

                ChattingList.Dispatcher.Invoke(() => { 
                    ChattingList.Items.Add("서버로부터 되돌아옴 : " + message);
                }, DispatcherPriority.ApplicationIdle);

                Debug.WriteLine(message);
            }
        }
    }
}
