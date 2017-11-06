using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseAI
{
    /*     Dijkstra no longer needed?     */

    /*
    public class DijkstraAlgorithm
    {
        private List<Node> vertexSet;

        public int[,] GetDistanceMatrix(List<Node> vertices, List<Node> edges, Node source)
        {
            int[,] distanceMatrix = new int[vertices.Count, vertices.Count];
            Node current;
            vertexSet = new List<Node>();

            for (int i = 0; i < vertices.Count; i++)
            {
                vertexSet.Add(vertices[i]);
                for (int j = 0; j < vertices.Count; j++)
                {
                    if (vertices[i] == source && i == j)
                        distanceMatrix[i, j] = 0; //Set the distance from source to itself to 0.
                    else
                        distanceMatrix[i, j] = -1;
                }
            }
            
            while (vertexSet.Count > 0)
            {
                current = 
            }
            

            return distanceMatrix;
        }

        private Node GetVertexWithMinimumDistanceToSource(List<Node> vertices, Node source)
        {
            return null;
        }
    }
    */
}
