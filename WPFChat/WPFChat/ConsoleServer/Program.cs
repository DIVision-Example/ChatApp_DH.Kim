using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleServer {
    public class Program {
        static void Main(string[] args) {
            TcpListener server = new TcpListener(IPAddress.Any, 7000);
            server.Start();
            Console.WriteLine("Server Activated.");

            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Client Connected.");

            while(true) {
                // Socket은 Byte 형식으로 데이터를 교환한다
                byte[] buffer = new byte[1024];

                client.GetStream().Read(buffer, 0, buffer.Length);
                string strData = Encoding.Default.GetString(buffer);

                int endPoint = strData.IndexOf("\0");
                string parsedMessage = strData.Substring(0, endPoint + 1);

                Console.WriteLine(parsedMessage);
            }
        }
    }
}
