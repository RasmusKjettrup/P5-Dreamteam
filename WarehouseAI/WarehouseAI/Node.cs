using System.Collections.Generic;
using System.Linq;

namespace WarehouseAI
{
    public class Edge
    {
        public float weight;
        public Node to;
    }

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