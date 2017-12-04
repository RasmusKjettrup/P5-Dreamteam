using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseAI.Representation
{
    public class Shelf : Node
    {
        protected class ItemInstance
        {
            public Item item;
            public int instances;
        }

        public int MaxCapacity { get; } = 5;

        public int RemaningCapacity {
            get {
                int capacity = MaxCapacity;
                foreach (ItemInstance instance in _itemInstances)
                {
                    capacity -= instance.instances;
                }
                return capacity;
            }
        }

        public void RemoveBook(Item book)
        {
            ItemInstance instance = _itemInstances.Find(i => i.item.Equals(book));
            if (instance != null && instance.instances > 1)
                instance.instances--;
            else
                throw new ArgumentException($"Shelf {Id} did not contain an instance of {book.Id}");
        }

        protected List<ItemInstance> _itemInstances = new List<ItemInstance>();

        public virtual Item[] Items {
            get { return _itemInstances.Select(i => i.item).ToArray(); }
        }

        public virtual int GetNumberOfItem(Item item)
        {
            if (!_itemInstances.Select(i => i.item).Contains(item))
                return 0;
            return _itemInstances.First(i => i.item == item).instances;
        }

        /// <summary>
        /// Adds an item to the shelf.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public void AddBook(Item item)
        {
            int books = 0;
            foreach (ItemInstance instance in _itemInstances)
            {
                books += instance.instances;
            }
            if (books >= MaxCapacity)
            {
                throw new ArgumentException("Shelf " + Id + "is already full");
            }

            if (Contains(item))
            {
                _itemInstances.Find(i => i.item == item).instances++;
            }
            else
            {
                _itemInstances.Add(new ItemInstance()
                {
                    item = item,
                    instances = 1,
                });
            }
        }

        /// <summary>
        /// Returns true if any of the items in "items" are already on the shelf
        /// </summary>
        /// <param name="items">The items to be checked.</param>
        /// <returns></returns>
        public bool Contains(params Item[] items)
        {
            foreach (Item item in items)
            {
                if (Items.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
