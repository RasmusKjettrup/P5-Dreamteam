using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseAI
{
    public class Arc
    {
        /// <summary>
        /// The first item in the relation.
        /// </summary>
        public Item Item1 { get; set; }
        /// <summary>
        /// An item frequently bought with the first item.
        /// </summary>
        public Item Item2 { get; set; }

        public Arc(Item item1, Item item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }
}
