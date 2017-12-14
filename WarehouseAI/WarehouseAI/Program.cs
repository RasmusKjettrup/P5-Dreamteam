using WarehouseAI.Representation;
using WarehouseAI.UI;

namespace WarehouseAI
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            IController consoleController = new ConsoleController();
            WarehouseRepresentation warehouse = new WarehouseRepresentation();
            ItemDatabase itemDatabase = new ItemDatabase();

            consoleController.Warehouse = warehouse;
            consoleController.ItemDatabase = itemDatabase;
            warehouse.ItemDatabase = itemDatabase;

//            warehouse.Initialize();

            consoleController.Start(args);
            
        }
    }
}
