using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseAI
{
    public class Program
    {
        private static void Main(string[] args)
        {
        }

        /// <summary>
        /// Loads the Item database from a file.
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <returns>returns a list of items</returns>
        private List<Item> LoadAllItemsFromFile(string filePath)
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
        /// Loads the Relation database from a file
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="items">The list over all files that are related.</param>
        /// <returns>returns a list of arcs</returns>
        private List<Arc> LoadAllRelationsFromFile(string filePath, List<Item> items)
        {
            List<Arc> arcs = new List<Arc>();
            string[] setOfAllRelations = File.ReadAllLines(filePath);

            foreach (string s in setOfAllRelations)
            {
                string[] tempString = s.Split(',');
                arcs.Add(new Arc(items.Find(item1 => item1.ID == tempString[0]), items.Find(item2 => item2.ID == tempString[1])));
            }
            return arcs;
        }

        /// <summary>
        /// Calculates the importance coefficient.
        /// </summary>
        /// <param name="setOfItems">The set of items to calculate upon.</param>
        /// <param name="relations">The list of all related items in the system.</param>
        /// <returns></returns>
        public static float ImportanceCoefficientAlgorithm(List<Item> setOfItems, List<Arc> relations)
        {
            float a = 0; // Number of Arcs
            int n = setOfItems.Count; // Number of nodes in the setOfItems
            foreach (Item item in setOfItems)
            {
                // Adds all relations where both items in the arc are in the setOfItems.
                a += relations
                    .Where(arc => arc.Item1 == item)
                    .Count(ar => setOfItems.Contains(ar.Item2));
            }
            return a / n;
        }
    }
}
