using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseAI
{
    /*     Dijkstra no longer needed?     */

    
    public class DijkstraAlgorithm
    {
        //private List<Node> vertexSet;

        //public int[,] GetDistanceMatrix(List<Node> vertices, List<Node> edges, Node source)
        //{
        //    int[,] distanceMatrix = new int[vertices.Count, vertices.Count];
        //    Node current;
        //    vertexSet = new List<Node>();

        //    for (int i = 0; i < vertices.Count; i++)
        //    {
        //        vertexSet.Add(vertices[i]);
        //        for (int j = 0; j < vertices.Count; j++)
        //        {
        //            if (vertices[i] == source && i == j)
        //                distanceMatrix[i, j] = 0; //Set the distance from source to itself to 0.
        //            else
        //                distanceMatrix[i, j] = -1;
        //        }
        //    }
            
        //    while (vertexSet.Count > 0)
        //    {
        //        current = 
        //    }
            

        //    return distanceMatrix;
        //}

        //private Node GetVertexWithMinimumDistanceToSource(List<Node> vertices, Node source)
        //{
        //    return null;
        //}
        private Dictionary<Tuple<Node,Node>, float> _distances = new Dictionary<Tuple<Node,Node>, float>();

        private void LoadAllDistances(List<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                foreach (Node node1 in nodes)
                {
                    _distances.Add(new Tuple<Node, Node>(node,node1), float.MaxValue);
                }
            }
        }

        public void GenerateMatrix(List<Node> nodes)
        {
            LoadAllDistances(nodes);
            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < nodes.Count; j++)
                {
                    if (i < j)
                    {
                        _distances[new Tuple<Node, Node>(nodes[i], nodes[j])] = CalculateDistance(nodes[i], nodes[j], 0);
                        _distances[new Tuple<Node, Node>(nodes[j], nodes[i])] = _distances[new Tuple<Node, Node>(nodes[i], nodes[j])];
                    }
                    else if (i == j)
                    {
                        _distances[new Tuple<Node, Node>(nodes[i], nodes[j])] = 0;
                    }
                }
            }
        }

        private float CalculateDistance(Node currentNode, Node endNode, float relativeWeight)
        {
            List<Node> markedNodes = new List<Node>();
            Dictionary<Node, float> queue = new Dictionary<Node, float>();
            Node next = null;
            float val = 0;
            while (next == null || next == endNode)
            {
                markedNodes.Add(currentNode);
                Edge<Node>[] edges = currentNode.Edges;
                foreach (Edge<Node> t in edges)
                {
                    if (markedNodes.Contains(t.to)) continue;
                    val = t.weight + relativeWeight;
                    if (!queue.ContainsKey(t.to))
                        queue.Add(t.to, t.weight + relativeWeight);
                    else if (queue.ContainsKey(t.to) && val < queue[t.to])
                        queue[t.to] = val;
                }
                KeyValuePair<Node, float> nextPair = queue.OrderBy(v => v.Value).First();
                next = nextPair.Key;
                val = nextPair.Value;
                queue.Remove(next);
                
                currentNode = next;
                relativeWeight = val;
            }
            return val;
        }
    }
}
