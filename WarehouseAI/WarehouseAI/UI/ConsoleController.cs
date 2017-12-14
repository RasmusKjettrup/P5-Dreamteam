using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using WarehouseAI.Representation;

namespace WarehouseAI.UI
{
    /// <summary>
    /// This class handles interaction with the server. Commands must be defined here.
    /// </summary>
    public class ConsoleController : IController
    {
        /// <summary>
        /// The representation of the warehouse.
        /// This must be instantiated before calling Start()
        /// </summary>
        public WarehouseRepresentation Warehouse { get; set; }
        /// <summary>
        /// The ItemDatabase used for the warehouse.
        /// This must be instantiated before calling Start()
        /// </summary>
        public ItemDatabase ItemDatabase { get; set; }

        /// <summary>
        /// A private dictionary containing all of the commands in the controller
        /// </summary>
        private readonly Dictionary<string, Command> _commands;
        
        /// <summary>
        /// Initializes all commands of the controller.
        /// </summary>
        public ConsoleController() {
            _commands = new Dictionary<string, Command> {
                {"importwarehouse", new Command(ImportWarehouse, "Imports a warehouse from a file, expects the path to a file")},
                {"importitems", new Command(ImportItems, "Imports items from a file, expects the path to a file.")},
                {"importrelations", new Command(ImportRelations, "Imports relations from a file, expects the path to a file.")},
                {"evaluate", new Command(s => EvaluateWarehouse(), "Evaluates the warehouse.")},
                {"eval", new Command(s => EvaluateWarehouse(), "Evaluates the warehouse.")},
                {"addnode", new Command(AddNode, "Adds a node to the warehouse, expects the type of node(shelf or node), the x coordinate, and the y coodinate.")},
                {"addbook", new Command(AddBook, "Adds a book to the warehouse, expects the ID of a book.")},
                {"addbooks", new Command(AddBooks, "Adds books to the warehouse, expects the ID of the books.")},
                {"randomaddbooks", new Command(RandomAddBooks, "Adds books to random places of the warehouse, expects the ID of the books.")},
                {"distance", new Command(Distance, "Calculates the distance between two nodes, expects the id of the first node, and the id of the second node.")},
                {"dist", new Command(Distance, "Calculates the distance between two nodes, expects the id of the first node, and the id of the second node.")},
                {"quit", new Command(s => Quit(), "Terminates the program.")},
                {"q", new Command(s => Quit(), "Terminates the program.")},
                {"help", new Command(PrintAllCommands, "Prints all commands or specifies specific commands.")},
                {"printlog",  new Command(s => Console.WriteLine(WarehouseServerIO.GetMessageLogs()), "Prints the serverlogs between client and server.")},
                {"clearlog",  new Command(s => WarehouseServerIO.ClearMessageLog(), "Clears the serverlogs between client and server.")},
                {"showip", new Command(s => Console.WriteLine(WarehouseServerIO.GetIP().ToString()), "Shows the IP-address of the server.")},
                {"orderbooks", new Command(OrderBooks, "Sends an order of bought books to the warehouse, expects the id of each bought book.")},
                {"init", new Command(s => InitCache(), "Initializes the cache. Need to do this after importing.")}
            };
            WarehouseServerIO.MessageRecievedEvent += WarehouseServerIOOnMessageRecievedEvent;
        }

