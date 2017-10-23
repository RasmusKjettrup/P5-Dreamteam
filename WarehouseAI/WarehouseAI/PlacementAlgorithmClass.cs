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
                setOfItems = SortByPriority(setOfItems, item);
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

        /// <summary>
        /// Removes an item from a given set and sorts the set by highest priority.
        /// </summary>
        /// <param name="setOfItems">The set of items to be sorted</param>
        /// <param name="lastPlacedItem">The most recently placed item that is to be removed.</param>
        /// <returns></returns>
        public static List<Item> SortByPriority(List<Item> setOfItems, Item lastPlacedItem)
        {
            if (!setOfItems.Contains(lastPlacedItem))
                throw new ArgumentException("The set of items didn't contain the last placed item.");
            setOfItems.Remove(lastPlacedItem);
            List<Item> items = setOfItems.OrderBy(i => i.Priority).Reverse().ToList();
            return items;
        }

        public static void AddBook(List<Shelf> shelves, Item item)
        {

        }

        public static float Weightcalculation(List<Item> i)
        {
            float TotalWeight=0;
            List<Frontier> ListOfFrontiers =new List<Frontier>();
            List<Item> ExploredEdges;
            Frontier ResultingFrontier;
            Item ResultingEdge;

            while (true)
            {
                ResultingFrontier = new Frontier(null,null,int.MaxValue);
                ResultingEdge = null;
                foreach (var frontier in ListOfFrontiers)
                {
                   Item fl = frontier.route.Last();
                    if (frontier.books.Count<1) 
                    {
                        foreach (var neighbour in fl.Neighbours())
                        {
                            
                        }   
                    }
                }
                
            }



            return TotalWeight;
        }

    }
}