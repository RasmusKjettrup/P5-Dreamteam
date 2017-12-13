using WarehouseAI.Representation;
using WarehouseAI.UI;

namespace WarehouseAI
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            ItemDatabase itemDatabase = new ItemDatabase();
            WarehouseRepresentation warehouse = new WarehouseRepresentation(itemDatabase);
            IController consoleController = new ConsoleController(warehouse, itemDatabase);

            warehouse.Inintialize();

            consoleController.Start(args);
            
        }
    }
}
