using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseAI
{
    public static class Algorithms
    {
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
        
        private static Network<Shelf> _minimalNetwork;
        private static WeightCache _cache;

        //public static void AddBook(Item item)
        //{
        //    if (allItems == null)
        //    {
        //        allItems = new List<Item>();
        //    }
        //    allItems.Add(item);

        //    //Create a subnetwork, where each node except the dropoff point is a new FilteredShelf node created from the original node
        //    Node[] subNetwork = _minimalNetwork[0].Append(_minimalNetwork.Skip(1).Select(n => new FilterShelf((Shelf)n, item))).ToArray();
        //    //The first node of the greedy descent algorithm
        //    Node currentNode = subNetwork[0];
        //    //All sets in the powerset of all items, where "item" is represented.
        //    Item[][] itemSets = allItems.Power().Where(items => items.Contains(item)).ToArray();
        //    //The lowest evaluation score between all neighbours are initialized to the max value of float
        //    float lowestEvaluation = float.MaxValue;
        //    bool cont = true;

        //    //While cont is still being set to true, new neighbours are still being found
        //    while (cont)
        //    {
        //        cont = false;

        //        //The 5 first neighbours of the current node, casted to a FilteredShelf, where there is still space for new books.
        //        //The neighbours are sorted with lowest weight first, so te first 5 is the 5 closest.
        //        FilterShelf[] neighbours = currentNode.Neighbours.Where(node => node is FilterShelf).Cast<FilterShelf>()
        //            .Where(shelf => shelf.Capacity != 0).Take(5).ToArray();

        //        //Check through each neighbour to detect better candidates to place the book in.
        //        foreach (FilterShelf shelf in neighbours)
        //        {
        //            //First set the filterShelf to contain the new book...
        //            shelf.ContainsFilteredBook = true;
        //            //Then evaluate the placement of the new book.
        //            float eval = EvaluationFunction(subNetwork, itemSets);
        //            //Unequip the book from the shelf again.
        //            shelf.ContainsFilteredBook = false;

        //            //Set the new currentNode, if appropriate
        //            if (eval < lowestEvaluation)
        //            {
        //                currentNode = shelf;
        //                lowestEvaluation = eval;
        //                cont = true;
        //            }
        //        }
        //    }

        //    //the new book is added to the appropriate node
        //    ((FilterShelf)currentNode).Parent.AddBook(item);

        //    //Set each value with the new item as "marked", and their values need to be updated when caching the weights again.
        //    foreach (KeyValuePair<Item[], CacheElement> pair in weightCache)
        //    {
        //        if (pair.Key.Contains(item))
        //        {
        //            pair.Value.marked = true;
        //        }
        //    }
        //}

        //private static float EvaluationFunction(Node[] graph, Item[][] itemSets)
        //{
        //    Dictionary<Item[], CacheElement> cache = new Dictionary<Item[], CacheElement>();
        //    foreach (Item[] set in itemSets)
        //    {
        //        cache.Add(set, new CacheElement()
        //        {
        //            marked = true,
        //            weight = 0,
        //        });
        //    }

        //    float result = 0;

        //    foreach (Item[] set in itemSets.OrderByDescending(set => set.Length))
        //    {
        //        result += EvaluateSet(graph, cache, set);
        //    }

        //    return result;
        //}

        //private static float EvaluateSet(Node[] graph, Dictionary<Item[], CacheElement> cache, Item[] set)
        //{
        //    float importance = Importance(set);
        //    if (importance <= 0)
        //    {
        //        return 0;
        //    }

        //    return importance * Weight(graph, cache, set);
        //}
        
        public static void InitializeWeight(params Node[] nodes)
        {
            _minimalNetwork = new Network<Shelf>(nodes, n => n is Shelf, n => new NetworkNode<Shelf>((Shelf)n));
        }

        public static float Weight(params Item[] itemSet)
        {
            return Weight(_minimalNetwork.AllNodes, _cache, itemSet);
        }

        public static float Weight(Node[] graph, WeightCache cache, params Item[] itemSet)
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
                    if (cache[itemSet.Except(frontier.books).ToArray()].Marked)
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

