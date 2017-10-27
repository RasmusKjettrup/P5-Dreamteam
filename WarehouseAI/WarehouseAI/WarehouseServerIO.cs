using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseAI
{
    internal class WarehouseServerIO
    {
        private byte[] _buffer = new byte[1024]; // Weird ass buffer
        public List<Socket> ClientSockets { get; set; }
        private Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public WarehouseServerIO()
        {
            ClientSockets = new List<Socket>();
        }

        public void SetupServer()
        {
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 100));
            _serverSocket.Listen(1);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private void AcceptCallback(IAsyncResult asyncResult)
        {
            Socket socket = _serverSocket.EndAccept(asyncResult);
            ClientSockets.Add(socket);
            // socket.RemoteEndPoint.ToString(); <- IP address of Client
            
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {

            Socket socket = (Socket)asyncResult.AsyncState;
            if (socket.Connected)
            {
                int received;
                try
                {
                    received = socket.EndReceive(asyncResult);
                }
                catch (Exception)
                {
                    // client close connection
                    foreach (Socket clientSocket in ClientSockets)
                    {
                        if (clientSocket.RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                        {
                            ClientSockets.Remove(clientSocket);
                        }
                    }
                    return;
                }
                if (received != 0)
                {
                    byte[] dataBuf = new byte[received];
                    Array.Copy(_buffer, dataBuf, received);
                    string text = Encoding.ASCII.GetString(dataBuf);

                    string reponse = string.Empty;
                    //if (text.Contains("@@"))
                    //{
                    //    for (int i = 0; i < list_Client.Items.Count; i++)
                    //    {
                    //        if (socket.RemoteEndPoint.ToString().Equals(__ClientSockets[i]._Socket.RemoteEndPoint.ToString()))
                    //        {
                    //            list_Client.Items.RemoveAt(i);
                    //            list_Client.Items.Insert(i, text.Substring(1, text.Length - 1));
                    //            __ClientSockets[i]._Name = text;
                    //            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                    //            return;
                    //        }
                    //    }
                    //}
                    
                    if (text == "bye")
                    {
                        return;
                    }
                    reponse = "dick recieved" + text;
                    Sendata(socket, reponse);
                }
                else
                {
                    for (int i = 0; i < __ClientSockets.Count; i++)
                    {
                        if (__ClientSockets[i]._Socket.RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                        {
                            __ClientSockets.RemoveAt(i);
                            lb_soluong.Text = "Number of clients connected: " + __ClientSockets.Count.ToString();
                        }
                    }
                }
            }
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }
    }
}
