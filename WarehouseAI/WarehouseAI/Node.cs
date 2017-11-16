using System.Collections.Generic;
using System.Linq;
using System;

namespace WarehouseAI
{
    public class Edge<T>
    {
        public float weight;
        public T to;
        public T from;
    }

    public class Node
    {
        public int Id;

        protected Edge<Node>[] _edges;
        #region AStar_Fields
        private float g_cost;
        private float h_cost;
        private Node cameFrom;
        #endregion AStar_Fields
        
        public float X { get; set; }
        public float Y { get; set; }

        public Node[] Neighbours
        {
            get { return _edges.Select(e => e.to).ToArray(); }
        }

        public Edge<Node>[] Edges
        {
            get { return _edges; }
            set { _edges = value; }
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

        public Edge<Node> GetEdgeToNeighbour(Node neighbour)
        {
            if (Neighbours.Contains<Node>(neighbour) != true)
            {
                throw new UnfittingNodeException("GetEdgeToNeighbour: The input node is not recognized as a neighbour");
            }
            foreach (Edge<Node> e in Edges)
            {
                if (e.to == neighbour || e.from == neighbour)
                {
                    return e;
                }
            }
            throw new UnfittingNodeException("GetEdgeToNeighbour: The input node is recognized as a neighbour, but not found as part of any edge in Node.Edges");
        }

        public float EuclidDistance(Node node)
        {
            return (float)Math.Sqrt(Math.Pow(node.X - X, 2) + Math.Pow(node.Y - Y, 2));
        }
    }

    public class UnfittingNodeException: Exception
    {
        public UnfittingNodeException(string message) : base(message)
        {
        }
    }
}