using System.Collections.Generic;
using System.Linq;

namespace WarehouseAI
{/// <summary>
/// Connects the nodes
/// </summary>
    public class Edge
    {
        public float weight;
        public Node to;
    }
    /// <summary>
    /// Walkable path goes from node to node through edges
    /// </summary>
    public class Node
    {
        private Edge[] edges;

        public Node[] Neighbours
        {
            get { return edges.Select(e => e.to).ToArray(); }
        }

        public Edge[] Edges
        {
            get { return edges; }
            set { edges = value; }
        }
    }
}