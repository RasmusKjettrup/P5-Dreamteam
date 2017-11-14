using System.Linq;

namespace WarehouseAI
{
    public class NetworkNode : Node
    {
        public Node Parent;

        protected new Edge<NetworkNode>[] _edges;
        public new NetworkNode[] Neighbours => _edges.Select(e => e.to).ToArray();

        public NetworkNode(Node parent)
        {
            Parent = parent;
        }

        public NetworkNode Cast()
        {
            return new NetworkNode(Parent);
        }

        public void SetEdges(Edge<NetworkNode>[] edges)
        {
            _edges = edges;
            _edges = _edges.OrderBy(e => e.weight).ToArray();
        }
    }
}