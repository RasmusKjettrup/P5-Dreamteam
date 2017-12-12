using System.Linq;
using WarehouseAI.Representation;

namespace WarehouseAI.ShortestPathGraph
{
    public class FilteredShelfShortestPathGraphNode : ShelfShortestPathGraphNode
    {
        /// <summary>
        /// Add the filtered item to the shelf
        /// </summary>
        public bool AddFilteredItem = false;
        /// <summary>
        /// The remaning capacity of the abstraction on the shelf.
        /// </summary>
        public int Capacity;

        private Item _filterItem;
        
        /// <summary>
        /// Override the parent's set of items to create an abstraction on the shelf. 
        /// </summary>
        public override Item[] Items {
            get {
                return (AddFilteredItem
                    ? ((Shelf)Parent).Items.Where(i => i != _filterItem).Append(_filterItem)
                    : ((Shelf)Parent).Items.Where(i => i != _filterItem)).ToArray();
            }
        }

        public FilteredShelfShortestPathGraphNode(Shelf parent, Item filterItem) : base(parent)
        {
            _filterItem = filterItem;
            Capacity = parent.RemaningCapacity;
        }
    }
}