﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseAI
{
    public class Shelf : Node
    {
        public string ID;

        public int MaxItemCapacity { get; set; } = 5;

        private readonly List<Item> _items = new List<Item>();

        public virtual Item[] Items
        {
            get { return _items.ToArray(); }
        }

        /// <summary>
        /// Adds an item to the shelf.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public void AddBook(Item item)
        {
            if (Contains(item))
            {
                if(_items.Count < MaxItemCapacity)
                    _items.Add(item);
                else
                    throw new ArgumentException("Shelf " + ID + "is already full");
            }
            else
            {
                throw new ArgumentException("Shelf " + ID + " already contains a book with ID " + item.Id);
            }
        }

        /// <summary>
        /// Returns whether an item of the same ID is in the shelf
        /// </summary>
        /// <param name="items">The items to be checked.</param>
        /// <returns></returns>
        public bool Contains(params Item[] items)
        {
            foreach (Item item in items)
            {
                if (!_items.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
