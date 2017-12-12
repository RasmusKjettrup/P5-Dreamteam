using System.Linq;
using WarehouseAI.Representation;

namespace WarehouseAI.ShortestPathGraph
{
    public class ShortestPathGraphNode : Node, IShortestPathGraphNode
    {
        /// <summary>
        /// The parent node that this is an abstraction on.
        /// </summary>
        public Node Parent { get; set; }


        public ShortestPathGraphNode(Node parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Set the edges of the node.
        /// </summary>
        /// <param name="edges"></param>
        public void SetEdges(Edge<Node>[] edges)
        {
            _edges = edges;
            _edges = _edges.OrderBy(e => e.weight).ToArray();
        }
    }
}