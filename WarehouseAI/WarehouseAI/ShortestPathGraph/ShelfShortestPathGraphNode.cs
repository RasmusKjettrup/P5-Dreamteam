using System.Linq;
using WarehouseAI.Representation;

namespace WarehouseAI.ShortestPathGraph
{
    public class ShelfShortestPathGraphNode : Shelf, IShortestPathGraphNode
    {
        /// <summary>
        /// The parent of the abstraction on a shelf.
        /// </summary>
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

        /// <summary>
        /// Set the edges of the abstraction of the shelf. Also orders them by weight.
        /// </summary>
        /// <param name="edges">All new edges.</param>
        public void SetEdges(Edge<Node>[] edges)
        {
            _edges = edges;
            _edges = _edges.OrderBy(e => e.weight).ToArray();
        }
    }
}