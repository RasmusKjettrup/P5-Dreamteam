using WarehouseAI.Representation;

namespace WarehouseAI.UI
{
    public interface IController
    {
        WarehouseRepresentation Warehouse { get; set; }
        ItemDatabase ItemDatabase { get; set; }

        void Start(params string[] args);
    }
}