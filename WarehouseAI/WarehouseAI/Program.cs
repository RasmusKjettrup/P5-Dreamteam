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

            consoleController.warehouse = warehouse;
            consoleController.itemDatabase = itemDatabase;
            warehouse.ItemDatabase = itemDatabase;

            warehouse.Inintialize();

            consoleController.Start(args);
            
        }
    }
}
