namespace WarehouseAI
{
    public class ShelfNetworkNode : NetworkNode
    {
        public new Shelf Parent;

        public ShelfNetworkNode(Shelf parent) : base(parent)
        {
            Parent = parent;
        }
    }
}