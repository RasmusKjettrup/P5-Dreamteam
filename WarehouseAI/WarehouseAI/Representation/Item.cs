using System.Collections.Generic;

namespace WarehouseAI.Representation
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

        /// <summary>
        /// Placement priority of the item, used when placing an array of books.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// All items that has a relation to this item.
        /// </summary>
        private List<Item> IngoingRelations { get; }
        /// <summary>
        /// All items that this item relates to.
        /// </summary>
        private List<Item> OutgoingRelations  { get; }

        public Item(int id, string name)
        {
            Id = id;
            Name = name;
            OutgoingRelations = new List<Item>();
            IngoingRelations = new List<Item>();
        }

        /// <summary>
        /// Adds an outgoing relation to this item, and an ingoing relation to the related item.
        /// </summary>
        /// <param name="relatedItem">The item to make a relation to.</param>
        public void AddOutgoingRelation(Item relatedItem)
        {
            OutgoingRelations.Add(relatedItem);
            relatedItem.AddIngoingRelations(this);
            Priority++;
        }

        private void AddIngoingRelations(Item relatedItem)
        {
            IngoingRelations.Add(relatedItem);
        }

        /// <summary>
        /// The set of all items that this item has a relation to.
        /// </summary>
        /// <returns></returns>
        public Item[] Neighbours()
        {
            return OutgoingRelations.ToArray();
        }
    }
}
