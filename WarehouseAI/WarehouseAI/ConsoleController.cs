using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseAI
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
                {"distance", Distance},
                {"dist", Distance},
                {"quit", s => Quit()},
                {"q", s => Quit()},
            };
        }
        
        public void Start(params string[] args)
        {
            string arg = "";
            foreach (string s in args)
            {
                arg += s + " ";
            }
            foreach (string s in arg.Split('-'))
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
            string[] inputStrings = input.Split(' ');

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
            Console.WriteLine("Import complete.");
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
    }
}