using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WarehouseAI {
    public static class WarehouseServerIO {
        private const int ConnectionQueueAmount = 1;
        public static ManualResetEvent AllDone = new ManualResetEvent(false);
        private static readonly List<string> MessageLog = new List<string>();
        private static readonly Queue<string> DataQueue = new Queue<string>();
        public static event Action<string> MessageRecievedEvent;

        public static string GetMessageLogs()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in MessageLog)
            {
                sb.AppendLine(s);
            }
            return sb.ToString();
        }

        public static void ClearMessageLog() => MessageLog.Clear();

        public static void EnqueueRoute(string route)
        {
            DataQueue.Enqueue(route);
        }

        public static void StartListening() {
            IPAddress ipAddress = GetIP();
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 100);

            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try {
                socket.Bind(localEndPoint);
                socket.Listen(ConnectionQueueAmount);
                while (true) {
                    AllDone.Reset();
                    
                    socket.BeginAccept(new AsyncCallback(AcceptCallback), socket);

                    AllDone.WaitOne(); // Block current thread and wait for signal from other thread
                }
            } catch (Exception e) {
                MessageLog.Add(e.ToString());
            }
            
            Console.Read();
        }

        private static void AcceptCallback(IAsyncResult ar) { 
            AllDone.Set();

            Socket listener = (Socket) ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            StateObject state = new StateObject();
            state.WorkSocket = handler;
            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        private static void ReadCallback(IAsyncResult ar) {
            StateObject state = (StateObject) ar.AsyncState;
            Socket handler = state.WorkSocket;

            int bytesRead = handler.EndReceive(ar);

            if (bytesRead <= 0) return;
            state.Sb.Append(Encoding.UTF8.GetString(state.Buffer, 0, bytesRead));

            string content = state.Sb.ToString();
            if (content.EndsWith("<EOF>")) {
                MessageLog.Add($"Read {content.Length} chars from socket. \n Data : {content}");
                if (content == "@req")
                    if (DataQueue.Count <= 0)
                        Send(handler, "No routes available");
                    else
                        Send(handler, DataQueue.Dequeue());
                else
                {
                    Send(handler, "Message recieved");
                    MessageRecievedEvent?.Invoke(content.Remove(content.Length - 5));
                }
            }
            else {
                handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
        }

        private static void Send(Socket handler, string data) {
            byte[] byteData = Encoding.UTF8.GetBytes(data + "<EOF>");

            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar) {
            try {
                Socket handler = (Socket) ar.AsyncState;

                int bytesSent = handler.EndSend(ar);
                MessageLog.Add($"Sent {bytesSent} bytes to client.\n");

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            } catch (Exception e) {
                MessageLog.Add(e.ToString());
            }
        }

        internal static IPAddress GetIP()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ipAddress in host.AddressList)
            {
                if (ipAddress.AddressFamily==AddressFamily.InterNetwork)
                {
                    return ipAddress;
                }
            }
            return null;
        }


        private class StateObject {
            public Socket WorkSocket = null;
            public const int BufferSize = 1024;
            public byte[] Buffer = new byte[BufferSize];
            public StringBuilder Sb = new StringBuilder();
        }
    }
}