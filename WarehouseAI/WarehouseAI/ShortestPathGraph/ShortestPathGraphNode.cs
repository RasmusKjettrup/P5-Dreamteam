using System.Linq;
using WarehouseAI.Representation;

namespace WarehouseAI.ShortestPathGraph
{
    public class ShortestPathGraphNode : Node, IShortestPathGraphNode
    {
        public Node Parent { get; set; }

        public ShortestPathGraphNode(Node parent)
        {
            Parent = parent;
        }

        public void SetEdges(Edge<Node>[] edges)
        {
            _edges = edges;
            _edges = _edges.OrderBy(e => e.weight).ToArray();
        }
    }
}