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
        private readonly byte[] _buffer = new byte[1024]; // Weird ass buffer
        public List<Socket> ClientSockets { get; set; }
        private readonly Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private const int Port = 100;
        private readonly IPAddress _ipAddress;
        public event Action<string> MessageRecieved;

        public WarehouseServerIO()
        {
            ClientSockets = new List<Socket>();
            // Provides an IP address that indicates that the server must listen for client activity on all network interfaces. This field is read-only.
            _ipAddress = IPAddress.Any; 
        }

        /// <summary>
        /// Binds IP-address and Port to the server's socket.
        /// Starts waiting for incoming connections.
        /// </summary>
        public void SetupServer()
        {
            _serverSocket.Bind(new IPEndPoint(_ipAddress, Port));
            _serverSocket.Listen(1);
            _serverSocket.BeginAccept(AcceptCallback, null);
        }

        /// <summary>
        /// Accepts a connection from a client, prepares to recieve data from the client and prepares for another client.
        /// </summary>
        /// <param name="asyncResult"></param> Todo: Write what this thing does when we are sure.
        private void AcceptCallback(IAsyncResult asyncResult)
        {
            Socket socket = _serverSocket.EndAccept(asyncResult);
            ClientSockets.Add(socket);
            // socket.RemoteEndPoint.ToString(); <- IP address of Client
            
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, socket); // Todo: Add exceptionhandling
            _serverSocket.BeginAccept(AcceptCallback, null);
        }

        /// <summary>
        /// Stops a recieve call and reads the data from a client and gives it to the MessageRecieved event.
        /// </summary>
        /// <param name="asyncResult"></param> Todo: Write what this thing does when we are sure.
        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            Socket socket = (Socket)asyncResult.AsyncState; // Todo: Implement casting exception handling
            if (socket.Connected)
            {
                int received;
                try
                {
                    // Ends a pending async read and stores the number of bytes recieved
                    received = socket.EndReceive(asyncResult);
                }
                catch (Exception)
                {
                    // Removes connection to the client causing an exception
                    // Todo: Maybe this can simply be done by ClientSockets.Remove(socket);
                    foreach (Socket clientSocket in ClientSockets)
                    {
                        if (clientSocket.RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                        {
                            ClientSockets.Remove(clientSocket);
                            clientSocket.Close();
                        }
                    }
                    return;
                }
                if (received != 0)
                {
                    byte[] dataBuffer = new byte[received];
                    Array.Copy(_buffer, dataBuffer, received);
                    string text = Encoding.ASCII.GetString(dataBuffer);

                    string response = string.Empty;
                    
                    // Method stops when recieving an end signal from client
                    // Todo: Consider changing stopsignal
                    if (text == "bye")
                    {
                        ClientSockets.Remove(socket);
                        socket.Close();
                        return;
                    }

                    // Invoke event method to handle recieved message
                    MessageRecieved?.Invoke(text);
                    response = "Client recieved: " + text;
                    SendData(socket, response);
                }
                else
                {
                    // Removes connection to the client causing an exception
                    // Todo: Not sure why this is done here
                    // Todo: Maybe this can simply be done by ClientSockets.Remove(socket);
                    foreach (Socket clientSocket in ClientSockets)
                    {
                        if (clientSocket.RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                        {
                            ClientSockets.Remove(clientSocket);
                            clientSocket.Close();
                        }
                    }
                }
            }
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, socket);
        }

        /// <summary>
        /// Sends specified data to a connected socket.
        /// </summary>
        /// <param name="socket">The socket recieving the message.</param>
        /// <param name="message">The message that will be sent to the specified socket.</param>
        private void SendData(Socket socket, string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallback, socket); // Todo: Add exceptionhandling
            _serverSocket.BeginAccept(AcceptCallback, null);
        }

        /// <summary>
        /// Ends a pending async send.
        /// </summary>
        /// <param name="ascyncResult"></param> Todo: Write what this thing does when we are sure.
        private static void SendCallback(IAsyncResult ascyncResult)
        {
            Socket socket = (Socket)ascyncResult.AsyncState; // Todo: Implement some casting exception handling
            socket.EndSend(ascyncResult); // Todo: Add exceptionhandling
        }
    }
}
