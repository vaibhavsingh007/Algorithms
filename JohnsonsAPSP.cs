using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public class JohnsonsAPSP
    {
        private List<Edge1> _edges;
        private List<int> _vertices;
        private int[,] A;
        private int _totalVertices; // Will be used for i in A[i,v]

        private const int Infinite = 1000000;    // Hopefully. Do not change this to max value
                // ..because during B-Ford, computing case 2 will fail when Cwv will be added to maxval.

        /// <summary>
        /// Returns shortest path edge
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public Edge1 Execute(string filePath)   //
        {
            Edge1 retval;
            _edges = EdgeBuilder.BuildEdges(filePath);

            //_edges.Add(new Edge(1, 2, -2));
            //_edges.Add(new Edge(2, 3, -1));
            //_edges.Add(new Edge(3, 1, 4));
            //_edges.Add(new Edge(3, 4, 2));
            //_edges.Add(new Edge(3, 5, -3));
            //_edges.Add(new Edge(6, 4, 1));
            //_edges.Add(new Edge(6, 5, -4));
            //_edges.Add(new Edge(4, 1, 0));    // To test -ve cost cycle

            _vertices = EdgeBuilder.ExtractVertices(_edges);
            _vertices.OrderBy(v => v);

            AttachTempVertex_S();
            if (!BFord())
            {
                throw new InvalidOperationException("Negative cost cycle found in graph");
            }
            ReWeightG();
            retval = RunDijkstras();
            return retval;
        }

        /// <summary>
        /// Attaches the temporary vertex S with edges of cost 0
        /// to every other v E (G) to facilitate the execution of
        /// B-Ford algorithm.
        /// </summary>
        private void AttachTempVertex_S()
        {
            // Denoting S as 0
            _vertices.ForEach(v => _edges.Add(new Edge1(0, v, 0)));
            _vertices.Add(0);
        }

        /// <summary>
        /// Run Bellman Ford iteration once to compute
        /// shortest path from S to all v E (G) and also
        /// detect a negative cost cycle, if any.
        /// Minimun vertices costs will be available in A[n-1,v]
        /// </summary>
        private bool BFord()
        {
            _totalVertices = _vertices.Count;    // n (# of vertices)
            A = new int[_totalVertices, _totalVertices];    // A[i,v]

            // Initialize A[0,v] to infinity
            for (int i = 0; i < _totalVertices; i++)
            {
                for (int v = 0; v < _totalVertices; v++)
                {
                    A[i, v] = Infinite;     // Actually, initialized all to infinity.
                }
            }
            A[0, 0] = 0;    // Source vertex to itself
            
            // Start BFord iteration
            // For detecting negative cost cycle - final iteration (n) of the 
            // outer for loop is run (otherwise, i = 0 to n-1)
            for (int i = 1; i < _totalVertices; i++)
            {
                bool stopEarly = true;
                foreach (int v in _vertices)
                {
                    int case1 = A[i - 1, v];
                    int case2;                  // min (of all c(w,v) in E(G)){A[i-1, w] + Cwv}
                    int minC_sv = Infinite;    // Minimum (sw + wv) cost

                    // Find min of all wv paths (in-degree of v)
                    // From all the edges, Cwv of all edges with v == current v
                    foreach (Edge1 wv_edge in _edges.Where(e => e.V == v))
                    {
                        int current_svLength = A[i - 1, wv_edge.U] + wv_edge.Cost;
                        if (current_svLength < minC_sv)
                        {
                            minC_sv = current_svLength;
                        }
                    }
                    case2 = minC_sv;

                    A[i, v] = Math.Min(case1, case2);

                    // Check stopping early condition
                    if (A[i - 1, v] != A[i, v])
                    {
                        stopEarly = false;
                    }
                }
                if (stopEarly)
                {
                    foreach (int v in _vertices)
                    {
                        A[_totalVertices - 2, v] = A[_totalVertices - 1, v] = A[i, v];
                    }
                    break;
                }
            }


            foreach (int v in _vertices)
            {
                // totalVertices -2 and -1 are to maintain indices.
                // Actually A[n-1,v] == A[n,v] when no -ve cost cycle.
                if (A[_totalVertices - 2, v] != A[_totalVertices - 1, v])
                {
                    return false;
                }
            }

            return true;    // No negative cost cycle found and shortest path computed.
        }

        /// <summary>
        /// Reweighting Technique : Computing C'e for all edges in G --> G'
        /// </summary>
        private void ReWeightG()
        {
            int newCe;

            // Remove S and its edges from G
            _vertices.Remove(0);
            _edges.RemoveAll(e => e.U == 0);
            for (int e = 0; e < _edges.Count; e++)
            {
                // C'e = Ce + Pu - Pv
                newCe = _edges[e].Cost + A[_totalVertices - 1, _edges[e].U] - A[_totalVertices - 1, _edges[e].V];
                Edge1 e1 = _edges[e];
                e1.Cost = newCe;
                _edges[e] = e1;
            }
        }

        /// <summary>
        /// Run Dijkstra's Algorigthm x n (for each vertex).
        /// Returns shortest shortest path Edge
        /// Record the shortest shortest path amongst all vertices.
        /// O(n*mlogn)
        /// </summary>
        private Edge1 RunDijkstras()
        {
            int min = Infinite;
            List<KeyValuePair<int, int>> minPaths = null;
            Edge1 minEdge = new Edge1(0, 0, 0);

            Dijkstras dijkstraObj = new Dijkstras(_edges);

            foreach (int u in _vertices)
            {
                minPaths = dijkstraObj.Compute(u);

                foreach (KeyValuePair<int, int> minPath in minPaths)
                {
                    // d(u) or Ce = C'e + Pv - Pu
                    // Actual cost, reversing the reweight C'e
                    int d_u = minPath.Value + A[_totalVertices - 1, minPath.Key] - A[_totalVertices - 1, u];

                    if (d_u < min)
                    {
                        min = d_u;
                        minEdge.U = u;
                        minEdge.V = minPath.Key;
                        minEdge.Cost = min;    // d(u)
                    }
                }
            }
            return minEdge;
        }

        /// <summary>
        /// Gets original cost Ce. G' --> G
        /// </summary>
        private void GetOriginalCosts()
        {
            int Ce;

            for (int e = 0; e < _edges.Count; e++)
            {
                // Ce = C'e + Pv - Pu 
                Ce = _edges[e].Cost + A[_totalVertices - 1, _edges[e].V] - A[_totalVertices - 1, _edges[e].U];
                Edge1 e1 = _edges[e];
                e1.Cost = Ce;
                _edges[e] = e1;
            }
        }
    }
}
