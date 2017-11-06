using System.Collections.Generic;
using System.Linq;
using System;

namespace WarehouseAI
{
    public class Edge
    {
        public float weight;
        public Node to;
        public Node from;
    }

    public class Node
    {
        #region AStar_Fields
        private Edge[] edges;
        private float g_cost;
        private float h_cost;
        private Node cameFrom;
        #endregion AStar_Fields

        private int x;
        private int y;

        public int X
        {
            get { return x; }
            set { x = value; }
        }
        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public Node[] Neighbours
        {
            get { return edges.Select(e => e.to).ToArray(); }
        }

        public virtual Edge[] Edges
        {
            get { return edges; }
            set { edges = value; }
        }

        #region AStar_Props
        public float fCost
        {
            get { return g_cost + h_cost; }
        }
        public float gCost
        {
            get { return g_cost; }
            set { g_cost = value; }
        }
        public float hCost
        {
            get { return h_cost; }
            set { h_cost = value; }
        }
        public Node CameFrom
        {
            get { return cameFrom;  }
            set { cameFrom = value;  }
        }
        #endregion AStar_Props

        public Edge GetEdgeToNeighbour(Node neighbour)
        {
            if (Neighbours.Contains<Node>(neighbour) != true)
            {
                throw new UnfittingNodeException("GetEdgeToNeighbour: The input node is not recognized as a neighbour");
            }
            foreach (Edge e in Edges)
            {
                if (e.to == neighbour || e.from == neighbour)
                {
                    return e;
                }
            }
            throw new UnfittingNodeException("GetEdgeToNeighbour: The input node is recognized as a neighbour, but not found as part of any edge in Node.Edges");
        }
    }

    public class UnfittingNodeException: Exception
    {
        public UnfittingNodeException(string message) : base(message)
        {
        }
    }
}