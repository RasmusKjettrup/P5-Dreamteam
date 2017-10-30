using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseAI
{
    public class Program
    {
        private static void Main(string[] args)
        {
            WarehouseServerIO server = new WarehouseServerIO();
            server.SetupServer();
            server.MessageRecieved += ServerOnMessageRecieved;

            bool running = true;
            while (running)
            {
                string input = Console.ReadLine()?.ToLower();
                switch (input)
                {
                    case "\\list-clients": ListAllClients(server); break;
                    case "\\stop": running = false; break;
                    default: Console.WriteLine($"Unknown command {input}"); break;
                }
            }
        }

        private static void ServerOnMessageRecieved(string s)
        {
            throw new NotImplementedException();
        }

        private static void ListAllClients(WarehouseServerIO server)
        {
            foreach (Socket serverClientSocket in server.ClientSockets)
            {
                Console.WriteLine(serverClientSocket.RemoteEndPoint.ToString());
            }
        }
    }
}
