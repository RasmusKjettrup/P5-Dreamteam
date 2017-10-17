using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseAI
{
    public class PlacementAlgorithmClass
    {
        /// <summary>
        /// Calculates the importance coefficient.
        /// </summary>
        /// <param name="setOfItems">The set of items to calculate upon.</param>
        /// <returns></returns>
        public static float ImportanceCoefficientAlgorithm(List<Item> setOfItems)
        {
            float a = 0; // Number of Arcs
            if (setOfItems == null || setOfItems.Count == 0)
                throw new ArgumentException("setOfItems Was empty");
            int n = setOfItems.Count; // Number of nodes in the setOfItems
            foreach (Item item in setOfItems)
            {
                // Adds the number of relations from each item to any other items in setOfItems to the number of arcs
                a += item.OutgoingRelations.Count(setOfItems.Contains);
            }
            return a / n;
        }


        public static void PlacementAlgorithm(List<Item> setOfItems)
        {
            foreach (Item item in setOfItems)
            {
                // Place Book | AddBook
                UpdatePriorities(item);
                // setOfItems.Sort by priority
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reduces the priority of given item by a constant k, and enhances priority on al of the items' relations.
        /// </summary>
        /// <param name="item">The most recently placed item.</param>
        public static void UpdatePriorities(Item item)
        {
            const int k = 1;
            item.Priority -= k;
            foreach (Item outgoingRelation in item.OutgoingRelations)
            {
                outgoingRelation.Priority++;
            }
        }
    }
}