        private void InitCache()
        {
            Console.WriteLine("Initializing cache...");
            Warehouse.Initialize();
            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Creates a new Book order for the system, removing the book instances from the warehouse and sending the path to the WarehouseIO.
        /// </summary>
        /// <param name="books">The Ids of the books in the book order expected to be in numeral form.</param>
        private void OrderBooks(string[] books)
        {
            if (ItemDatabase.Items.Length <= 0 || Warehouse.Nodes == null)
            {
                Console.WriteLine("Error: no items in the database or warehouse did not contain any nodes");
                return;
            }
            /*Adds item to idb if books contains item id*/
            Item[] idb = ItemDatabase.Items.Where(item => books.Contains(item.Id.ToString())).ToArray();
            try
            {
                Node[] nodes;
                Algorithms.Weight(Warehouse.Nodes, idb, out nodes);
                StringBuilder sb = new StringBuilder();
                foreach (Node node in nodes)
                {
                    Shelf shelf = node as Shelf;
                    if (shelf == null) continue;
                    sb.Append(shelf.Id);
                    foreach (Item item in shelf.Items)
                    {
                        if (books.Contains(item.Id.ToString()))
                        {
                            sb.AppendLine(item.Name);
                            shelf.RemoveBook(item);
                            books[Array.IndexOf(books, item.Id.ToString())] = null;
                        }
                    }
                }
                WarehouseServerIO.EnqueueRoute(sb.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        /// <summary>
        /// This event occurs whenever the server recieves an input from a client that tries to add a new book instance to the warehouse.
        /// </summary>
        /// <param name="message">The message recieved from the event.</param>
        private void WarehouseServerIOOnMessageRecievedEvent(string message)
        {
            AddBook(new[] { message });
        }

        /// <summary>
        /// This starts the controller and enables the UI in the form of console commands.
        /// The server will be set up in a seperate thread to prevent disruptions in the UI.
        /// Start accepts an amount of arguments on the form: -arg1 -arg2.
        /// </summary>
        /// <param name="args">The commands that will be run before any other user input.</param>
        public void Start(params string[] args)
        {
            string arg = "";
            new Thread(WarehouseServerIO.StartListening).Start();

            foreach (string s in args)
            {
                arg += s + " ";
            }
            foreach (string s in arg.Split('-').Where(s => s != ""))
            {
                Console.WriteLine(s);
                Command(s);
            }
            Console.WriteLine("Please enter a command.\nFor a list of all commands type: help");
            while (true) //Run until termination by Quit()
            {
                Console.Write("> ");
                Command(Console.ReadLine());
            }
        }

        /// <summary>
        /// Invokes the expected method given a command string.
        /// The first part of the input must always be the Command key  while all of the following parts are expected to be arguments.
        /// </summary>
        /// <param name="input">The inputstring given to the command. Expects a space between each seperate input in the string.</param>
        private void Command(string input)
        {
            string[] inputStrings = input.Split(' ').Where(s => s != "").ToArray();

            Command c;
            if (input.Length <= 0) return;
            if (_commands.TryGetValue(inputStrings[0].ToLower(), out c))
            {
                c.Action(inputStrings.Skip(1).ToArray());
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Imports the WarehouseRepresentation from a file.
        /// </summary>
        /// <param name="args">Expects the path to the expected file as the first argument. Ignores any additional arguments given.</param>
        private void ImportWarehouse(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                Console.WriteLine("Import warehouse expects a path to a warehouse database");
                return;
            }
            Console.WriteLine("Now importing warehouse...");
            try
            {
                Warehouse.ImportWarehouse(args[0]);
                PrintWarehouse();

                Console.WriteLine("Import complete.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Import failed...");
                Console.WriteLine("Error: " + e.Message);
            }

        }

        /// <summary>
        /// Imports the ItemDatabase from a file.
        /// </summary>
        /// <param name="args">Expects the path to the expected file as the first argument. Ignores any additional arguments given.</param>
        private void ImportItems(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                Console.WriteLine("Import items expects a path to a item database");
                return;
            }
            Console.WriteLine("Importing items...");
            try
            {
                ItemDatabase.ImportItems(args[0]);
                foreach (Item item in ItemDatabase.Items)
                {
                    Console.WriteLine($"{item.Id}: {item.Name}");
                }
                Console.WriteLine("Import complete.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Import failed...");
                Console.WriteLine("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Imports the Relations between books from a file.
        /// </summary>
        /// <param name="args">Expects the path to the expected file as the first argument. Ignores any additional arguments given.</param>
        private void ImportRelations(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                Console.WriteLine("Import relations expects a path to a relation database");
                return;
            }
            Console.WriteLine("Importing relations on items...");
            try
            {
                ItemDatabase.ImportRelations(args[0]);
                foreach (Item item in ItemDatabase.Items)
                {
                    string neighbours = "";
                    for (int i = 0; i < item.Neighbours().Length; i++)
                    {
                        neighbours += item.Neighbours()[i].Id;
                        if (i != item.Neighbours().Length - 1)
                        {
                            neighbours += " ";
                        }
                    }
                    Console.WriteLine($"{item.Id}: [{neighbours}]");
                }
                Console.WriteLine("Import complete.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Import failed...");
                Console.WriteLine("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Evaluates the current configuration of the warehouse.
        /// </summary>
        private void EvaluateWarehouse()
        {
            Console.WriteLine("Evaulating warehouse state...");
            try
            {
                double result = Warehouse.Evaluate();
                Console.WriteLine("Result: " + result);
                Console.WriteLine("Evaluation finished.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Evaluation failed...");
                Console.WriteLine("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Adds a book instance to the warehouse or to a specific shelf in the warehouse.
        /// </summary>
        /// <param name="args">Expects the first part of the input to be the ID of a book. 
        /// If additional arguments are given the second argument must be ID of a shelf in the warehouse.
        /// Any additional arguments will be ignored.</param>
        private void AddBook(string[] args)
        {
            Console.WriteLine("Adding item...");
            Item item;
            try
            {
                item = ItemDatabase.Items.First(i => i.Id == int.Parse(args[0]));
            }
            catch
            {
                Console.WriteLine("Error: The book with the specified ID was not found in the database.");
                return;
            }
            if (args.Length == 1)
            {
                Warehouse.AddBook(item);
            }
            else
            {
                Shelf shelf;
                try
                {
                    shelf = (Shelf)Warehouse.Nodes.First(n => n.Id == int.Parse(args[1]));
                }
                catch
                {
                    Console.WriteLine("Error: The specified shelf ID was not found in the database.");
                    return;
                }
                shelf.AddBook(item);
            }

            PrintItemsOnShelves();
            Console.WriteLine("Book added.");
        }

        /// <summary>
        /// Add a number of books to the warehouse.
        /// </summary>
        /// <param name="args">Expects the ID's of the book instances to be added to the warehouse.</param>
        private void AddBooks(string[] args)
        {
            Console.WriteLine("Adding items...");
            List<Item> items = new List<Item>();
            foreach (string s in args)
            {
                try
                {
                    Item item = ItemDatabase.Items.First(i => i.Id == int.Parse(s));
                    items.Add(item);
                }
                catch
                {
                    Console.WriteLine("Error: One or more of the specified ID's was not found in the database, or in the wrong format");
                    return;
                }
            }
            try
            {
                Warehouse.AddBooks(items.ToArray());

                PrintItemsOnShelves();
                Console.WriteLine("Books added.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Places a number of books randomly in the warehouse.
        /// Primarily used for showcasing and testing purposes.
        /// </summary>
        /// <param name="args">The ID's of the books to be added to the warehouse.</param>
        private void RandomAddBooks(string[] args)
        {
            Console.WriteLine("Adding books at random places...");
            List<Item> items = new List<Item>();
            foreach (string s in args)
            {
                try
                {
                    Item item = ItemDatabase.Items.First(i => i.Id == int.Parse(s));
                    items.Add(item);
                }
                catch
                {
                    Console.WriteLine("Error: One or more of the specified ID's was not found in the database, or in the wrong format");
                    return;
                }
            }
            try
            {
                Warehouse.RandomlyAddBooks(items.ToArray());

                PrintItemsOnShelves();
                Console.WriteLine("Done adding books.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Adding books failed...");
                Console.WriteLine("Error: " + e.Message);
            }

        }

        /// <summary>
        /// Prints each shelf in the warehouse and what books are contained in it.
        /// </summary>
        private void PrintItemsOnShelves()
        {
            foreach (Node node in Warehouse.Nodes)
            {
                Shelf shelf = node as Shelf;
                if (shelf != null)
                {
                    string items = "";
                    for (int i = 0; i < shelf.Items.Length; i++)
                    {
                        items += shelf.Items[i].Id;
                        if (i != shelf.Items.Length - 1)
                        {
                            items += " ";
                        }
                    }
                    Console.WriteLine($"{shelf.Id}: [{items}]");
                }
            }
        }

        /// <summary>
        /// Adds a node to the warehouse and prints the warehouse afterwards.
        /// </summary>
        /// <param name="args">Expects the first argument to be Either "Node" or "Shelf" but will default to Node.
        /// Expects the second argument to be the nodes placement on the X axis.
        /// Expects the second argument to be the nodes placement on the Y axis.</param>
        private void AddNode(string[] args)
        {
            Console.WriteLine("Adding node...");

            CultureInfo c = (CultureInfo)CultureInfo.CurrentCulture.Clone();

            int relationalIndex = 0;
            try
            {
                Node newNode;
                switch (args[relationalIndex])
                {
                    case "Node":
                        newNode = new Node();
                        break;
                    case "Shelf":
                        newNode = new Shelf();
                        break;
                    default:
                        newNode = new Node();
                        relationalIndex--;
                        break;
                    
                }
                // Parse any float format with current culture
                newNode.X = float.Parse(args[relationalIndex + 1], NumberStyles.Any, c);
                newNode.Y = float.Parse(args[relationalIndex + 2], NumberStyles.Any, c);

                Warehouse.AddNode(newNode, args.Skip(relationalIndex + 3).Select(s => int.Parse(s)).ToArray());

                PrintWarehouse();
                Console.WriteLine("Node added.");
            } catch (Exception e) {
                Console.WriteLine("Node was not added...");
                Console.WriteLine("Error: " + e.Message);
            }

        }

        /// <summary>
        /// Calculates the distance between two nodes
        /// </summary>
        /// <param name="args">Expects two arguments in the form o Node ID's; 
        /// The node to calculate the distance from, And the node to calculate the distance to.
        /// Any additional arguments will be discarded.</param>
        private void Distance(string[] args)
        {
            try
            {
                Node from = Warehouse.Nodes.First(n => n.Id == int.Parse(args[0]));
                Node to = Warehouse.Nodes.First(n => n.Id == int.Parse(args[1]));
                //AStarAlgorithm aStar = new AStarAlgorithm();
                //float weight = aStar.FindPath(warehouse.Nodes, from, to);
                float weight = from.Edges.First(e => e.to == to).weight;
                Console.WriteLine("Distance between " + from.Id + " and " + to.Id + ": " + weight);
            }
            catch (FormatException)
            {
                Console.WriteLine("Error: The supplied arguments was not in the correct format.");
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Error: Not enough arguments was supplied.");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Error: The specified node id's was not found in the database.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Exits the application.
        /// </summary>
        private static void Quit()
        {
            Console.WriteLine("Now quitting...");
            Environment.Exit(0);
        }

        /// <summary>
        /// Prints each node in the warehouse by ID Type Placement & [Neighbours].
        /// </summary>
        private void PrintWarehouse()
        {
            foreach (Node node in Warehouse.Nodes)
            {
                string typ = "Node";
                if (node is Shelf)
                {
                    typ = "Shelf";
                }

                string neighbours = "";
                for (int i = 0; i < node.Neighbours.Length; i++)
                {
                    neighbours += node.Neighbours[i].Id;
                    if (i != node.Neighbours.Length - 1)
                    {
                        neighbours += " ";
                    }
                }

                Console.WriteLine($"{node.Id} {typ} ({node.X + " " + node.Y}) [{neighbours}]");
            }
        }
        
        /// <summary>
        /// Prints all commands available
        /// </summary>
        private void PrintAllCommands(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                foreach (string s in args)
                {
                    if (_commands.ContainsKey(s))
                    {
                        Console.WriteLine(s + " - " + _commands[s].Description);
                    }
                }
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            Console.WriteLine("The help command followed by any number of commands will describe the function of each command.");
            Console.ForegroundColor = ConsoleColor.Green;
            // Sort commands alphabetically and print
            foreach (string commandsKey in _commands.Keys.OrderBy(key => key))
            {
                Console.WriteLine(commandsKey);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}