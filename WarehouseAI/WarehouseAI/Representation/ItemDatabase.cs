using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WarehouseAI.Representation
{
    public class ItemDatabase
    {
        private List<Item> _items;

        /// <summary>
        /// All items that has been added to the 
        /// </summary>
        public Item[] Items => _items.ToArray();

        public ItemDatabase()
        {
            _items = new List<Item>();
        }

        /// <summary>
        /// Loads the Item database from a file.
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        public void ImportItems(string filePath)
        {
            List<Item> items = new List<Item>();
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                try
                {
                    int commaIndex = line.IndexOf(',');
                    int id = int.Parse(line.Substring(0, commaIndex));
                    string name = line.Substring(commaIndex + 2);
                    items.Add(new Item(id, name));
                }
                catch { }
            }
            _items = items;
        }

        /// <summary>
        /// Loads the Relation database from a file, and sets the relations of the items in the database.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        public void ImportRelations(string filePath)
        {
            string[] setOfAllRelations = File.ReadAllLines(filePath);

            foreach (string relation in setOfAllRelations)
            {
                string[] nodes = relation.Split(',').Select(s => s.Trim()).ToArray();

                try
                {
                    Item item = _items.Find(i => i.Id == int.Parse(nodes[0]));
                    item.AddOutgoingRelation(_items.Find(i => i.Id == int.Parse(nodes[1])));
                }
                catch { }
            }
        }

        /// <summary>
        /// Adds a book to the database of books.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddBook(Item item)
        {
            _items.Add(item);
        }
    }
}