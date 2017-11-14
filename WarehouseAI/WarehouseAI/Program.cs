using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseAI
{
    public class Program
    {
        private static void Main(string[] args)
        {
            IController consoleController = new ConsoleController();
            WarehouseRepresentation warehouse = new WarehouseRepresentation();
            ItemDatabase itemDatabase = new ItemDatabase();

            consoleController.warehouse = warehouse;
            consoleController.itemDatabase = itemDatabase;
            warehouse.ItemDatabase = itemDatabase;

            warehouse.Inintialize();

            consoleController.Start(args);
        }
    }
}
