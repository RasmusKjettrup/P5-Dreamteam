using System.Linq;

namespace WarehouseAI
{
    public class NetworkNode : Node
    {
        public Node Parent;

        public NetworkNode(Node parent)
        {
            Parent = parent;
        }

        public NetworkNode Cast()
        {
            return new NetworkNode(Parent);
        }

        public void SetEdges(Edge<Node>[] edges)
        {
            _edges = edges;
            _edges = _edges.OrderBy(e => e.weight).ToArray();
        }
    }
}