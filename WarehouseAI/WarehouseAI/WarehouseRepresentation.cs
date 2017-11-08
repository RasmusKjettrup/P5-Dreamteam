using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace WarehouseAI
{
    public class WarehouseRepresentation
    {
        private Node[] _nodes;

        public Node[] Nodes => _nodes;

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
                    node.Edges = neighbourNodes.Select(n => new Edge{ from = node, to = n, weight = -1 }).ToArray();
                }
                catch { }
            }

            _nodes = nodes.ToArray();

            foreach (Node node in _nodes)
            {
                foreach (Edge edge in node.Edges)
                {
                    edge.weight = (float) Math.Sqrt(Math.Pow(edge.from.X - edge.to.X, 2) + Math.Pow(edge.from.Y - edge.to.Y, 2));
                }
            }
        }
    }
}