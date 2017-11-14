using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseAI
{
    public class Network<T> where T : NetworkNode
    {
        public readonly NetworkNode Dropoff;
        public readonly T[] Nodes;
        public NetworkNode[] AllNodes => Dropoff.Append(Nodes).ToArray();

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

            foreach (NetworkNode i in AllNodes)
            {
                List<Edge<NetworkNode>> edges = new List<Edge<NetworkNode>>();
                foreach (NetworkNode j in AllNodes)
                {
                    Edge<NetworkNode> edge = new Edge<NetworkNode>();
                    edge.from = i;
                    edge.to = j;
                    edge.weight = 2; //aStar.FindPath(graph, i.Parent, j.Parent); //TODO: Fix the weight of edges in the subnetwork
                    edges.Add(edge);
                }
                i.SetEdges(edges.ToArray());
            }
        }
    }
}