using System.Linq;

namespace WarehouseAI
{
    public class ShelfNetworkNode : Shelf, INetworkNode
    {
        public Node Parent { get; set; }

        public void SetEdges(Edge<Node>[] edges)
        {
            _edges = edges;
            _edges = _edges.OrderBy(e => e.weight).ToArray();
        }

        public ShelfNetworkNode(Shelf parent)
        {
            Parent = parent;
        }
    }
}