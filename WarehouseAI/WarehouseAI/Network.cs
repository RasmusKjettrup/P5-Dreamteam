using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseAI
{
    public class Network<T> where T : Node
    {
        public readonly NetworkNode<Node> Dropoff;
        public readonly NetworkNode<T>[] Nodes;
        public NetworkNode<Node>[] AllNodes => Dropoff.Append(Nodes.Select(n => n.Cast())).ToArray();

        public Network(WarehouseRepresentation warehouse, Func<Node, bool> include, Func<T, NetworkNode<T>> conversion)
        {
            Dropoff = new NetworkNode<Node>(warehouse.Nodes[0]);
            List<NetworkNode<T>> nodes = new List<NetworkNode<T>>();
            foreach (Node node in warehouse.Nodes.Skip(1))
            {
                if (include(node))
                {
                    nodes.Add(conversion((T)node));
                }
            }
            Nodes = nodes.ToArray();
        }
    }
}