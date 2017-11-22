using System;
using System.Collections.Generic;
using System.Linq;
using WarehouseAI.Representation;

namespace WarehouseAI.Pathfinding
{
    /// <summary>
    /// A map of distances from all nodes in a graph to all other nodes in the graph.
    /// </summary>
    public class DistanceMap
    {
        private Dictionary<int, Dictionary<int, float>> _dictionary;
        private Node[] _fullGraph;

        public DistanceMap(Node[] graph)
        {
            InitializeDistanceMap(graph);
            List<Node> allNodes = new List<Node>();
            allNodes.Add(graph[0]);
            int i = 0;
            while (i != allNodes.Count)
            {
                foreach (Node neighbour in allNodes[i].Neighbours)
                {
                    if (!allNodes.Contains(neighbour))
                    {
                        allNodes.Add(neighbour);
                    }
                }
                i++;
            }
            _fullGraph = allNodes.ToArray();
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

        private class DijkstraData
        {
            public float Distance;
            public bool Visited;
        }

        private void DistancesFromNode(Node node)
        {
            Dictionary<int, float> nodeSubDictionary;
            if (_dictionary.TryGetValue(node.Id, out nodeSubDictionary))
            {
                Dictionary<Node, DijkstraData> dijkstraDict = new Dictionary<Node, DijkstraData>();
                foreach (Node n in _fullGraph)
                {
                    dijkstraDict.Add(n,new DijkstraData()
                    {
                        Distance = float.MaxValue,
                        Visited = false,
                    });
                }
                dijkstraDict[node].Distance = 0;

                Node currentNode = node;

                while (nodeSubDictionary.ContainsValue(float.MaxValue))
                {
                    foreach (Edge<Node> edge in currentNode.Edges)
                    {
                        float dist = edge.weight + dijkstraDict[currentNode].Distance;
                        if (dist < dijkstraDict[edge.to].Distance)
                        {
                            dijkstraDict[edge.to].Distance = dist;
                        }
                    }

                    float dummy;
                    if (nodeSubDictionary.TryGetValue(currentNode.Id, out dummy))
                    {
                        nodeSubDictionary[currentNode.Id] = dijkstraDict[currentNode].Distance;
                    }
                    dijkstraDict[currentNode].Visited = true;

                    float minDistance = float.MaxValue;
                    foreach (KeyValuePair<Node,DijkstraData> pair in dijkstraDict.Where(pair => pair.Value.Visited == false))
                    {
                        if (pair.Value.Distance<minDistance)
                        {
                            minDistance = pair.Value.Distance;
                            currentNode = pair.Key;
                        }
                    }
                }
            }
        }
    }
}