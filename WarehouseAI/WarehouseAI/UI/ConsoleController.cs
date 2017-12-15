using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using WarehouseAI.Representation;
using WarehouseAI.ShortestPathGraph;

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
        public ConsoleController()
        {
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

        /// <summary>
        /// Initializes the Cache, must be invoked before many commands commands are called
        /// </summary>
        private void InitCache()
        {
            Console.WriteLine("Initializing cache...");
            Warehouse.Initialize();
            Algorithms.InitializeCache(ItemDatabase);
            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Creates a new Book order for the system, removing the book instances from the warehouse and sending the path to the WarehouseIO.
        /// </summary>
        /// <param name="books">The Ids of the books in the book order expected to be in numeral form.</param>
        private void OrderBooks(string[] books)
        {
            Console.WriteLine("Ordering books...");
            if (ItemDatabase.Items.Length <= 0 || Warehouse.Nodes == null)
            {
                ShowError("Error: no items in the database or warehouse did not contain any nodes");
                return;
            }
            /*Adds item to idb if books contains item id*/
            Item[] idb = ItemDatabase.Items.Where(item => books.Contains(item.Id.ToString())).ToArray();
            ShortestPathGraph<ShelfShortestPathGraphNode> shortestPathGraph = new ShortestPathGraph<ShelfShortestPathGraphNode>(
                Warehouse.Nodes.ToArray(), n => n is Shelf,
                s => new ShelfShortestPathGraphNode((Shelf)s));
            try
            {
                Node[] nodes;
                Algorithms.Weight(shortestPathGraph.AllNodes.Cast<Node>().ToArray(), idb, out nodes);
                StringBuilder sb = new StringBuilder();
                foreach (Shelf shelf in nodes.Where(n => n is ShelfShortestPathGraphNode).Cast<ShelfShortestPathGraphNode>().Select(n => (Shelf)n.Parent))
                {
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
                Console.WriteLine("Order placed.");
            }
            catch (Exception e)
            {
                ShowError("Error: " + e.Message);
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
            StringBuilder sb = new StringBuilder();

            Prompt();
            int row = Console.CursorTop;

            while (true) {//Run until termination by Quit()
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Tab:
                        Console.WriteLine();
                        ClearBelowLines(row);
                        Console.SetCursorPosition(Console.CursorLeft, row + 1);

                        ConsoleColor previousColour = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.DarkYellow;

                        int numberOfCommands = 0;
                        string currCommand = "";
                        foreach (var ck in _commands.Keys)
                        {
                            if (ck.StartsWith(sb.ToString()))
                            {
                                numberOfCommands++; //This is so stupid...
                                currCommand = ck;
                                Console.WriteLine(ck);
                            }
                        }
                        Console.ForegroundColor = previousColour;
                        if (numberOfCommands == 1)
                        {
                            Console.SetCursorPosition(Console.CursorLeft, row);
                            Prompt(append: currCommand);
                            sb.Clear();
                            sb.Append(currCommand);
                            break;
                        }

                        Console.SetCursorPosition(Console.CursorLeft, row);
                        Prompt(append: sb.ToString());
                        break;

                    case ConsoleKey.Backspace:
                        if (sb.Length > 0)
                        {
                            sb.Remove(sb.Length - 1, 1);
                            Console.Write("\b \b");
                        }
                        break;

                    case ConsoleKey.Enter:
                        ClearBelowLines(row);
                        Console.SetCursorPosition(Console.CursorLeft, row);
                        Command(sb.ToString());
                        sb.Clear();
                        row = Console.CursorTop;
                        break;

                    case ConsoleKey.OemPeriod:
                    case ConsoleKey.OemComma:
                    case ConsoleKey.OemMinus:
                        char c = ConvertOem(key);
                        sb.Append(c);
                        Console.Write(c);
                        break;

                    default:
                        sb.Append(char.ToLower((char) key));
                        Console.Write(char.ToLower((char)key));
                        break;
                }

            }
        }

        private char ConvertOem(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.OemPeriod:
                    return '.';
                case ConsoleKey.OemComma:
                    return ',';
                case ConsoleKey.OemMinus:
                    return '-';
            }
            return '?';
        }

        /// <summary>
        /// Clears console lines below the current cursor line/>
        /// </summary>
        /// <param name="row">The row to place the cursor after the clearing</param>
        private void ClearBelowLines(int row) {
            for (int i = 0; i < _commands.Count + 2; i++) {
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(Console.CursorLeft, row + i);
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
            Console.WriteLine();
            Command c;
            if (input.Length <= 0) return;
            if (_commands.TryGetValue(inputStrings[0].ToLower(), out c))
            {
                c.Action(inputStrings.Skip(1).ToArray());
            }
            else
            {
                ShowError("Command not found.");
            }
            Prompt(true);
        }

        private void ShowError(string message)
        {
            ConsoleColor previousColour = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = previousColour;
        }

        private void Prompt(bool newline = false, string append = "")
        {
            Console.Write(newline ? "\n> " + append : "> " + append);
        }

        /// <summary>
        /// Imports the WarehouseRepresentation from a file.
        /// </summary>
        /// <param name="args">Expects the path to the expected file as the first argument. Ignores any additional arguments given.</param>
        private void ImportWarehouse(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                ShowError("Import warehouse expects a path to a warehouse database");
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
                ShowError("Import failed...");
                ShowError("Error: " + e.Message);
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
                ShowError("Import items expects a path to a item database");
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
                ShowError("Import failed...");
                ShowError("Error: " + e.Message);
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
                ShowError("Import relations expects a path to a relation database");
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
                ShowError("Import failed...");
                ShowError("Error: " + e.Message);
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
                ShowError("Evaluation failed...");
                ShowError("Error: " + e.Message);
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
                ShowError("Error: The book with the specified ID was not found in the database.");
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
                    ShowError("Error: The specified shelf ID was not found in the database.");
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
                    ShowError("Error: One or more of the specified ID's was not found in the database, or in the wrong format");
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
                ShowError("Error: " + e.Message);
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
                    ShowError("Error: One or more of the specified ID's was not found in the database, or in the wrong format");
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
                ShowError("Adding books failed...");
                ShowError("Error: " + e.Message);
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
        /// <param name="args">
        /// Expects the first argument to be an unique ID numeral.
        /// Expects the second argument to be Either "Node" or "Shelf" but can also be left out to create a Node.
        /// Expects the third argument to be the nodes placement on the X axis.
        /// Expects the fourth argument to be the nodes placement on the Y axis.
        /// </param>
        private void AddNode(string[] args)
        {
            Console.WriteLine("Adding node...");

            CultureInfo c = (CultureInfo)CultureInfo.CurrentCulture.Clone();

            int relationalIndex = 0;
            try
            {
                Node newNode;
                int id = int.Parse(args[relationalIndex++]);
                switch (args[relationalIndex].ToLower())
                {
                    case "node":
                        newNode = new Node();
                        relationalIndex++;
                        break;
                    case "shelf":
                        newNode = new Shelf();
                        relationalIndex++;
                        break;
                    default:
                        newNode = new Node();
                        break;
                }
                newNode.Id = id;
                // Parse any float format with current culture
                newNode.X = float.Parse(args[relationalIndex++], NumberStyles.Any, c);
                newNode.Y = float.Parse(args[relationalIndex++], NumberStyles.Any, c);
                

                Warehouse.AddNode(newNode, args.Skip(relationalIndex).Select(s => int.Parse(s)).ToArray());

                PrintWarehouse();
                Console.WriteLine("Node added.");
            }
            catch (Exception e)
            {
                ShowError("Node was not added...");
                ShowError("Error: " + e.Message);
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
                ShowError("Error: The supplied arguments was not in the correct format.");
            }
            catch (IndexOutOfRangeException)
            {
                ShowError("Error: Not enough arguments was supplied.");
            }
            catch (InvalidOperationException)
            {
                ShowError("Error: The specified node id's was not found in the database.");
            }
            catch (Exception e)
            {
                ShowError("Error: " + e.Message);
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