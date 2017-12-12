using System;
using System.Collections.Generic;
using System.Linq;
using WarehouseAI.Pathfinding;
using WarehouseAI.Representation;

namespace WarehouseAI.ShortestPathGraph
{
    public class ShortestPathGraph<T> where T : IShortestPathGraphNode
    {
        /// <summary>
        /// The dropoff point of the graph.
        /// </summary>
        public readonly IShortestPathGraphNode Dropoff;
        /// <summary>
        /// All nodes in the graph, other than the dropoff node.
        /// </summary>
        public readonly T[] Nodes;
        /// <summary>
        /// All nodes of the graph.
        /// </summary>
        public IShortestPathGraphNode[] AllNodes => Dropoff.Append(Nodes.Cast<IShortestPathGraphNode>()).ToArray();

        /// <summary>
        /// ShortestPathGraph is an all-pairs shortest path graph between all nodes included, and the dropoff point.
        /// </summary>
        /// <param name="graph">The base graph on which to make an APSPG on</param>
        /// <param name="include">Prerequisite that each node in the graph should pass. All nodes that does
        /// not pass this prerequisite is not included in the APSPG</param>
        /// <param name="conversion">The function that converts a node into a network node.</param>
        public ShortestPathGraph(Node[] graph, Func<Node, bool> include, Func<Node, T> conversion)
        {
            //The dropoff point.
            Dropoff = new ShortestPathGraphNode(graph[0]);
            
            List<T> nodes = new List<T>();
            //Add all nodes that pass the prerequisite, skipping the first (dropoff) point.
            foreach (Node node in graph.Skip(1))
            {
                //If the nodes passes the prerequisite...
                if (include(node))
                {
                    //Convert it to an appropriate format, and add the new node to "nodes".
                    nodes.Add(conversion(node));
                }
            }
            Nodes = nodes.ToArray();

            DistanceMap map = new DistanceMap(AllNodes.Select(n => n.Parent).ToArray());

            int id = 0;

            //Set the distance between the new nodes, and the new edges between them.
            foreach (IShortestPathGraphNode i in AllNodes)
            {
                ((Node) i).Id = i.Parent.Id;
                List<Edge<Node>> edges = new List<Edge<Node>>();
                foreach (IShortestPathGraphNode j in AllNodes)
                {
                    //Create a new node between i and j.
                    Edge<Node> edge = new Edge<Node>();
                    edge.from = (Node)i;
                    edge.to = (Node)j;
                    //The weight between them is the distance between the old nodes.
                    map.TryGet(i.Parent.Id, j.Parent.Id, out edge.weight); //TODO: Fix the weight of edges in the subnetwork
                    edges.Add(edge);
                }
                //Set the new edges.
                i.SetEdges(edges.OrderBy(e => e.weight).ToArray());
            }
        }
    }
}