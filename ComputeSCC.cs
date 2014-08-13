using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    // Kosaraju's 2-pass graph SCC computing algorithm
    public static class ComputeSCC
    {
        // for finishing time in 1st pass
        private static int _t = 0;
        private static int _currentSource;
        private static List<int> _exploredVertices;
        private static IDictionary<int, List<int>> _leader;
        private static IDictionary<int, int> _finishingTime;

        public static void DfsLoop(string pathToGraph)
        {
            // fill the graph in dict(G)
            string[] lines = File.ReadAllLines(pathToGraph);
            IDictionary<int, List<int>> dictG = new Dictionary<int, List<int>>();
            IDictionary<int, List<int>> dictGrev = new Dictionary<int, List<int>>();
            string[] split;

            _leader = new Dictionary<int, List<int>>();
            _finishingTime = new Dictionary<int, int>();
            _exploredVertices = new List<int>();


            foreach (string line in lines)
            {
                split = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                int vertex = Int32.Parse(split[0]);
                int edge = Int32.Parse(split[1]);

                // generate dict(G rev)
                if (dictGrev.Keys.Contains(edge))
                {
                    dictGrev[edge].Add(vertex);
                }
                else
                {
                    dictGrev.Add(edge, new List<int>() { vertex });
                }
            }


            // Start 1st phase on G rev
            // from n to 1
            foreach (int vert in dictGrev.Keys.OrderByDescending(k => k))
            {
                // if vertex not yet explored
                if (!_exploredVertices.Contains(vert))
                {
                    _currentSource = vert;
                    DFS(dictGrev, vert);
                }
            }

            // generate original graph, replace nodes with respective finishing time
            foreach (string line in lines)
            {
                split = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                int vertex = Int32.Parse(split[0]);
                int edge = Int32.Parse(split[1]);
                int thisVertexFinishingTime = _finishingTime[vertex];
                int thisEdgeFinishingTime = _finishingTime[edge];

                if (dictG.Keys.Contains(thisVertexFinishingTime))
                {
                    dictG[thisVertexFinishingTime].Add(thisEdgeFinishingTime);
                }
                else
                {
                    dictG.Add(thisVertexFinishingTime, new List<int>() { thisEdgeFinishingTime });
                }
            }

            // Start 2nd phase on G after replacing the nodes with their
            // corresponding finishing time (the 1st phase helped us to now start from the sink SCC -subtle)
            _leader.Clear();
            _exploredVertices.Clear();
            foreach (int vert in dictG.Keys.OrderByDescending(k => k))
            {

                // if vertex not yet explored
                if (!_exploredVertices.Contains(vert))
                {
                    _currentSource = vert;
                    DFS(dictG, vert);
                }
            }
            string counts = String.Empty;
            foreach (int key in _leader.Keys)
            {
                counts += _leader[key].Count.ToString() + ",";
            }
            File.WriteAllText(@"D:\outPut.txt", counts);
        }

        #region Iterative DFS using stack
        public static void DFS(IDictionary<int, List<int>> graph, int i)
        {
            Stack<int> stack1 = new Stack<int>();
            //Stack<int> timeStack = new Stack<int>();
            List<int> result = new List<int>();
            bool unvisitedNeighbour;
            // mark i as explored

            // seed
            stack1.Push(i);

            while (stack1.Count > 0)
            {
                unvisitedNeighbour = false;
                int v = stack1.Peek();

                // mark v as visited
                if (!_exploredVertices.Contains(v))
                {
                    _exploredVertices.Add(v);
                }
                // set leader for i as the current source node
                if (_leader.Keys.Contains(_currentSource))
                {
                    if (!_leader[_currentSource].Contains(v))
                    {
                        _leader[_currentSource].Add(v);
                    }
                }
                else
                {
                    _leader.Add(_currentSource, new List<int>() { v });
                }

                // foreach arc (i,j) ? G, if j not explored, recurse
                if (graph.Keys.Contains(v))
                {
                    foreach (int j in graph[v])
                    {
                        // if v has an unvisited neighbour, push the first neighbour in stack
                        if (!_exploredVertices.Contains(j))
                        {
                            stack1.Push(j);
                            unvisitedNeighbour = true;
                            break;
                        }
                    }
                }
                if (!unvisitedNeighbour)
                {
                    // pop v and add to result
                    if (!result.Contains(v))
                    {
                        result.Add(stack1.Pop());
                    }

                }


            }

            foreach (int v in result)
            {
                //int v = timeStack.Pop();
                _t++;

                if (!_finishingTime.Keys.Contains(v))
                {
                    // set finishing time for i
                    _finishingTime.Add(v, _t);
                }
            }
        }

        #endregion

        //public static void DFS(IDictionary<int, List<int>> graph, int i)
        //{
        //    try
        //    {
        //        // mark i as explored
        //        _exploredVertices.Add(i);
        //        // set leader for i as the current source node
        //        if (_leader.Keys.Contains(_currentSource))
        //        {
        //            if (!_leader[_currentSource].Contains(i))
        //            {
        //                _leader[_currentSource].Add(i);
        //            }
        //        }
        //        else
        //        {
        //            _leader.Add(_currentSource, new List<int>() { i });
        //        }

        //        // foreach arc (i,j) ? G, if j not explored, recurse
        //        if (graph.Keys.Contains(i))
        //        {
        //            foreach (int j in graph[i])
        //            {
        //                if (!_exploredVertices.Contains(j))
        //                {
        //                    DFS(graph, j);
        //                }
        //            }
        //        }

        //        _t++;

        //        if (!_finishingTime.Keys.Contains(i))
        //        {
        //            // set finishing time for i
        //            _finishingTime.Add(i, _t);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}
    }
}
