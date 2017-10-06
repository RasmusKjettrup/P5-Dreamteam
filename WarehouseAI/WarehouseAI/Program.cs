using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseAI
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        public List<Item> LoadAllItemsFromFile(string filePath)
        {
            List<Item> items = new List<Item>();
            StringBuilder sb = new StringBuilder();
            string[] k = File.ReadAllLines(filePath);

            foreach (string s in k)
            {
                string[] tempString = s.Split(',');
                for (int i = 1; i < tempString.Length; i++)
                {
                    sb.Append(tempString[i]);
                }
                items.Add(new Item(tempString[0], tempString[1]));
                sb.Clear();
            }
            return items;
        }

        public List<Arc> LoadAllRelationsFromFile(string filePath, List<Item> items)
        {
            List<Arc> arcs = new List<Arc>();
            string[] k = File.ReadAllLines(filePath);

            foreach (string s in k)
            {
                string[] tempString = s.Split(',');

                arcs.Add(new Arc(items.Find(item1 => item1.ID == tempString[0]), items.Find(item2 => item2.ID == tempString[1])));
            }
            return arcs;
        }

        public float ImportanceCoefficientAlgorithm(List<Item> relation, List<Arc> arcs)
        {
            float a = 0; // Number of Arcs
            int n = relation.Count; // Number of nodes in the relation
            foreach (Item item in relation)
            {
                a += arcs.Where(arc => arc.Item1 == item).Count(ar => relation.Contains(ar.Item2));
            }
            return a / n;
        }
    }
}
