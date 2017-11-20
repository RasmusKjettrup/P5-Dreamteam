using System;
using System.Collections.Generic;
using WarehouseAI.Representation;

namespace WarehouseAI.Pathfinding
{
    public class AStarAlgorithm
    {
        private List<Node> openSet;
        private HashSet<Node> closedSet;
/*
        // Cost from start to current.
        private float Calculate_gCost(Node n)
        {
            // Get the G-Cost to the previous node
            float gCostToCameFrom = n.CameFrom.gCost;

            //Find the edge from the previous node to the current node
            Edge EdgeToCameFrom = n.GetEdgeToNeighbour(n.CameFrom);

            //Add the weight of the new edge to the g-Cost of the previous node
            return EdgeToCameFrom.weight + gCostToCameFrom;
        }
        */

        // Estimated cost from current to goal node.
        private float Calculate_hCost(Node start, Node goal)
        {
            if (start == null || goal == null)
            {
                throw new UnfittingNodeException("cannot calculate distance for A*, nodes are null");
            }
            double x1, x2, y1, y2;
            x1 = start.X;
            y1 = start.Y;
            x2 = goal.X;
            y2 = goal.Y;
            return (float)Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }


        private Node GetNextNodeToInvestigate()
        {
            //find node with the lowest f-cost, else, search by h-cost
            float lastFCost = 1000000, lastHCost = 1000000;
            Node lastNode = null;
            bool NodeFound = false;
            foreach (Node n in openSet)
            {
                if (n.fCost < lastFCost)
                {
                    lastFCost = n.fCost;
                    lastHCost = n.hCost;
                    lastNode = n;
                    NodeFound = true;
                }
                else if (n.fCost == lastFCost && lastHCost < n.hCost)
                {
                    lastFCost = n.fCost;
                    lastHCost = n.hCost;
                    lastNode = n;
                    NodeFound = true;
                }
            }
            if (!NodeFound)
            {
                throw new Exception("GetNextNodeToInvestigate: Did not find any node");
            }
            return lastNode;
        }

        public float FindPath(Node[] graph, Node startingNode, Node endingNode)
        {
            Node[] temp;
            return FindPath(graph, startingNode, endingNode, out temp);
        }

        public float FindPath(Node[] graph, Node startingNode, Node endingNode, out Node[] path)
        {
            //Initialize the start node.
            startingNode.gCost = 0;
            startingNode.hCost = Calculate_hCost(startingNode, endingNode);

            //Initialize g-Cost for all nodes in the graph
            foreach (Node n in graph)
            {
                n.gCost = 1000000; //1.000.000 being some default high value.
            }

            Node current;
            float temp_gCost;
            openSet = new List<Node>();
            closedSet = new HashSet<Node>();

            openSet.Add(startingNode);

            while (openSet.Count > 0)
            {
                //Find a node to investigate within the openSet
                current = GetNextNodeToInvestigate();

                if (current == endingNode)
                {
                    path = BackTrackPath(endingNode).ToArray();
                    return endingNode.gCost;
                }

                //  Remove the currently investigated node from the openSet, 
                //  and add it to the closedSet
                openSet.Remove(current);
                closedSet.Add(current);

                foreach (Node neighbour in current.Neighbours)
                {
                    if (closedSet.Contains(neighbour)) continue;

                    if (!openSet.Contains(neighbour)) openSet.Add(neighbour);

                    temp_gCost = current.gCost + current.GetEdgeToNeighbour(neighbour).weight;
                    if (temp_gCost >= neighbour.gCost) continue;

                    neighbour.CameFrom = current;
                    neighbour.gCost = temp_gCost;
                    neighbour.hCost = Calculate_hCost(neighbour, endingNode);
                }
            }

            path = null;
            return -1;
        }

        private List<Node> BackTrackPath(Node endNode)
        {
            Node current;
            List<Node> path = new List<Node>();
            current = endNode;
            path.Add(current); 
            while ((current = current.CameFrom) != null)
            {
                path.Add(current);
            }
            path.Reverse();
            return path;
        }

        
    }
}
