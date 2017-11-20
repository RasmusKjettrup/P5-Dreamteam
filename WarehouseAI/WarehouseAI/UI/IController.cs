using WarehouseAI.Representation;

namespace WarehouseAI.UI
{
    public interface IController
    {
        WarehouseRepresentation warehouse { get; set; }
        ItemDatabase itemDatabase { get; set; }

        void Start(params string[] args);
    }
}