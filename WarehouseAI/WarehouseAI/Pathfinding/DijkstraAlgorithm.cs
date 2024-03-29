﻿using System;
using System.Collections.Generic;
using System.Linq;
using WarehouseAI.Representation;

namespace WarehouseAI.Pathfinding
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

        /// <summary>
        /// A dictionary which consists of a tuple and a float.
        /// Where the tuple contain two nodes and the float represent the distance between them. 
        /// </summary>
        private Dictionary<Tuple<Node, Node>, float> _distances = new Dictionary<Tuple<Node, Node>, float>();
        /// <summary>
        /// Sets nodes distance to maxvalue
        /// </summary>
        /// <param name="nodes"></param>
        private void LoadAllDistances(List<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                foreach (Node node1 in nodes)
                {
                    _distances.Add(new Tuple<Node, Node>(node, node1), float.MaxValue);
                }
            }
        }
        /// <summary>
        /// Generates a matrix of the distances between the nodes.
        /// All pairs shortest path matrix
        /// </summary>
        /// <param name="nodes"></param>
        public void GenerateMatrix(List<Node> nodes)
        {
            LoadAllDistances(nodes);
            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < nodes.Count; j++)
                {
                    if (i < j)
                    {
                        // We know that all pairs of nodes are in _distances.
                        // Therefore, we calculate distance from node1 to node2 and set the same distance from node2 to node1
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
        /// <summary>
        /// Calculates the distance between the nodes.
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="endNode"></param>
        /// <param name="relativeWeight"></param>
        /// <returns>The distance</returns>
        private float CalculateDistance(Node currentNode, Node endNode, float relativeWeight)
        {
            List<Node> markedNodes = new List<Node>();// already visited nodes.
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
