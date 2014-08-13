using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Algorithms
{
    /// <summary>
    /// This Disjkstra has been customized for Johnson's APSP.
    /// Original Dijkstra's commented below.
    /// </summary>
    public class Dijkstras
    {
        private const int Infinity = 100000;
        private List<KeyValuePair<int, int>> _vertices;
        private List<Edge1> _edges;

        /// <summary>
        /// Returns all the vertices with the min path from source.
        /// This comes from a slight cutomization for Johnson's.
        /// Otherwise, return void and _A array contains the costs of all vertices and _X the order
        /// </summary>
        /// <param name="sourceV"></param>
        /// <returns>Vertex with shortest path from sourceV</returns>
        public List<KeyValuePair<int, int>> Compute(int sourceV)    //  , string filePath
        {
            // Initialize X
            List<int> _X = new List<int>();     // 1st invariant

            // Read the file in a dict<E, W>
            //List<Edge> edges = _edges; // new List<Edge>();

            //string[] lines = File.ReadAllLines(filePath);
            //foreach (string line in lines)
            //{
            //    string[] content = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            //    edges.Add(new Edge(Int32.Parse(content[0]), Int32.Parse(content[1]), Int32.Parse(content[2])));
            //}
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

            

            // Start at the source vertex
            _X.Add(sourceV);
            int[] _A = new int[_vertices.Count + 1];
            _A[sourceV] = 0; // distance from S to itself

            // Initialise the min-heap
            PriorityQueue<int, int> minHeap = new PriorityQueue<int, int>(_vertices);
            
            // Remove source vertex from min-heap, not required.
            minHeap.Remove(new KeyValuePair<int, int>(Infinity, sourceV));

            foreach (Edge1 edge in _edges.Where(e => e.U == sourceV))
                minHeap.DecreaseKey(new KeyValuePair<int, int>(edge.Cost, edge.V));

            // Initialize the main loop
            while (_X.Count != _vertices.Count)
            {
                KeyValuePair<int, int> currMin = minHeap.Peek();    // Peek at the vertex
                _A[currMin.Value] = currMin.Key;    // update W's score
                _X.Add(minHeap.Dequeue().Value);    // Extract min

                if (minHeap.Count == 0) continue;

                // Recompute cost of all vertices incident to current min cost vertes (assuming no parallel edges).
                // This is the delete-insert combo on the heap ~O(m*log n).
                foreach (Edge1 edge in _edges.Where(e => e.U == currMin.Value || e.V == currMin.Value))
                {
                    // Skip the edge already in X
                    if (_X.Exists(x => x == edge.V) && _X.Exists(x => x == edge.U))
                        continue;

                    // Update the costs of all adjacent vertices to v (in V-X)
                    if (edge.U == currMin.Value)
                    {
                        // V is the vertex we want to update with Dijkstra's greedy score
                        minHeap.DecreaseKey(new KeyValuePair<int, int>(_A[edge.U] + edge.Cost, edge.V));
                    }
                    else  // So the other edge V of this edge has to be currMin vertex. Check..
                    {
                        // U is the vertex we want to update
                        minHeap.DecreaseKey(new KeyValuePair<int, int>(_A[edge.V] + edge.Cost, edge.U));
                    }
                }
            }

            // A couple of customizations for use with Johnson's.
            // Will return just the (shortest) of all u_v pair distance.
            List<KeyValuePair<int, int>> minPaths = new List<KeyValuePair<int, int>>();
            KeyValuePair<int, int> currentMin = new KeyValuePair<int, int>();
            int minimumDistance = 1000000;
            // First calculate the minimum distance
            foreach (KeyValuePair<int, int> vertex in _vertices)    // O(n)
            {
                //output.Append(String.Format(" Distance to {0} is: {1}", k.ToString(), _A[k]));
                //output.AppendLine();
                int v = vertex.Value;       // current vertex v E V(G)

                if (_A[v] < minimumDistance)  // && k != sourceVertex
                {
                    minimumDistance = _A[v];
                }

            }

            // Populate all the minimum paths (multiple with the same minimum distance) 
            // as d(u) will be computed at Johnson's for
            // each of these d'(u) path distances
            foreach (KeyValuePair<int, int> kv in _vertices)
            {
                //output.Append(String.Format(" Distance to {0} is: {1}", k.ToString(), _A[k]));
                //output.AppendLine();
                int k = kv.Value;
                if (_A[k] == minimumDistance)  // && k != sourceVertex
                {
                    currentMin = new KeyValuePair<int, int>(k, _A[k]);
                    minPaths.Add(currentMin);
                }
            }
            //File.WriteAllText("D:\\output.txt", output.ToString());
            return minPaths;
        }

        public Dijkstras(List<Edge1> edges)
        {
            _edges = edges;
            _vertices = SetVerticesForHeap(edges);
        }

        public Dijkstras(List<Edge1> edges, List<KeyValuePair<int, int>> vertices)
        {
            _edges = edges;
            _vertices = vertices;
        }

        /// <summary>
        /// Creates list of vertices with the respective cost(infinity)
        /// for use with the Priority Queue.
        /// </summary>
        /// <param name="edges"></param>
        private List<KeyValuePair<int, int>> SetVerticesForHeap(List<Edge1> edges)
        {
            // Create heap data (vertices with cost)
            List<int> vertices = EdgeBuilder.ExtractVertices(edges);

            // Assign 'infinite' cost to the vertices.
            List<KeyValuePair<int, int>> verticesWithCost = new List<KeyValuePair<int, int>>();
            foreach (int vertex in vertices)
            {
                verticesWithCost.Add(new KeyValuePair<int, int>(Infinity, vertex));
            }

            return verticesWithCost;
        }
    }



    // -- Area commented to customize for execution from Johnson's APSP algo impln. --
    //    private static IDictionary<int, List<Edge>> _graph;
    //    private static IList<int> _X;

    //    public static void InitializeDirectGraph(IDictionary<int, List<Edge>> graph)
    //    {
    //        _graph = graph;
    //    }

    //    public static List<KeyValuePair<int, int>> ComputeShortestPath(int sourceVertex)    //string filePath
    //    {
    //        
    //        //// Load graph
    //        //string[] lines = File.ReadAllLines(filePath);
    //        //_graph = new Dictionary<int, List<string>>();

    //        //// Build graph
    //        //foreach (string line in lines)
    //        //{
    //        //    string[] content = line.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
    //        //    _graph.Add(Int32.Parse(content[0]), new List<string>());
    //        //    for (int i = 1; i < content.Length; i++)
    //        //    {
    //        //        _graph[Int32.Parse(content[0])].Add(content[i]);
    //        //    }
    //        //}

    //        // Initialize arrays A & X
    //        int[] _A = new int[_graph.Keys.Count + 1];
    //        _X = new List<int>();
    //        Dictionary<int, int> currentScores = new Dictionary<int, int>();

    //        // Start from vertex S
    //        _X.Add(sourceVertex);
    //        _A[sourceVertex] = 0; // distance from S to itself

    //        // Initialize the main loop
    //        while (_X.Count != _graph.Keys.Count)
    //        {
    //            // foreach vertex in X, find edge not in X and choose one
    //            // with lowest score A[w].

    //            // now check-out all the edges from each vertex in X to V-X
    //            foreach (int vertex in _X)
    //            {
    //                foreach (Edge edge in _graph[vertex])
    //                {
    //                    int w = edge.V;
    //                    int lenW = edge.Cost;

    //                    // Edge should not be in X
    //                    if (!_X.Contains(w))
    //                    {
    //                        int lenVW = _A[vertex] + lenW;

    //                        // Update current edge scores (vw)
    //                        if (currentScores.Keys.Contains(w))
    //                        {
    //                            // Update if current lenVW is smaller
    //                            if (currentScores[w] > lenVW)
    //                            {
    //                                currentScores[w] = lenVW;
    //                            }
    //                        }
    //                        else
    //                        {
    //                            currentScores.Add(w, lenVW);
    //                        }
    //                    }
    //                }
    //                // Select the edge with minimum Dijkstra's greedy cost/score and add to X
    //                if (currentScores.Count == 0)
    //                {
    //                    break;
    //                }
    //                int minScore = currentScores.Min(pair => pair.Value);
    //                int minScoreEdge = currentScores.Where(p => p.Value == minScore).Select(p => p.Key).First();
    //                _A[minScoreEdge] = minScore; // Distance from S to each vertex
    //                _X.Add(minScoreEdge);
    //                currentScores.Clear();
    //            }
    //        }
    //        // Set the unvisited edges to infinity
    //        foreach (int v in _graph.Keys)
    //        {
    //            if (!_X.Contains(v))
    //            {
    //                _A[v] = 1000000;
    //            }
    //        }

    //        //StringBuilder output = new StringBuilder();
    //        // A couple of customizations for use with Johnson's
    //        List<KeyValuePair<int, int>> minPaths = new List<KeyValuePair<int, int>>();
    //        KeyValuePair<int, int> currentMin = new KeyValuePair<int, int>();
    //        int minimumDistance = 1000000;
    //        foreach (int k in _graph.Keys.Where(x => x != sourceVertex))
    //        {
    //            //output.Append(String.Format(" Distance to {0} is: {1}", k.ToString(), _A[k]));
    //            //output.AppendLine();
    //            if (_A[k] < minimumDistance)  // && k != sourceVertex
    //            {
    //                minimumDistance = _A[k];
    //            }

    //        }
    //        // Populate all the minimum paths as d(u) will be computed at Johnson's for
    //        // each of these d'(u) path distances
    //        foreach (int k in _graph.Keys.Where(x => x != sourceVertex))
    //        {
    //            //output.Append(String.Format(" Distance to {0} is: {1}", k.ToString(), _A[k]));
    //            //output.AppendLine();
    //            if (_A[k] == minimumDistance)  // && k != sourceVertex
    //            {
    //                currentMin = new KeyValuePair<int, int>(k, _A[k]);
    //                minPaths.Add(currentMin);
    //            }
    //        }
    //        //File.WriteAllText("D:\\output.txt", output.ToString());
    //        return minPaths;
    //    }
    //}
}

