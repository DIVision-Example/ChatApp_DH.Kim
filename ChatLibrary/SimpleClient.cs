using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using static ChatLibrary.PersonalPacket;

namespace ChatLibrary {
    public class SimpleClient {
        public Guid ClientId {
            get;
            private set;
        }

        public Socket Socket {
            get;
            private set;
        }

        public IPEndPoint EndPoint {
            get;
            private set;
        }

        public IPAddress Address {
            get;
            private set;
        }

        public bool IsConnected {
            get;
            set;
        }
        public bool IsGuidAssigned {
            get;
            set;
        }

        public int ReceiveBufferSize {
            get {
                return Socket.ReceiveBufferSize;
            }
            set {
                Socket.ReceiveBufferSize = value;
            }
        }
        public int SendBufferSize {
            get {
                return Socket.SendBufferSize;
            }
            set {
                Socket.SendBufferSize = value;
            }
        }

        // Constructor
        public SimpleClient() { }

        public SimpleClient(string address, int port) {
            IPAddress ipAddress;
            var validIp = IPAddress.TryParse(address, out ipAddress);

            /*
             * Dns 클래스는 특정 호스트 정보의 도메인 이름을 확인하는 기능을 제공하는 클래스
             * Dns 쿼리에서 검색된 호스트 정보는 IPHostEntry를 인스턴스 형태로 반환함
             * 
             * GetHostByName, GetHostAddresses 등의 메서드가 복수 개의 IP주소를 return함
             * IPAddress[] host = Dns.GetHostAddresses("www.microsoft.com");
             */
            if (!validIp) ipAddress = Dns.GetHostAddresses(address)[0]; // 잘못된 주소 처리

            Address = ipAddress;
            EndPoint = new IPEndPoint(ipAddress, port);
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            ReceiveBufferSize = 8000;
            SendBufferSize = 8000;
        }

        // Method
        public async Task<bool> Connect() {
            var result = await Task.Run(() => TryConnect());
            string guid = string.Empty;

            try {
                if(result) {
                    guid = RecieveGuid();
                    ClientId = Guid.Parse(guid);
                    IsGuidAssigned = true;
                    return true;
                }
            } catch (SocketException e) {
                // Process
            }
            return false;
        }


        public async Task<string> CreateGuid(Socket socket) {
            return await Task.Run(() => TryCreateGuid(socket));
        }

        public async Task<bool> SendMessage(string message) {
            return await Task.Run(() => TrySendMessage(message));
        }

        public async Task<bool> SendObject(object obj) {
            return await Task.Run(() => TrySendObject(obj));
        }

        public async Task<object> RecieveObject() {
            return await Task.Run(() => TryRecieveObject());
        }

        private object TryRecieveObject() {
            if (Socket.Available == 0)
                return null;

            byte[] data = new byte[Socket.ReceiveBufferSize];

            try {
                using (Stream s = new NetworkStream(Socket)) {
                    s.Read(data, 0, data.Length);
                    var memory = new MemoryStream(data);
                    memory.Position = 0;

                    var formatter = new BinaryFormatter();
                    var obj = formatter.Deserialize(memory);

                    return obj;
                }
            } catch (Exception e) {
                Console.WriteLine("TryRecieveObject " + e.Message);
                return null;
            }
        }

        private bool TrySendObject(object obj) {
            try {
                using (Stream s = new NetworkStream(Socket)) {
                    var memory = new MemoryStream();
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(memory, obj);

                    var newObj = memory.ToArray();

                    memory.Position = 0;
                    s.Write(newObj, 0, newObj.Length);
                    return true;
                }
            } catch (IOException e) {
                Console.WriteLine("TrySendObject " + e.Message);
                return false;
            }
        }

        public bool TrySendMessage(string message) {
            try {
                using (Stream s = new NetworkStream(Socket)) {
                    StreamWriter writer = new StreamWriter(s);
                    writer.AutoFlush = true;

                    writer.WriteLine(message);
                    return true;
                }
            } catch (IOException e) {
                Console.WriteLine("TrySendMessage " + e.Message);
                return false;
            }
        }

        private bool TryConnect() {
            try {
                Socket.Connect(EndPoint);
                return true;
            } catch {
                Console.WriteLine("Connection failed.");
                return false;
            }
        }

        public string RecieveGuid() {
            try {
                using (Stream s = new NetworkStream(Socket)) {
                    var reader = new StreamReader(s);
                    s.ReadTimeout = 5000;

                    return reader.ReadLine();
                }
            } catch (IOException e) {
                Console.WriteLine("RecieveGuid " + e.Message);
                return null;
            }
        }

        private string TryCreateGuid(Socket socket) {
            Socket = socket;
            var endPoint = ((IPEndPoint)Socket.LocalEndPoint);
            EndPoint = endPoint;

            ClientId = Guid.NewGuid();
            return ClientId.ToString();
        }

        //https://stackoverflow.com/questions/2661764/how-to-check-if-a-socket-is-connected-disconnected-in-c
        public bool IsSocketConnected() {
            try {
                bool part1 = Socket.Poll(5000, SelectMode.SelectRead);
                bool part2 = (Socket.Available == 0);
                if (part1 && part2)
                    return false;
                else
                    return true;
            } catch (ObjectDisposedException e) {
                Console.WriteLine("IsSocketConnected " + e.Message);
                return false;
            }
        }

        public async Task<bool> PingConnection() {
            try {
                var result = await SendObject(new PingPacket());
                return result;
            } catch (ObjectDisposedException e) {
                Console.WriteLine("IsSocketConnected " + e.Message);
                return false;
            }
        }

        public void Disconnect() {
            Socket.Close();
        }


    }    
}
