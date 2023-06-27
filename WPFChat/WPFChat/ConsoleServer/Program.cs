using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConsoleServer {
    class Program {
        public List<Baby> school = new List<Baby>();

        public void MyWorld() {
            TcpListener listener = new TcpListener(IPAddress.Any, 7000);
            listener.Start();
            Debug.WriteLine("서버 실행");
            while (true) {
                TcpClient childSocket = listener.AcceptTcpClient();
                Debug.WriteLine("새 클라이언트 연결됨");
                school.Add(new Baby(childSocket, this));
            }
        }
        static void Main(string[] args) {
            new Program().MyWorld();
        }
    }

    class Baby {
        byte[] buffer = new byte[1024];

        TcpClient socket = new TcpClient();
        Program world = new Program();
        NetworkStream stream = null;

        string ClientName;

        public Baby(TcpClient socket, Program world) {
            this.socket = socket;
            this.world = world;
            stream = socket.GetStream();
            Thread thread = new Thread(Run);
            thread.Start();

        }
        public void Broadcast(string message) {
            byte[] buffer = Encoding.Default.GetBytes("안녕하세요");
            stream.Write(buffer, 0, buffer.Length);
        }

        public void Run() {
            Debug.WriteLine("새로운 Baby");

            try {
                while (true) {
                    byte[] buffer = new byte[1024];
                    int nBytes = stream.Read(buffer, 0, buffer.Length);
                    string message = Encoding.Default.GetString(buffer);

                    Debug.WriteLine(message);

                    if (nBytes > 0) {
                        // 모두에게
                        if (true) {
                            foreach (Baby student in world.school) {
                                student.Broadcast(message);
                            }
                        } else {
                            stream.Write(buffer, 0, buffer.Length);
                        }
                    }

                    //Debug.WriteLine(message);
                    Debug.WriteLine(message);
                }
            } catch (Exception e) {
                Debug.WriteLine(e.ToString());
            }

            Debug.WriteLine("*************");
            Debug.WriteLine("**** AND ****");
            Debug.WriteLine("*************");

            stream.Close();
            socket.Close();
        }
        // 모바텀을 설치하고,클아이언트 대신사용한다.
        // 프로토콜은 Telnet을 선택하고, ip, port 를 서버것을 사용한다.
    }
}