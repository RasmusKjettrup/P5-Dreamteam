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

        private static List<Item> _addedBooks;

        private class FilterShelf : Shelf
        {
            private Shelf _parentShelf;
            private Item _filterItem;

            private int Capacity;

            public FilterShelf(Shelf parent, Item filter)
            {
                _parentShelf = parent;
                _filterItem = filter;
            }

            public override Item[] Items {
                get { return _parentShelf.Items.Where(i => i != _filterItem).ToArray(); }
            }
        }

        private static float EvaluationFunction(Node[] network)
        {

            return 0;
        }

        public static void AddBook(Item item)
        {
            FilterShelf[] subNetwork = minimalNetwork.Skip(1).Select(n => new FilterShelf((Shelf) n, item)).ToArray();
            Node currentNode = minimalNetwork[0];
        }

        private static Node[] minimalNetwork;

        public static void InitializeWeight(params Node[] nodes)
        {
            List<Node> g = new List<Node>();

            foreach (Node n_i in nodes)
            {
                if (!(n_i is Shelf /*or n_i is the dropoff point*/))
                {
                    continue;
                }

                Node g_i = new Node();
                List<Edge> g_edges = new List<Edge>();
                foreach (Node n_j in nodes)
                {
                    if (n_j is Shelf /*or n_j is the dropoff point*/)
                    {
                        g_edges.Add(new Edge()
                        {
                            weight = 0, //TODO: Implement a pathfinding algorithm and set the weight to be weight of the resulting path from n_i to n_j
                            to = n_j
                        });
                    }
                }
                g_i.Edges = g_edges.ToArray();
                g.Add(g_i);
            }

            foreach (Node node in g)
            {
                node.Edges = node.Edges.OrderBy(e => e.weight).ToArray();
            }

            minimalNetwork = g.ToArray();
        }

        public static float Weight(params Item[] i)
        {
            float TotalWeight = 0;
            List<Frontier> frontiers = new List<Frontier>();
            frontiers.Add(new Frontier(new[] { minimalNetwork[0] }, i, 0));

            while (true)
            {
                Frontier resultingFrontier = new Frontier(null, null, int.MaxValue);
                foreach (Frontier f_i in frontiers)
                {
                    Node f_l = f_i.route.Last();
                    if (f_i.books.Length >= 1)
                    {
                        foreach (Shelf g_i in f_l.Neighbours.Cast<Shelf>())
                        {
                            if (g_i.Items.Any(item => f_i.books.Contains(item)))
                            {
                                if (!frontiers.Select(f => f.route).Contains(f_i.route.Concat(new[] { g_i }))
                                    && f_i.weight/*+dist(f_l, g_i)*/< resultingFrontier.weight)
                                {
                                    resultingFrontier = new Frontier(f_i.route.Concat(new[] { g_i }).ToArray(),
                                        f_i.books.Where(item => !g_i.Items.Contains(item)).ToArray(),
                                        f_i.weight/*+dist(f_l, g_i)*/);
                                }
                            }
                        }
                    }
                    else
                    {
                        Node g_0 = minimalNetwork[0];
                        if (f_i.weight/*+dist(f_l, g_i)*/< resultingFrontier.weight)
                        {
                            resultingFrontier = new Frontier(f_i.route.Concat(new[] { g_0 }).ToArray(),
                                f_i.books, f_i.weight/*+dist(f_l, g_i)*/);
                        }
                    }
                }
                frontiers.Add(resultingFrontier);
                if (resultingFrontier.route.Last() == minimalNetwork[0])
                {
                    return resultingFrontier.weight;
                }
            }
        }
    }
}