using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Algorithms
{
    public class TSP
    {
        #region Fields
        private const int Infinity = 100000;
        private readonly int _numberOfBits;     // Set this to the number of vertices for sets construction
        private readonly int _vertexCount;
        private List<double[]> _graph;
        private List<List<int>> _powerSet;      // The set S of all possible sub-sets of the vertices (2^n).

        private Dictionary<int, double[]> _memoizationDict1;
        private Dictionary<int, double[]> _memoizationDict2;

        private Dictionary<string, int> _setToIntegerMemoizationDict;

        private static object _locker1 = new object();
        private static object _locker2 = new object();

        #endregion

        #region ctor
        public TSP(string filepath, int vertexCount)
        {
            _numberOfBits = vertexCount - 1;  // Because starting vertex (city) excluded for simplicity.
            _vertexCount = vertexCount;
            _graph = ConstructGraphMatrix(filepath);
            _powerSet = ConstructSets();

            _memoizationDict1 = new Dictionary<int, double[]>();
            _memoizationDict2 = new Dictionary<int, double[]>();

            _setToIntegerMemoizationDict = new Dictionary<string, int>();
        }

        #endregion

        #region Public
        public double Execute()
        {
            // As per an innovative optimization strategy, use two dicts to memoize size m and m-1 results.
            // As these are what will ever be needed for the Dynamic Programming.
            // Hope the dicts don't blow up the memory.

            bool memoizationDictSwitchFlag = true;
            Stopwatch stopWatch = new Stopwatch();
            // Initialize the base+1 case to avoid handling in the main TSP loop
            // Below, i represents sets with m = 1 containing (logically) first vertex 0 by default.
            for (int i = 1; i < _vertexCount; i++)
            {
                int setInteger;
                double[] distances = new double[_vertexCount];    // i will be stored in dict and i to j cost in this array.

                setInteger = GetSetEquivalentInteger(new List<int> { i });
                distances[0] = Infinity;
                distances[i] = GetDistance(0, i);
                _memoizationDict1.Add(setInteger, distances);
            }

            stopWatch.Start();

            // Foreach subproblem size m = {2,3,..n} 
            // (First vertex '0' logically included in all sets by default)
            for (int m = 2; m < _vertexCount; m++)
            {
                // Foreach subset S of size m from {1,2,3,..n} excluding vertex 0 
                // ..as cost from 0 to 0 using any set but {0} is infinite anyway.
                Parallel.ForEach(_powerSet[m], (setEquivalentInteger, state, i) =>
                {
                    List<int> currentSubset = GetIntegerEquivalentSet(setEquivalentInteger);
                    double[] optimalDistances = new double[_vertexCount];
                    // Foreach j in subset and j != 0 (first vertex)
                    foreach (int j in currentSubset)
                    {
                        double minCost = Infinity;

                        // A[S,j] = min (k E S) and (k != j) { A[{S}-j,k] + d_kj }
                        // Hence, foreach k in subset where k != j
                        foreach (int k in currentSubset)
                        {
                            if (k == j) continue;

                            // Get {S}-j set equivalent integer and use appropriate memoization dict
                            // ..to fetch A[{S}-j,k] mincost.
                            int S_minus_j_setEquivalentInteger = GetSetEquivalentInteger(currentSubset, j);
                            double S_k_cost = GetMemoizedCostFor(S_minus_j_setEquivalentInteger, k, memoizationDictSwitchFlag);
                            double k_j_cost = GetDistance(k, j);
                            double currentCost = S_k_cost + k_j_cost;

                            if (currentCost < minCost)
                                minCost = currentCost;
                        }
                        // Store current sets (optimal) distance to j
                        optimalDistances[j] = minCost;
                    }
                    Memoize(setEquivalentInteger, optimalDistances, memoizationDictSwitchFlag);
                });
                ClearMemoizationDict(ref memoizationDictSwitchFlag);
            }

            // Return Min of (j=2..n){ A[(1,2,..n), j] + Cj1 }
            double minTspPath = GetFinalMinJ1Cost(memoizationDictSwitchFlag);

            stopWatch.Stop();

            TspExecutionTime = (int)(stopWatch.ElapsedMilliseconds / 1000);

            return minTspPath;
        }

        public int SubsetConstructionTime { get; private set; }
        public int GraphMatrixConstructionTime { get; private set; }
        public int TspExecutionTime { get; private set; }

        #endregion

        #region Private
        private double GetFinalMinJ1Cost(bool flag)
        {
            double retval = Infinity;
            double[] finalDistances;

            // At this moment, the dict should contain just ONE key of subproblem/sets
            // ..size, m = _vertexCount.
            finalDistances = flag ? _memoizationDict1.First().Value : _memoizationDict2.First().Value;

            for (int j = 1; j < _vertexCount; j++)
            {
                double current_min_j_1_cost;   // '1' in the variable name represents 'first' vertex.

                double min_S_j_cost = finalDistances[j];
                double j_1_cost = GetDistance(0, j);
                current_min_j_1_cost = min_S_j_cost + j_1_cost;

                if (current_min_j_1_cost < retval)
                {
                    retval = current_min_j_1_cost;
                }
            }
            return retval;
        }

        /// <summary>
        /// Gets memoized cost for the set and k using the dicts.
        /// </summary>
        /// <returns></returns>
        private double GetMemoizedCostFor(int setInteger, int k, bool flag)
        {
            return flag ? _memoizationDict1[setInteger][k] : _memoizationDict2[setInteger][k];
        }

        /// <summary>
        /// Memoizes current Set's optimal distances to j's.
        /// </summary>
        private void Memoize(int setEquivalentInteger, double[] optimalDistances, bool flag)
        {
            // Any set's optimal path to first vertex (0) is infinite
            optimalDistances[0] = Infinity;

            lock (_locker2)
            {
                if (flag)
                {
                    _memoizationDict2.Add(setEquivalentInteger, optimalDistances);
                }
                else
                {
                    _memoizationDict1.Add(setEquivalentInteger, optimalDistances);
                }
            }
        }

        /// <summary>
        /// Clears (m-1) sized sets memoization dictionary and 
        /// toggles the flag.
        /// Also clears the set to integer memoization dictionary.
        /// </summary>
        /// <param name="flag">memoizationDictSwitchFlag</param>
        private void ClearMemoizationDict(ref bool flag)
        {
            if (flag)
            {
                // Means dict 1 was used to read (m-1) sized sets/subproblems and now its job is done..
                _memoizationDict1.Clear();
            }
            else
            {
                _memoizationDict2.Clear();
            }
            _setToIntegerMemoizationDict.Clear();
            flag = !flag;
        }



        private int GetSetEquivalentInteger(List<int> set, int exclude_j = 0)
        {
            int retval = 0;
            // We know (by the elements in the set) exactly which bits are 'set'.
            // Hence, calculating setInteger is again the simple bit-bashing magic.
            foreach (int element in set.Where(s => s != exclude_j))
            {
                retval += (int)Math.Pow(2, element - 1);
            }
            return retval;
        }

        private List<int> GetIntegerEquivalentSet(int setEquivalentInteger)
        {
            List<int> retval = new List<int>();

            for (int j = 0; j < _numberOfBits; j++)
            {
                if ((setEquivalentInteger & 1 << j) != 0)
                {
                    // Because vertices here are a sequence of n whole numbers starting from 0
                    // ..and we have removed vertex 0 (starting city) for simplicity, simulate
                    // ..vertex selection by adjusting for the removed vertex.
                    retval.Add(j + 1);    // So now you know why j+1. :)
                }
            }
            return retval;
        }

        /// <summary>
        /// Constructs the set equivalent integer bitmap using the
        /// bitbashing magic.
        /// Tested to 13 seconds on 25 bits (cities) using parallelization on core i5.
        /// </summary>
        /// <returns></returns>
        private List<List<int>> ConstructSets()
        {
            List<List<int>> retval = new List<List<int>>();

            //int numberOfBits = 25;
            int power = (int)Math.Pow(2, _numberOfBits);
            int[] A = new int[_numberOfBits];
            for (int i = 0; i < _numberOfBits; i++)
                A[i] = i;

            for (int i = 0; i <= _numberOfBits; i++)
                retval.Add(new List<int>());

            Parallel.For(0, power, i =>
            {
                int bitsSetCounter = 0;
                //Console.WriteLine(String.Format("\nSet # {0} --", i));
                for (int j = 0; j < _numberOfBits; j++)
                {
                    if ((i & 1 << j) != 0)
                    {
                        bitsSetCounter++;
                    }
                }
                lock (_locker1)
                    retval[bitsSetCounter].Add(i);
            });

            return retval;
        }

        /// <summary>
        /// Constructs the lower triangular matrix for the complete-graph.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private List<double[]> ConstructGraphMatrix(string filePath)
        {
            // Need a lower triangular matrix (adjacency list). Check!
            List<double[]> retval = new List<double[]>();

            string[] lines = File.ReadAllLines(filePath);
            string[] coordinateSplits;
            double x1, y1, x2, y2;

            for (int i = 0; i < _vertexCount; i++)
            {
                // Capture distances from i to all other vertices ahead.
                // (i+1) because skipping the diagonal as it will alwas be 0 (cost of a vertex to itself).
                double[] currentDistances = new double[_vertexCount - (i + 1)];
                coordinateSplits = lines[i].Split(' ');
                x1 = double.Parse(coordinateSplits[0]);
                y1 = double.Parse(coordinateSplits[1]);

                // PS: Variable j needs to fill the distance array but the next vertex is actually 
                // ..the ones ahead of i in lines[] (vertices). So j starts from 0.
                for (int j = 0; j < currentDistances.Length; j++)
                {
                    int nextVertex = i + (j + 1);
                    coordinateSplits = lines[nextVertex].Split(' ');
                    x2 = double.Parse(coordinateSplits[0]);
                    y2 = double.Parse(coordinateSplits[1]);

                    // Calculate the Euclidean Distance between vertex co-ordinates.
                    double ijDistance = Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2));
                    currentDistances[j] = ijDistance;
                }
                retval.Add(currentDistances);
            }
            return retval;
        }

        /// <summary>
        /// Returns the distance between two vertices.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private double GetDistance(int i, int j)
        {
            if (i > j)
            {
                int temp = j;
                j = i;
                i = temp;
            }
            else if (i == j)
            {
                return 0;
            }
            return _graph[i][j - (i + 1)];
        }

        #region Old Subset Construction
        ///// <summary>
        ///// State-of-the-art set construction algo
        ///// Creates unique sets of size m from given sequence
        ///// vertices n. Each set containing vertex 0 (first vertex)
        ///// </summary>
        ///// <param name="n"></param>
        ///// <param name="m"></param>
        ///// <returns></returns>
        //public List<int[]> ConstructSets(int n, int m)
        //{
        //    int cI = 0;
        //    List<int[]> S = new List<int[]>();
        //    int[] ss;
        //    ss = new int[m];
        //    for (int i = 0; i < m; i++)
        //    {
        //        ss[i] = i;
        //    }

        //    S.Add(ss);

        //    while (true)
        //    {
        //        cI++;
        //        ss = new int[m];
        //        int i = m - 1;  // i responsible for incrementing just the last index
        //        // newVal essentially denotes new index but it doesn't matter here-
        //        // as the indices are same as values for the vertices will be 0,1,2,..n :)
        //        int newVal = S[cI - 1][i] + 1;

        //        if (newVal < n) // || (i < m - 1 && newVal < n))
        //        {
        //            ss[i] = newVal;
        //            for (int k = i - 1; k >= 0; k--)
        //                ss[k] = S[cI - 1][k];
        //        }
        //        else
        //        {
        //            int bk = 2;     // Adjust the index m. ex: last element in an array is m-1 and second last m-2

        //            while (true)
        //            {
        //                bool skip = false;  // flag to skip last for loop if previous for encounters 'break;'
        //                int bkIndex = m - bk;
        //                if (bkIndex <= 0)
        //                {
        //                    return S;
        //                }
        //                int newVal2 = S[cI - 1][bkIndex] + 1;

        //                if (newVal2 < n - 1)    // || (i < m - 1 && newVal2 < n - 1))
        //                {
        //                    ss[bkIndex] = newVal2;

        //                    for (int b = bkIndex + 1; b < m; b++)
        //                    {
        //                        int currentVal = ss[b - 1] + 1;

        //                        // coz only the last index can be max n-1, and rest n-2
        //                        if ((b == m - 1 && currentVal < n) || (b < m - 1 && currentVal < n - 1))
        //                        {
        //                            ss[b] = currentVal;
        //                        }
        //                        else
        //                        {
        //                            bk++;
        //                            skip = true;
        //                            break;
        //                        }
        //                    }
        //                    // The above for loop populates [b,..,m] and the below [0,..,b-1]
        //                    if (!skip)
        //                    {
        //                        for (int k = bkIndex - 1; k >= 0; k--)
        //                            ss[k] = S[cI - 1][k];
        //                        break;
        //                    }
        //                }
        //                else
        //                    bk++;
        //            }
        //        }
        //        S.Add(ss);
        //    }
        //} 

        #endregion

        #endregion
    }
}
