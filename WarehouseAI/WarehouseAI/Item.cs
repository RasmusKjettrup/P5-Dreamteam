using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseAI
{
    class Item
    {
        public string ID { get; set; }
        public string Name { get; set; }

        public Item(string id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}
