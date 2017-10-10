using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseAI
{
    public class Item
    {
        /// <summary>
        /// The ID of the item.
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// The name of the item.
        /// </summary>
        public string Name { get; set; }

        public List<Item> Relations  { get; }

        public Item(string id, string name)
        {
            ID = id;
            Name = name;
            Relations = new List<Item>();
        }

        public void AddRelation(Item relatedItem)
        {
            Relations.Add(relatedItem);
        }
    }
}
