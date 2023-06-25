using System;
using System.Net.Sockets;
using System.Text;

namespace ConsoleClient {
    internal class Program {
        static void Main(string[] args) {
            TcpClient client = new TcpClient();
            client.Connect("localhost", 7000);

            byte[] buf = new byte[1024];

            while(true) {
                Console.Write("\nType Text: ");
                buf = Encoding.Default.GetBytes(Console.ReadLine());
                if (Encoding.Default.GetString(buf) == "") continue;
                else {
                    client.GetStream().Write(buf, 0, buf.Length);
                }
            }
            client.Close();
        }
    }
}
