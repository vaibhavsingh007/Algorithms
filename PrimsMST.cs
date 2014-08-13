using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    /// <summary>
    /// Computes MST in O(E log V) time complexity
    /// </summary>
    public class PrimsMST
    {
        public void Compute(string filePath)
        {
            // Initialize X and T
            List<int> _X = new List<int>();     // 1st invariant
            int mstCost = 0;

            // Read the file in a dict<E, W>
            List<Edge1> edges = EdgeBuilder.BuildEdges(filePath);
            //edges.Add(new Edge(1, 6, 7));
            //edges.Add(new Edge(5, 6, 4));
            //edges.Add(new Edge(1, 5, 1));
            //edges.Add(new Edge(1, 2, 6));
            //edges.Add(new Edge(1, 4, 4));
            //edges.Add(new Edge(2, 5, 7));
            //edges.Add(new Edge(4, 5, 5));
            //edges.Add(new Edge(2, 4, 3));
            //edges.Add(new Edge(2, 3, 4));
            //edges.Add(new Edge(3, 4, 2));

            // Create heap data (vertices with cost)
            List<int> vertices = EdgeBuilder.ExtractVertices(edges);

            // Assign 'infinity' cost to the vertices.
            List<KeyValuePair<int, int>> verticesWithCost = new List<KeyValuePair<int,int>>();
            foreach (int vertex in vertices)
            {
                verticesWithCost.Add(new KeyValuePair<int, int>(Int32.MaxValue, vertex));
            }

            // Initialise the min-heap
            PriorityQueue<int, int> minHeap = new PriorityQueue<int, int>(verticesWithCost);

            // Begin Prim's MST sequence
            // Start at an arbitrary vertex
            int firstVertex = minHeap.Dequeue().Value;
            _X.Add(firstVertex);

            foreach (Edge1 edge in edges)
            {
                if (edge.U == firstVertex)
                    minHeap.DecreaseKey(new KeyValuePair<int, int>(edge.Cost, edge.V));
                else if (edge.V == firstVertex)
                    minHeap.DecreaseKey(new KeyValuePair<int, int>(edge.Cost, edge.U));
            }

            for (int j = 1; j < vertices.Count; j++)
            {
                // Extract min
                mstCost += minHeap.Peek().Key;
                _X.Add(minHeap.Dequeue().Value);

                // Recompute cost of all vertices incident to this (assuming no parallel edges)
                for (int i = 0; i < edges.Count; i++)
                {
                    // If 'w'E V-X
                    if (_X.Exists(x => x == edges[i].U) && !_X.Exists(x => x == edges[i].V))
                    {
                        // V (or w) is the vertex we want to update
                        minHeap.DecreaseKey(new KeyValuePair<int, int>(edges[i].Cost, edges[i].V));
                    }
                    else if (_X.Exists(x => x == edges[i].V) && !_X.Exists(x => x == edges[i].U))
                    {
                        // U is the vertex we want to update
                        minHeap.DecreaseKey(new KeyValuePair<int, int>(edges[i].Cost, edges[i].U));
                    }
                    else if (_X.Exists(x => x == edges[i].V) && _X.Exists(x => x == edges[i].U))
                    {
                        //edges.RemoveAt(i);     // Already in X
                    }
                }
            }
            string stub = "";
        }
    }

    public struct Edge1
    {
        public int U;
        public int V;
        public int Cost;
        public Edge1(int u, int v, int w)
        {
            this.U = u;
            this.V = v;
            this.Cost = w;
        }
    }
}
