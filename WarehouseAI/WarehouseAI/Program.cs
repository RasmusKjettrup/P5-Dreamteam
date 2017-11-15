using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseAI
{
    public static class Program
    {
        private const string CommandList = "\\help";
        private static readonly Dictionary<string, Delegate> Commands = new Dictionary<string, Delegate>();
        private static WarehouseServerIO _server;
        private static bool _running = true;

        private static void Main(string[] args)
        {
            Debug.WriteLine("Test");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Setting up _server...");
            _server = new WarehouseServerIO();
            _server.SetupServer();
            _server.MessageRecieved += ServerOnMessageRecieved;
            _server.ErrorOccured += ServerOnErrorOccured;

            Console.WriteLine("Server setup complete");
            Console.WriteLine($"Type {CommandList} for all commands");

            CommandSetup();
            while (_running)
            {
                Console.Write("Please enter a command: ");
                Console.ForegroundColor = ConsoleColor.White;
                string input = Console.ReadLine()?.ToLower();
                Console.ForegroundColor = ConsoleColor.Cyan;
                if (input != null && Commands.ContainsKey(input))
                {
                    Commands[input].DynamicInvoke();
                }
                else
                {
                    Console.WriteLine($"Unknown command {input}");
                }
            }
        }
        /// <summary>
        /// Sets up the CLI
        /// </summary>
        private static void CommandSetup()
        {
            Commands.Add(CommandList, new Action(PrintAllCommands));
            Commands.Add("\\list-clients", new Action(ListAllClients));
            Commands.Add("\\exit", new Action(() => { _running = false; }));
        }

        /// <summary>
        /// Marks the error with red
        /// </summary>
        /// <param name="s">Error occured</param>
        private static void ServerOnErrorOccured(string s)
        {
            ConsoleColor currentConsoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(s);
            Console.ForegroundColor = currentConsoleColor;
        }
        /// <summary>
        /// Prints all commands available
        /// </summary>
        private static void PrintAllCommands()
        {
            foreach (string commandsKey in Commands.Keys)
            {
                Console.WriteLine(commandsKey);
            }
        }

        private static void ServerOnMessageRecieved(string data, string client)
        {
            switch (data[0])
            {
                case 'Q': break; // Todo: Qr Message was recieved from client
                case 'R': break; // Todo: Route request recieved from client
                default: Console.WriteLine($"The following message was recieved from the IP {client}: {data}"); break;
            }
        }
        /// <summary>
        /// Prints the list of clients
        /// </summary>
        private static void ListAllClients()
        {
            if (_server.ClientSockets.Count < 1)
            {
                Console.WriteLine("There is currently no clients connected to the server");
                return;
            }
            foreach (Socket serverClientSocket in _server.ClientSockets)
            {
                Console.WriteLine(serverClientSocket.RemoteEndPoint.ToString());
            }
        }
    }
}