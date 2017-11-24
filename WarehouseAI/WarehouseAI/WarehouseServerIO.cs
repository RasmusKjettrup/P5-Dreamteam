using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WarehouseAI {
    public static class WarehouseServerIO {
        private const int ConnectionQueueAmount = 1;
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public static void StartListening() {
            IPAddress ipAddress = GetIP();
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 100);

            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try {
                socket.Bind(localEndPoint);
                socket.Listen(ConnectionQueueAmount);
                while (true) {
                    allDone.Reset();

                    Console.WriteLine("Waiting for a connection...");
                    socket.BeginAccept(new AsyncCallback(AcceptCallback), socket);

                    allDone.WaitOne(); // Block current thread and wait for signal from other thread
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress enter to continue...");
            Console.Read();
        }

        private static void AcceptCallback(IAsyncResult ar) { 
            allDone.Set();

            Socket listener = (Socket) ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        private static void ReadCallback(IAsyncResult ar) {
            StateObject state = (StateObject) ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0) {
                state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));

                var content = state.sb.ToString();
                if (content.EndsWith("<EOF>")) {
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);
                    Send(handler, "sErVeR sAyS hElLo!!! as well as " + content);
                    Console.WriteLine("Message sent.");
                }
                else {
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private static void Send(Socket handler, string data) {
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar) {
            try {
                Socket handler = (Socket) ar.AsyncState;

                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.\n", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private static IPAddress GetIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ipAddress in host.AddressList)
            {
                if (ipAddress.AddressFamily==AddressFamily.InterNetwork)
                {
                    return ipAddress;
                }
            }
            return null;
        }


        private class StateObject {
            public Socket workSocket = null;
            public const int BufferSize = 1024;
            public byte[] buffer = new byte[BufferSize];
            public StringBuilder sb = new StringBuilder();
        }
    }
}