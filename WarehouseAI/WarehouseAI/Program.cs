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
            ConsoleController consoleController = new ConsoleController();
            WarehouseRepresentation warehouse = new WarehouseRepresentation();

            consoleController.warehouse = warehouse;

            consoleController.Start(args);
        }
    }
}
