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

        public int Priority { get; set; }

        public List<Item> IngoingRelations { get; }

        public List<Item> OutgoingRelations  { get; }

        public Item(string id, string name)
        {
            ID = id;
            Name = name;
            OutgoingRelations = new List<Item>();
            IngoingRelations = new List<Item>();
        }

        public void AddOutgoingRelation(Item relatedItem)
        {
            OutgoingRelations.Add(relatedItem);
            relatedItem.AddIngoingRelations(this);
            Priority++;
        }

        public void AddIngoingRelations(Item relatedItem)
        {
            IngoingRelations.Add(relatedItem);
        }
    }
}
