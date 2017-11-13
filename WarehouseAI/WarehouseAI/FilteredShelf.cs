using System.Linq;

namespace WarehouseAI
{
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

        public override Node[] Neighbours => Parent.Neighbours;

        public FilteredShelf(Shelf parent, Item filterItem)
        {
            _filterItem = filterItem;
            Parent = parent;
        }
    }
}