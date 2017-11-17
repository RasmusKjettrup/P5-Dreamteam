namespace WarehouseAI
{
    public interface INetworkNode
    {
        Node Parent { get; set; }
        void SetEdges(Edge<Node>[] edges);
    }
}