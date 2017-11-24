using System.Linq;
using WarehouseAI.Representation;

namespace WarehouseAI.ShortestPathGraph
{
    public class ShelfShortestPathGraphNode : Shelf, IShortestPathGraphNode
    {
        public Node Parent { get; set; }

        public override Item[] Items
        {
            get
            {
                return ((Shelf)Parent).Items;
            }
        }

        public ShelfShortestPathGraphNode(Shelf parent)
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