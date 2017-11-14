using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseAI
{
    public class Network<T> where T : INetworkNode
    {
        public readonly INetworkNode Dropoff;
        public readonly T[] Nodes;
        public INetworkNode[] AllNodes => Dropoff.Append(Nodes.Cast<INetworkNode>()).ToArray();

        public Network(Node[] graph, Func<Node, bool> include, Func<Node, T> conversion)
        {
            Dropoff = new NetworkNode(graph[0]);

            List<T> nodes = new List<T>();
            foreach (Node node in graph.Skip(1))
            {
                if (include(node))
                {
                    nodes.Add(conversion(node));
                }
            }
            Nodes = nodes.ToArray();

            AStarAlgorithm aStar = new AStarAlgorithm();

            foreach (INetworkNode i in AllNodes)
            {
                List<Edge<Node>> edges = new List<Edge<Node>>();
                foreach (INetworkNode j in AllNodes)
                {
                    Edge<Node> edge = new Edge<Node>();
                    edge.from = (Node)i;
                    edge.to = (Node)j;
                    edge.weight = 2; //aStar.FindPath(graph, i.Parent, j.Parent); //TODO: Fix the weight of edges in the subnetwork
                    edges.Add(edge);
                }
                i.SetEdges(edges.ToArray());
            }
        }
    }
}