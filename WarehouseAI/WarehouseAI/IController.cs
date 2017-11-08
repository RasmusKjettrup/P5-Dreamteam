namespace WarehouseAI
{
    public interface IController
    {
        WarehouseRepresentation warehouse { get; set; }

        void Start(params string[] args);
    }
}