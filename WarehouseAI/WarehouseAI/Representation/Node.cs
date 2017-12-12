using System;
using System.Linq;

namespace WarehouseAI.Representation
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
        
        /// <summary>
        /// The X coordinate of the node.
        /// </summary>
        public float X { get; set; }
        /// <summary>
        /// The Y coordinate of the node.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// The neighbours of the node
        /// </summary>
        public Node[] Neighbours
        {
            get { return _edges.Select(e => e.to).ToArray(); }
        }

        /// <summary>
        /// The set of all edges from the node to another node.
        /// </summary>
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

        /// <summary>
        /// Gets an edge from this node to another, if it is contained in the set of edges.
        /// </summary>
        /// <param name="neighbour">The node to find an edge to.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Finds the distance form this node to another using Euclid's calculation of distance.
        /// </summary>
        /// <param name="node">The node to find distance to.</param>
        /// <returns></returns>
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
