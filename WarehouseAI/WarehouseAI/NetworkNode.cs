using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace WarehouseAI
{
    public class NetworkNode<T> : Node where T: Node
    {
        public T Parent;

        public NetworkNode(T parent)
        {
            Parent = parent;
        }

        public NetworkNode<Node> Cast()
        {
            return new NetworkNode<Node>(Parent);
        }

        public void SetEdges(Edge[] edges)
        {
            _edges = edges;
            _edges = _edges.OrderBy(e => e.weight).ToArray();
        }
    }
}