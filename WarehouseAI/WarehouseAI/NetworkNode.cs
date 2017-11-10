using System.Linq;

namespace WarehouseAI
{
    public class NetworkNode<T> : Node where T: Node
    {
        public T Parent;

        public NetworkNode<Node> Cast()
        {
            return new NetworkNode<Node>(Parent);
        }

        public void SetEdges(Edge[] edges)
        {
            _edges = edges;
            _edges = _edges.OrderBy(e => e.weight).ToArray();
        }

        public NetworkNode(T parent)
        {
            Parent = parent;
        }
    }

    public class FilterShelf : NetworkNode<Shelf>
    {
        public bool AddFilteredItem = false;

        private Item _filterItem;

        public Item[] Items {
            get {
                return (AddFilteredItem
                    ? Parent.Items.Where(i => i != _filterItem).Append(_filterItem)
                    : Parent.Items.Where(i => i != _filterItem)).ToArray();
            }
        }

        public FilterShelf(Shelf parent, Item filterItem) : base(parent)
        {
            _filterItem = filterItem;
        }
    }
}