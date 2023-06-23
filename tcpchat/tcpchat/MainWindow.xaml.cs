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
using System.Threading;

namespace tcpchat {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary> http://192.168.2.80:7000/
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();

            // (1) IP 주소와 포트를 지정하고 TCP 연결 
            TcpClient tc = new TcpClient("192.168.2.80", 7000);
            //TcpClient tc = new TcpClient("localhost", 7000);
            string msg = "GO HOME!";
            byte[] buff = Encoding.ASCII.GetBytes(msg);
            // (2) NetworkStream을 얻어옴 
            NetworkStream stream = tc.GetStream();
            // (3) 스트림에 바이트 데이타 전송
            stream.Write(buff, 0, buff.Length);
            // (4) 스트림으로부터 바이트 데이타 읽기
            byte[] outbuf = new byte[1024];
            int nbytes = stream.Read(outbuf, 0, outbuf.Length);
            string output = Encoding.ASCII.GetString(outbuf, 0, nbytes);
            // (5) 스트림과 TcpClient 객체 닫기
            stream.Close();
            tc.Close();
            Console.WriteLine($"{nbytes} bytes: {output}");
        }
    }
}
