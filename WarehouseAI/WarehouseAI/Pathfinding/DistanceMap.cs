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
                int i = 0;
                List<Tuple<Node, float>> frontiers = new List<Tuple<Node, float>>();

                frontiers.Add(new Tuple<Node, float>(node, 0));

                while (frontiers.Count != i)
                {
                    Tuple<Node, float> currentFront = frontiers[i];

                    float weight;
                    if (nodeSubDictionary.TryGetValue(currentFront.Item1.Id, out weight))
                    {
                        if (currentFront.Item2 < weight)
                        {
                            nodeSubDictionary[currentFront.Item1.Id] = currentFront.Item2;
                        }
                    }

                    foreach (Edge<Node> edge in currentFront.Item1.Edges)
                    {
                        float newWeight = currentFront.Item2 + edge.weight;
                        bool add = true;
                        foreach (Tuple<Node, float> frontier in frontiers)
                        {
                            if (frontier.Item1 == edge.to && frontier.Item2 < newWeight)
                            {
                                add = false;
                                break;
                            }
                        }
                        if (add)
                        {
                            frontiers.Add(new Tuple<Node, float>(edge.to, newWeight));
                        }
                    }
                    i++;
                }
            }
        }
    }
}