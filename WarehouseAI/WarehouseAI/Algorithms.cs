using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseAI.Network;

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

        /// <summary>
        /// Calculates and returns the weight of collecting a set of items. The calculation is done on the graph of nodes
        /// "graph", which is assumed to contain shelves that contain the items in the item set.
        /// </summary>
        /// <param name="itemSet">The items to collect.</param>
        /// <returns></returns>
        public static float Weight(Item[] itemSet)
        {
            return Weight(_minimalNetwork.AllNodes.Cast<Node>().ToArray(), itemSet);
        }

        /// <summary>
        /// Calculates and returns the weight of collecting a set of items. The calculation is done on the graph of nodes
        /// "graph", which is assumed to contain shelves that contain the items in the item set.
        /// </summary>
        /// <param name="graph">The warehouse representation.</param>
        /// <param name="cache">A cache of item weights that helps with optimization when many weights are calculated in
        /// bulk.</param>
        /// <param name="itemSet">The items to collect.</param>
        /// <param name="distanceMap">The distance map. Give as parameter to prevent calculating it every time
        /// weight is run. </param>
        /// <returns></returns>
        public static float Weight(Node[] graph, Item[] itemSet, WeightCache cache = null, DistanceMap distanceMap = null)
        {
            Node[] dummy;
            return Weight(graph, itemSet, out dummy, cache, distanceMap);
        }

        /// <summary>
        /// Calculates and returns the weight of collecting a set of items. The calculation is done on the graph of nodes
        /// "graph", which is assumed to contain shelves that contain the items in the item set.
        /// </summary>
        /// <param name="graph">The warehouse representation.</param>
        /// <param name="cache">A cache of item weights that helps with optimization when many weights are calculated in
        /// bulk.</param>
        /// <param name="itemSet">The items to collect.</param>
        /// <param name="path">The path that an agent should take when collecting the items in the set.</param>
        /// <param name="distanceMap">The distance map. Give as parameter to prevent calculating it every time
        /// weight is run. </param>
        /// <returns></returns>
        public static float Weight(Node[] graph, Item[] itemSet, out Node[] path, WeightCache cache = null, DistanceMap distanceMap = null)
        {
            path = null;
            if (distanceMap == null)
            {
                distanceMap = new DistanceMap(graph);
            }
            if (cache == null)
            {
                cache = _cache;
            }

            //frontiers describe the paths currently getting examined.
            List<Frontier> frontiers = new List<Frontier>();
            //graph[0] is the dropoff point in the graph.
            Node dropoff = graph[0];
            //Initially, the first fronter is selected, from the input graph. graph[0] is the dropoff point.
            //The "books" field in Frontiers are the books that still need to be collected on the trip.
            frontiers.Add(new Frontier(new[] { dropoff }, itemSet, 0));

            //While the cache element that we are interested in are marked for updating, keep exploring frontiers.
            while (cache[itemSet].Marked)
            {
                //The resultingfrontier is the result of this specific iteration of the while loop.
                Frontier resultingFrontier = new Frontier(null, null, int.MaxValue);
                //Loop through each frontier in frontiers
                foreach (Frontier frontier in frontiers)
                {
                    //Find the last node of the route of the froontier currently getting examined.
                    Node lastNode = frontier.route.Last();

                    //Foreach node neighbour of the last node, that is a shelf, and contains at least 
                    //one book that we are interested in...
                    foreach (Shelf neighbour in lastNode.Neighbours.Where(n => n is Shelf).Cast<Shelf>()
                        .Where(s => s.Contains(frontier.books)))
                    {
                        //Find the distance between the neighbour and the last node.
                        float distance = Distance(distanceMap, lastNode, neighbour);
                        //If the route that the current frontiers route + the neighbour has not been explored already,
                        //and it is more efficient than the current resultingfrontier...
                        if (NotExplored(frontiers.ToArray(), frontier.route.Append(neighbour).ToArray())
                            && frontier.weight + distance < resultingFrontier.weight)
                        {
                            //Set the resultingfrontier to be a new frontier created form the current frontier.
                            //The route is the old route with the neighbour appended, the array of books are the
                            //old array of books with the neighbour's books subtracted, and the weight
                            //is the old weight plus the distance.
                            resultingFrontier = new Frontier(frontier.route.Append(neighbour).ToArray(),
                                frontier.books.Where(i => !neighbour.Contains(i)).ToArray(),
                                frontier.weight + distance);
                        }
                    }

                    CacheElement c;
                    //If the cache contains the current set of collected books, and is marked, the weight in the cache
                    //need to be updated.
                    if (cache.TryGet(itemSet.Except(frontier.books).ToArray(), out c) && c.Marked)
                    {
                        //The distance between the last node and the dropoff node is found...
                        float distance = Distance(distanceMap, lastNode, dropoff);
                        //And used to evaluate the weight of the resultingfrontier
                        if (frontier.weight + distance <= resultingFrontier.weight)
                        {
                            //If it is the most efficient, create a new frontier that routes through each book,
                            //and moves back to the dropoff point.
                            resultingFrontier = new Frontier(frontier.route.Append(dropoff).ToArray(),
                                frontier.books,
                                frontier.weight + distance);
                        }
                    }
                }
                //Add the current resultingfrontier to the set of frontiers, to evaluate in the next iteration.
                frontiers.Add(resultingFrontier);
                //If the last node in the resultingfrontier is the dropoff point then we know it is a full cycle and
                //should update the element in the cache that represents the set of items.
                if (resultingFrontier.route.Last() == dropoff)
                {
                    CacheElement c = cache[itemSet.Except(resultingFrontier.books).ToArray()];
                    c.Marked = false;
                    c.Weight = resultingFrontier.weight;
                    path = resultingFrontier.route;
                }
            }

            //Whenever the cache element for the requested itemset is updated, the weight of the cache element is returned.
            return cache[itemSet].Weight;
        }

        /// <summary>
        /// Checks wether a potential set of nodes have already been added to a route in the set of frontiers or not.
        /// </summary>
        /// <param name="frontiers"></param>
        /// <param name="potentialNodes"></param>
        /// <returns></returns>
        private static bool NotExplored(Frontier[] frontiers, Node[] potentialNodes)
        {
            foreach (Frontier frontier in frontiers)
            {
                if (frontier.route.Length != potentialNodes.Length)
                {
                    continue;
                }
                for (int i = 0; i < frontier.route.Length; i++)
                {
                    if (potentialNodes[i] != frontier.route[i])
                    {
                        break;
                    }
                    if (i == frontier.route.Length-1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Uses a DistanceMap to find the distance between two nodes, and returns the result.
        /// </summary>
        /// <param name="distanceMap"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private static float Distance(DistanceMap distanceMap, Node from, Node to)
        {
            float f;
            distanceMap.TryGet(from.Id, to.Id, out f);
            return f;
        }
    }
}

