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
    }
}
