using System;
using System.Collections.Generic;
using WarehouseAI.Representation;

namespace WarehouseAI.Pathfinding
{
    /// <summary>
    /// A map of distances from all nodes in a graph to all other nodes in the graph.
    /// </summary>
    public class DistanceMap
    {
        private Dictionary<int, Dictionary<int, float>> _dictionary;

        public DistanceMap(Node[] graph)
        {
            InitializeDistanceMap(graph);
            foreach (Node node in graph)
            {
                DistancesFromNode(node);
            }
        }

        public bool TryGet(int x, int y, out float weight)
        {
            Dictionary<int, float> subdict;
            weight = float.MaxValue;
            if (_dictionary.TryGetValue(x, out subdict) && subdict.TryGetValue(y, out weight))
            {
                return true;
            }
            if (_dictionary.TryGetValue(y, out subdict) && subdict.TryGetValue(x, out weight))
            {
                return true;
            }
            return false;
        }

        private void InitializeDistanceMap(Node[] graph)
        {
            int size = graph.Length;
            _dictionary = new Dictionary<int, Dictionary<int, float>>();
            for (int i = 0; i < size; i++)
            {
                Dictionary<int, float> subdict = new Dictionary<int, float>();
                for (int j = i; j < size; j++)
                {
                    subdict.Add(graph[j].Id, float.MaxValue);
                }
                _dictionary.Add(graph[i].Id, subdict);
            }
        }

        private void DistancesFromNode(Node node)
        {
            Dictionary<int, float> nodeSubDictionary;
            if (_dictionary.TryGetValue(node.Id, out nodeSubDictionary))
            {
                Queue<Tuple<Node, float>> frontiers = new Queue<Tuple<Node, float>>();

                frontiers.Enqueue(new Tuple<Node, float>(node, 0));

                while (frontiers.Count != 0)
                {
                    Tuple<Node, float> currentFront = frontiers.Dequeue();

                    bool updatedValue = false;
                    float weight;
                    if (nodeSubDictionary.TryGetValue(currentFront.Item1.Id, out weight))
                    {
                        if (currentFront.Item2 < weight)
                        {
                            updatedValue = true;
                            nodeSubDictionary[currentFront.Item1.Id] = currentFront.Item2;
                        }
                    }

                    if (updatedValue)
                    {
                        foreach (Edge<Node> edge in currentFront.Item1.Edges)
                        {
                            frontiers.Enqueue(new Tuple<Node, float>(edge.to, currentFront.Item2 + edge.weight));
                        }
                    }
                }
            }
        }
    }
}