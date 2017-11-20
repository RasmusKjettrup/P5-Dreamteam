using WarehouseAI.Representation;

namespace WarehouseAI.ShortestPathGraph
{
    public interface IShortestPathGraphNode
    {
        Node Parent { get; set; }
        void SetEdges(Edge<Node>[] edges);
    }
}