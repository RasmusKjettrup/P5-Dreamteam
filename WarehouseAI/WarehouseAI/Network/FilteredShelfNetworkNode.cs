using System.Linq;

namespace WarehouseAI
{
    public class FilteredShelfNetworkNode : ShelfNetworkNode
    {
        public bool AddFilteredItem = false;
        public int Capacity;

        private Item _filterItem;
        
        public override Item[] Items {
            get {
                return (AddFilteredItem
                    ? ((Shelf)Parent).Items.Where(i => i != _filterItem).Append(_filterItem)
                    : ((Shelf)Parent).Items.Where(i => i != _filterItem)).ToArray();
            }
        }

        public FilteredShelfNetworkNode(Shelf parent, Item filterItem) : base(parent)
        {
            _filterItem = filterItem;
            Capacity = parent.RemaningCapacity;
        }
    }
}