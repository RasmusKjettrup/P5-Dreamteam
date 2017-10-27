using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseAI
{
    public static class WarehouseIO
    {
        /// <summary>
        /// Loads the Item database from a file.
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <returns>returns a list of items</returns>
        public static List<Item> LoadAllItemsFromFile(string filePath)
        {
            List<Item> items = new List<Item>();
            string[] setOfAllItems = File.ReadAllLines(filePath);

            foreach (string s in setOfAllItems)
            {
                int commaIndex = s.IndexOf(',');
                string identifier = s.Substring(0, commaIndex);
                string name = s.Substring(commaIndex + 2);
                items.Add(new Item(identifier, name));
            }
            return items;
        }

        /// <summary>
        /// Loads the Relation database from a file.
        /// The relations are added onto the items.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="items">The list over all items.</param>
        public static void LoadAllRelationsFromFile(string filePath, List<Item> items)
        {
            string[] setOfAllRelations = File.ReadAllLines(filePath);

            foreach (string relation in setOfAllRelations)
            {
                string[] nodes = relation.Split(',').Select(s => s.Trim()).ToArray();
                var item1 = items.Find(item => item.ID == nodes[0]);
                item1.AddOutgoingRelation(items.Find(item => item.ID == nodes[1]));
            }
        }
    }
}
