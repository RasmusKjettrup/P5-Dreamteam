using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseAI
{
    public class Frontier
    {
        public Frontier(Node[] route, Item[] books, float weight)
        {
            this.route = route;
            this.books = books;
            this.weight = weight;
        }
        /// <summary>
        /// An ordered set of nodes in G.
        /// </summary>
        public Node[] route { get; }

        /// <summary>
        /// The remaning books required before the frontier should find its wayback to the dropoff point.
        /// </summary>
        public Item[] books { get; }
        /// <summary>
        /// the weight of the path the forntier has taken.
        /// </summary>
        public float weight { get; }
    }
}
