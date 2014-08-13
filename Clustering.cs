using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    /// <summary>
    /// Max-Spacing k-Clusterings.
    /// </summary>
    public class Clustering
    {
        private readonly int _k;    // k-clustering
        public Clustering(int numberOfClusters)
        {
            _k = numberOfClusters;
        }

        public int Execute(string filePath)
        {
            
            int retval = 0;
            List<Edge1> edges = EdgeBuilder.BuildEdges(filePath);
            //edges.Add(new Edge1(1, 2, 1));
            //edges.Add(new Edge1(1, 3, 3));
            //edges.Add(new Edge1(1, 4, 8));
            //edges.Add(new Edge1(1, 5, 12));
            //edges.Add(new Edge1(1, 6, 13));
            //edges.Add(new Edge1(2, 3, 2));
            //edges.Add(new Edge1(2, 4, 14));
            //edges.Add(new Edge1(2, 5, 11));
            //edges.Add(new Edge1(2, 6, 10));
            //edges.Add(new Edge1(3, 4, 15));
            //edges.Add(new Edge1(3, 5, 17));
            //edges.Add(new Edge1(3, 6, 16));
            //edges.Add(new Edge1(4, 5, 7));
            //edges.Add(new Edge1(4, 6, 19));
            //edges.Add(new Edge1(5, 6, 9));

            

            // Kruskal's: Sort edges in asc order of costs
            PerformQuickSort(edges, 0, edges.Count - 1);
            retval = KruskalRoutine(edges);
            return retval;
        }

        private int KruskalRoutine(List<Edge1> edges)
        {
            /* Start picking and adding edges to the UnionFind
             * without creating cycles. Stop when UnionFind components
             * become 'k'-clusters.
             */

            // Initialize UnionFind
            UnionFind uf = new UnionFind(EdgeBuilder.ExtractVertices(edges));
            int maxSpacing = 0;
            foreach (Edge1 edge in edges)
            {
                // If the vertices (clusters) are in different components, add.
                if (uf.Find(edge.U) != uf.Find(edge.V))
                {
                    // Check if k-cluster reached
                    if (uf.Components == _k)
                    {
                        maxSpacing = edge.Cost;
                        break;
                    }
                    uf.Union(edge.U, edge.V);
                }
            }
            return maxSpacing;
        }


        private static void PerformQuickSort(List<Edge1> edges, int l, int r)
        {
            //if ((r - l) == 0) { return; }
            if (l < r)
            {
                int pivotIndex = Partition(edges, l, r);
                PerformQuickSort(edges, l, (pivotIndex - 1));
                PerformQuickSort(edges, pivotIndex + 1, r);
            }
        }

        private static int Partition(List<Edge1> edges, int l, int r)
        {
            Edge1 p = edges[r];
            int i = l;
            Edge1 temp;

            for (int j = l; j <= r - 1; j++)
            {
                // Second condition to break ties. Edge with higher 
                // weight first (array will be finally in ascending order
                if (edges[j].Cost <= p.Cost)
                {
                    // swap j with i, increment i
                    temp = edges[i];
                    edges[i] = edges[j];
                    edges[j] = temp;
                    i++;
                }
            }
            // position pivot
            temp = edges[r];
            edges[r] = edges[i];
            edges[i] = temp;

            // return pivot's index
            return (i);
        }
    }


}
