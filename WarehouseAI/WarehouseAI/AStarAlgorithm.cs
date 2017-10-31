using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseAI
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
        private float Calculate_hCost(Node n)
        {
            throw new NotImplementedException();
        }

        private void ExploreNode(Node n)
        {
            //n.gCost = Calculate_gCost(n);
            n.hCost = Calculate_hCost(n);
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
                else if (n.fCost== lastFCost && lastHCost < n.hCost)
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
         
        public List<Node> FindPath(Node startingNode, Node endingNode)
        {
            //Initialize the start node.
            startingNode.gCost = 0;
            startingNode.hCost = Calculate_hCost(startingNode);

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
                    return BackTrackPath(endingNode);

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
                    neighbour.hCost = Calculate_hCost(neighbour);
                }
            }

            return null;
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
