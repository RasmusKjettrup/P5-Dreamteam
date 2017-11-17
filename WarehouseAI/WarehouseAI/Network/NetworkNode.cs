using System.Linq;

namespace WarehouseAI.Network
{
    public class NetworkNode : Node, INetworkNode
    {
        public Node Parent { get; set; }

        public NetworkNode(Node parent)
        {
            Parent = parent;
        }

        public void SetEdges(Edge<Node>[] edges)
        {
            _edges = edges;
            _edges = _edges.OrderBy(e => e.weight).ToArray();
        }
    }
}