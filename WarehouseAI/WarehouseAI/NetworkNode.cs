using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace WarehouseAI
{
    public class NetworkNode<T> : Node where T: Node
    {
        public T Parent;

        public NetworkNode(T parent)
        {
            Parent = parent;
        }

        public NetworkNode<Node> Cast()
        {
            return new NetworkNode<Node>(Parent);
        }

        public void SetEdges(Edge[] edges)
        {
            _edges = edges;
            _edges = _edges.OrderBy(e => e.weight).ToArray();
        }
    }

    public class FilteredShelf : Shelf
    {
        public Shelf Parent;
        public bool AddFilteredItem = false;
        public int Capacity = 5;

        private Item _filterItem;
        
        public new Item[] Items {
            get {
                return (AddFilteredItem
                    ? Parent.Items.Where(i => i != _filterItem).Append(_filterItem)
                    : Parent.Items.Where(i => i != _filterItem)).ToArray();
            }
        }

        public FilteredShelf(Shelf parent, Item filterItem)
        {
            _filterItem = filterItem;
            Parent = parent;
        }
    }
}