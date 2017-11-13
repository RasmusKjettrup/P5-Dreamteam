using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace WarehouseAI
{
    public class WarehouseRepresentation
    {
        public ItemDatabase ItemDatabase;

        private Node[] _nodes;
        public Node[] Nodes => _nodes;
        private WeightCache _cache;

        private Network<Shelf> _subNetwork;

        public void ImportWarehouse(string path)
        {
            CultureInfo c = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            c.NumberFormat.CurrencyDecimalSeparator = ".";

            string[][] lines = File.ReadAllLines(path).Select(s => s.Split(',').Select(t => t.Trim()).ToArray()).ToArray();

            List<Node> nodes = new List<Node>();

            foreach (string[] line in lines)
            {
                try
                {
                    Node newNode;

                    switch (line[1])
                    {
                        case "Node":
                            newNode = new Node();
                            break;
                        case "Shelf":
                            newNode = new Shelf();
                            break;
                        default:
                            newNode = new Node();
                            break;
                    }
                    newNode.Id = int.Parse(line[0]);
                    string[] pos = line[2].Split(' ');
                    newNode.X = float.Parse(pos[0], NumberStyles.Any, c);
                    newNode.Y = float.Parse(pos[1], NumberStyles.Any, c);

                    nodes.Add(newNode);
                }
                catch { }
            }
            foreach (string[] line in lines)
            {
                try
                {
                    int id = int.Parse(line[0]);
                    string[] neighbours = line[3].Split(' ');

                    List<Node> neighbourNodes = new List<Node>();
                    foreach (string neighbour in neighbours)
                    {
                        try
                        {
                            neighbourNodes.Add(nodes.Find(n => n.Id == int.Parse(neighbour)));
                        }
                        catch { }
                    }

                    Node node = nodes.Find(n => n.Id == id);
                    node.Edges = neighbourNodes.Select(n => new Edge { from = node, to = n, weight = -1 }).ToArray();
                }
                catch { }
            }

            _nodes = nodes.ToArray();

            foreach (Node node in _nodes)
            {
                foreach (Edge edge in node.Edges)
                {
                    edge.weight = (float)Math.Sqrt(Math.Pow(edge.from.X - edge.to.X, 2) + Math.Pow(edge.from.Y - edge.to.Y, 2));
                }
            }

            CreateSubNetwork();
        }

        private void CreateSubNetwork()
        {
            _subNetwork = new Network<Shelf>(_nodes, n => n is Shelf, s => new NetworkNode<Shelf>((Shelf)s));
        }

        public void AddBook(Item item)
        {
            Network<FilteredShelf> filterNetwork = new Network<FilteredShelf>(_nodes, n => n is Shelf,
                s => new NetworkNode<FilteredShelf>(new FilteredShelf((Shelf)s, item)));
            Node currentNode = filterNetwork.Dropoff;

            if (!ItemDatabase.Items.Contains(item))
            {
                ItemDatabase.AddBook(item);
                _cache = new WeightCache(ItemDatabase.Items.Power().ToArray());
            }
            Item[][] itemSets = ItemDatabase.Items.Power().Where(i => i.Contains(item)).ToArray();
            float lowestEvaluation = float.MaxValue;
            bool cont = true;

            while (cont)
            {
                cont = false;

                FilteredShelf[] neighbours = currentNode.Neighbours.Where(n => n is FilteredShelf)
                    .Cast<FilteredShelf>().Where(n => n.Capacity > 0).Take(5).ToArray();

                foreach (FilteredShelf neighbour in neighbours)
                {
                    neighbour.AddFilteredItem = true;
                    float eval = EvaluationFunction(filterNetwork, itemSets);
                    neighbour.AddFilteredItem = false;

                    if (eval < lowestEvaluation)
                    {
                        currentNode = neighbour;
                        lowestEvaluation = eval;
                        cont = true;
                    }
                }
            }

            ((FilteredShelf)currentNode).Parent.AddBook(item);

            _cache.MarkItem(item);
        }

        private float EvaluationFunction<T>(Network<T> network, Item[][] itemSets) where T : Node
        {
            WeightCache cache = new WeightCache(itemSets);

            float result = 0;

            foreach (Item[] set in itemSets.OrderByDescending(set => set.Length))
            {
                result += EvaluateSet(network, cache, set);
            }

            return result;
        }

        private float EvaluateSet<T>(Network<T> network, WeightCache cache, Item[] itemSet) where T : Node
        {
            float importance = Algorithms.Importance(itemSet);

            if (importance <= 0)
            {
                return 0;
            }

            return importance * Algorithms.Weight(network.AllNodes, cache, itemSet);
        }
    }
}