using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseAI
{
    public class Frontier
    {
        public Frontier(List<Item> route, List<Item> books, int weight)
        {
            this.route = route;
            this.books = books;
            this.weight = weight;
        }
        /// <summary>
        /// An ordered set of nodes in G.
        /// </summary>
        public List<Item> route { get; set; }

        /// <summary>
        /// The remaning books required before the frontier should find its wayback to the dropoff point.
        /// </summary>
        public List<Item> books { get; set; }
        /// <summary>
        /// the weight of the path the forntier has taken.
        /// </summary>
        public int weight { get; set; }
    }
}
