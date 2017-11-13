using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseAI
{
    public static class Algorithms
    {
        private static Network<ShelfNetworkNode> _minimalNetwork;
        private static WeightCache _cache;

        /// <summary>
        /// Calculates the importance coefficient.
        /// </summary>
        /// <param name="setOfItems">The set of items to calculate upon.</param>
        /// <returns></returns>
        public static float Importance(Item[] setOfItems)
        {
            float a = 0; // Number of Arcs
            if (setOfItems == null || setOfItems.Length == 0)
                throw new ArgumentException("setOfItems Was empty");
            int n = setOfItems.Length; // Number of nodes in the setOfItems
            foreach (Item item in setOfItems)
            {
                // Adds the number of relations from each item to any other items in setOfItems to the number of arcs
                a += item.Neighbours().Count(setOfItems.Contains);
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
            foreach (Item outgoingRelation in item.Neighbours())
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

        public static void InitializeWeight(params Node[] nodes)
        {
            _minimalNetwork = new Network<ShelfNetworkNode>(nodes, n => n is Shelf, n => new ShelfNetworkNode((Shelf)n));
        }

        public static void InitializeCache(ItemDatabase itemDatabase)
        {
            _cache = new WeightCache(itemDatabase.Items.Power().ToArray());
        }

        public static float Weight(Item[] itemSet)
        {
            return Weight(_minimalNetwork.AllNodes.Select(n => n.Parent).ToArray(), _cache, itemSet);
        }

        public static float Weight(Node[] graph, WeightCache cache, Item[] itemSet)
        {
            List<Frontier> frontiers = new List<Frontier>();
            frontiers.Add(new Frontier(new[] { graph[0] }, itemSet, 0));
            Node dropoff = graph[0];

            while (cache[itemSet].Marked)
            {
                Frontier resultingFrontier = new Frontier(null, null, int.MaxValue);
                foreach (Frontier frontier in frontiers)
                {
                    Node lastNode = frontier.route.Last();
                    foreach (Shelf neighbour in lastNode.Neighbours.Where(n => n is Shelf).Cast<Shelf>()
                        .Where(s => s.Contains(frontier.books)))
                    {
                        if (!frontiers.Select(f => f.route).Contains(frontier.route.Append(neighbour))
                            && frontier.weight/*+dist(lastNode, neighbour)*/< resultingFrontier.weight)
                        {
                            resultingFrontier = new Frontier(frontier.route.Append(neighbour).ToArray(),
                                frontier.books.Where(i => !neighbour.Contains(i)).ToArray(),
                                frontier.weight/*+dist(lastNode, neighbour)*/);
                        }
                    }
                    CacheElement c;
                    if (cache.TryGet(itemSet.Except(frontier.books).ToArray(), out c) && c.Marked)
                    {
                        if (frontier.weight /*+dist(lastNode,dropoff)*/ < resultingFrontier.weight)
                        {
                            resultingFrontier = new Frontier(frontier.route.Append(dropoff).ToArray(),
                                frontier.books,
                                frontier.weight/*+dist(lastNode,dropoff)*/);
                        }
                    }
                }
                frontiers.Add(resultingFrontier);
                if (resultingFrontier.route.Last() == dropoff)
                {
                    CacheElement c = cache[itemSet.Except(resultingFrontier.books).ToArray()];
                    c.Marked = false;
                    c.Weight = resultingFrontier.weight;
                }
            }

            return cache[itemSet].Weight;
        }
    }
}

