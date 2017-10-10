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
            StringBuilder sb = new StringBuilder();
            string[] setOfAllItems = File.ReadAllLines(filePath);

            foreach (string s in setOfAllItems)
            {
                string[] tempString = s.Split(',');
                // if a name contains "," then it puts them together to form one string.
                for (int i = 1; i < tempString.Length; i++)
                {
                    sb.Append(tempString[i]);
                }
                items.Add(new Item(tempString[0], tempString[1]));
                sb.Clear();
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

            foreach (string s in setOfAllRelations)
            {
                string[] tempString = s.Split(',');
                var item1 = items.Find(item => item.ID == tempString[0]);
                item1.AddRelation(items.Find(item => item.ID == tempString[1]));
            }
        }
    }
}
