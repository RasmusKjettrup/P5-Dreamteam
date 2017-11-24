using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WarehouseAI.Representation;

namespace WarehouseAI.UI
{
    public class ConsoleController : IController
    {
        public WarehouseRepresentation warehouse { get; set; }
        public ItemDatabase itemDatabase { get; set; }

        private readonly Dictionary<string, Action<string[]>> commands;

        private bool quit = false;

        public ConsoleController()
        {
            commands = new Dictionary<string, Action<string[]>>
            {
                {"importwarehouse", ImportWarehouse},
                {"importitems", ImportItems},
                {"importrelations", ImportRelations},
                {"evaluate", s=> EvaluateWarehouse() },
                {"eval", s=> EvaluateWarehouse() },
                {"addnode", AddNode },
                {"addbook", AddBook},
                {"addbooks", AddBooks},
                {"distance", Distance},
                {"dist", Distance},
                {"quit", s => Quit()},
                {"q", s => Quit()},
                {"help", PrintAllCommands},
            };
        }

        public void Start(params string[] args)
        {
            string arg = "";
            foreach (string s in args)
            {
                arg += s + " ";
            }
            foreach (string s in arg.Split('-').Where(s => s != ""))
            {
                Console.WriteLine(s);
                Command(s);
            }

            while (!quit)
            {
                Command(Console.ReadLine());
            }
        }

        private void Command(string input)
        {
            string[] inputStrings = input.Split(' ').Where(s => s != "").ToArray();

            Action<string[]> c;
            if (commands.TryGetValue(inputStrings[0].ToLower(), out c))
            {
                c(inputStrings.Skip(1).ToArray());
            }
        }

        private void ImportWarehouse(string[] args)
        {
            Console.WriteLine("Now importing warehouse...");
            warehouse.ImportWarehouse(args[0]);

            PrintWarehouse();
            Console.WriteLine("Import complete.");
        }

        private void ImportItems(string[] args)
        {
            Console.WriteLine("Importing items...");
            itemDatabase.ImportItems(args[0]);
            foreach (Item item in itemDatabase.Items)
            {
                Console.WriteLine(@"{0}: {1}", item.Id, item.Name);
            }
            Console.WriteLine("Import complete.");
        }

        private void ImportRelations(string[] args)
        {
            Console.WriteLine("Importing relations on items...");
            itemDatabase.ImportRelations(args[0]);
            foreach (Item item in itemDatabase.Items)
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
                Console.WriteLine(@"{0}: [{1}]", item.Id, neighbours);
            }
            Console.WriteLine("Import complete.");
        }

        private void EvaluateWarehouse()
        {
            Console.WriteLine("Evaulating warehouse state...");
            double result = warehouse.Evaluate();
            Console.WriteLine("Result: " + result);
            Console.WriteLine("Evaluation finished.");
        }

        private void AddBook(string[] args)
        {
            Console.WriteLine("Adding item...");
            Item item;
            try
            {
                item = itemDatabase.Items.First(i => i.Id == int.Parse(args[0]));
            }
            catch
            {
                Console.WriteLine("The book with the specified ID was not found in the database.");
                return;
            }
            if (args.Length == 1)
            {
                warehouse.AddBook(item);
            }
            else
            {
                Shelf shelf;
                try
                {
                    shelf = (Shelf)warehouse.Nodes.First(n => n.Id == int.Parse(args[1]));
                }
                catch
                {
                    Console.WriteLine("The specified shelf ID was not found in the database.");
                    return;
                }
                shelf.AddBook(item);
            }

            PrintItemsOnShelves();
            Console.WriteLine("Book added.");
        }

        private void AddBooks(string[] args)
        {
            Console.WriteLine("Adding items...");
            List<Item> items = new List<Item>();
            foreach (string s in args)
            {
                try
                {
                    Item item = itemDatabase.Items.First(i => i.Id == int.Parse(s));
                    items.Add(item);
                }
                catch
                {
                    Console.WriteLine("One or more of the specified ID's was not found in the database, or in the wrong format");
                    return;
                }
            }
            warehouse.AddBooks(items.ToArray());

            PrintItemsOnShelves();
            Console.WriteLine("Books added.");
        }

        private void PrintItemsOnShelves()
        {
            foreach (Node node in warehouse.Nodes)
            {
                if (node is Shelf)
                {
                    Shelf shelf = (Shelf)node;
                    string items = "";
                    for (var i = 0; i < shelf.Items.Length; i++)
                    {
                        items += shelf.Items[i].Id;
                        if (i != shelf.Items.Length - 1)
                        {
                            items += " ";
                        }
                    }
                    Console.WriteLine(@"{0}: [{1}]", node.Id, items);
                }
            }
        }

        private void AddNode(string[] args)
        {
            Console.WriteLine("Adding node...");

            CultureInfo c = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            Node newNode = null;

            int relationalIndex = 0;
            try
            {
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
                newNode.X = float.Parse(args[relationalIndex + 1], NumberStyles.Any, c);
                newNode.Y = float.Parse(args[relationalIndex + 2], NumberStyles.Any, c);
            }
            catch { }

            warehouse.AddNode(newNode, args.Skip(relationalIndex + 3).Select(s => int.Parse(s)).ToArray());

            PrintWarehouse();
            Console.WriteLine("Node added.");
        }

        private void Distance(string[] args)
        {
            try
            {
                Node from = warehouse.Nodes.First(n => n.Id == int.Parse(args[0]));
                Node to = warehouse.Nodes.First(n => n.Id == int.Parse(args[1]));
                //AStarAlgorithm aStar = new AStarAlgorithm();
                //float weight = aStar.FindPath(warehouse.Nodes, from, to);
                float weight = from.Edges.First(e => e.to == to).weight;
                Console.WriteLine("Distance between " + from.Id + " and " + to.Id + ": " + weight);
            }
            catch (FormatException)
            {
                Console.WriteLine("The supplied arguments was not in the correct format.");
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Not enough arguments was supplied.");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("The specified node id's was not found in the database.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Quit()
        {
            Console.WriteLine("Now quitting...");
            quit = true;
        }

        private void PrintWarehouse()
        {
            foreach (Node node in warehouse.Nodes)
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

                Console.WriteLine(@"{0} {1} ({2}) [{3}]", node.Id, typ, node.X + " " + node.Y, neighbours);
            }
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
        private void PrintAllCommands(string[] args)
        {
            foreach (string commandsKey in commands.Keys)
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
    }
}