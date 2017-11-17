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
        public int Id { get; set; }
        /// <summary>
        /// The name of the item.
        /// </summary>
        public string Name { get; set; }

        public int Priority { get; set; }

        private List<Item> IngoingRelations { get; }

        private List<Item> OutgoingRelations  { get; }

        public Item(int id, string name)
        {
            Id = id;
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

        public Item[] Neighbours()
        {
            return OutgoingRelations.ToArray();
        }

    }
}
