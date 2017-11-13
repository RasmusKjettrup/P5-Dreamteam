using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseAI
{
    public class Network<T> where T : Node
    {
        public readonly NetworkNode<Node> Dropoff;
        public readonly NetworkNode<T>[] Nodes;
        public NetworkNode<Node>[] AllNodes => Dropoff.Append(Nodes.Select(n => n.Cast())).ToArray();

        public Network(Node[] graph, Func<Node, bool> include, Func<Node, NetworkNode<T>> conversion)
        {
            Dropoff = new NetworkNode<Node>(graph[0]);

            List<NetworkNode<T>> nodes = new List<NetworkNode<T>>();
            foreach (Node node in graph.Skip(1))
            {
                if (include(node))
                {
                    nodes.Add(conversion(node));
                }
            }
            Nodes = nodes.ToArray();

            foreach (NetworkNode<Node> i in AllNodes)
            {
                List<Edge> edges = new List<Edge>();
                foreach (NetworkNode<Node> j in AllNodes)
                {
                    Edge edge = new Edge();
                    edge.from = i;
                    edge.to = j;
                    edge.weight = 0; //TODO: Fix the weight of edges in the subnetwork
                    edges.Add(edge);
                }
                i.SetEdges(edges.ToArray());
            }
        }
    }
}