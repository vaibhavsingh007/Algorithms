using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    /// <summary>
    /// Finds max clustering on the big (complete) graph with hamming distance
    /// between the vertices <= 2.
    /// </summary>
    public class ClusteringBig
    {
        private UnionFind _uf;

        public int Execute(string filepath)
        {
            int maxVertex = 199999;

            // Sample dict format <234123, "1001010010101001001~23,779, ..">
            Dictionary<int, string> graph = BuildGraph(filepath);

            // Will initialize Union Find to max vertex as 0-199999 already known
            _uf = new UnionFind(maxVertex);

            GetMaxClustersPossible(graph);

            // At the end, simply count the UF components
            return _uf.Components;
        }

        private void GetMaxClustersPossible(Dictionary<int, string> graph)
        {
            List<int> candidatePartners = null;
            List<int> validPartners = null;
            foreach (KeyValuePair<int, string> graphNode in graph)  // O(nk)
            {
                int decimalKey = graphNode.Key;
                int node;   // valid partners will be added to this cluster

                // Handle (duplicates) if node already has partners with Hamming Distance = 0 
                // i.e. more than 1 nodes mapped to the same bucket.
                if (graph[decimalKey].Contains(','))
                {
                    string[] hd_0_partners = graph[decimalKey].Split('~')[1].Split(',');
                    node = int.Parse(hd_0_partners[0]);
                    for (int i = 1; i < hd_0_partners.Length; i++)      // O(k) : k = # hd 0 nodes
                    {
                        AddToUnion(node, int.Parse(hd_0_partners[i]));
                    }
                }
                else
                {
                    node = int.Parse(graph[decimalKey].Split('~')[1]);
                }

                // Get all the 300 (24C2 + 24C1) partner nodes with Hamming distance 1 and 2
                candidatePartners = GetAllPartners(graphNode);

                // Check if partner present in graph
                validPartners = ExtractNodesForPresentPartners(candidatePartners, graph);

                // Add to UF
                for (int i = 0; i < validPartners.Count; i++)
                {
                    AddToUnion(node, validPartners[i]);
                }
            }
        }

        /// <summary>
        /// TODO: This can be further optimized. Can work with integers and 
        /// bitshifting for the combinatorics.
        /// </summary>
        /// <param name="graphNode"></param>
        /// <returns></returns>
        private List<int> GetAllPartners(KeyValuePair<int, string> graphNode)
        {
            // Using combinatorics, select (24C2 + 24C1) possibilities with HD <= 2 (excluding 0)
            int bitLength = 24;
            List<int> retval = new List<int>();
            string[] originalBits = graphNode.Value.Split('~')[0].Split(' ');
            string[] temp = new string[originalBits.Length];
            originalBits.CopyTo(temp, 0);
            string currBitString = String.Empty;

            // Create combinations of bitstrings with HD = 2
            for (int i = 0; i < bitLength - 1; i++)
            {
                temp[i] = temp[i] == "1" ? "0" : "1";

                // HD = 1 can be captured here
                currBitString = String.Join(" ", temp);
                retval.Add(GetDecimal(currBitString));

                // Now, for HD = 2
                for (int j = i + 1; j < bitLength; j++)
                {
                    temp[j] = temp[j] == "1" ? "0" : "1";
                    currBitString = String.Join(" ", temp);
                    retval.Add(GetDecimal(currBitString));

                    temp[j] = temp[j] == "1" ? "0" : "1";
                }
                temp[i] = temp[i] == "1" ? "0" : "1";
            }

            // There's one last HD = 1 capture remaining as the outer loop ran just one less
            temp[bitLength - 1] = temp[bitLength - 1] == "1" ? "0" : "1";
            currBitString = String.Join(" ", temp);
            retval.Add(GetDecimal(currBitString));

            return retval;
        }

        private List<int> ExtractNodesForPresentPartners(List<int> partners, Dictionary<int, string> graph)
        {
            List<int> retval = new List<int>();
            string value = String.Empty;
            foreach (int partner in partners)
            {
                if (graph.TryGetValue(partner, out value))
                {
                    string[] nodes = value.Split('~')[1].Split(',');

                    for (int i = 0; i < nodes.Length; i++)
                    {
                        retval.Add(int.Parse(nodes[i]));
                    }
                }
            }
            return retval;
        }

        private void AddToUnion(int u, int v)
        {
            if (_uf.Find(u) != _uf.Find(v))
            {
                _uf.Union(u, v);
            }
        }

        private Dictionary<int, string> BuildGraph(string filePath)
        {
            Dictionary<int, string> retval = new Dictionary<int, string>();
            string[] lines = File.ReadAllLines(filePath);
            int node = 0;
            foreach (string line in lines)
            {
                int decimalEquivalent = GetDecimal(line);
                if (retval.ContainsKey(decimalEquivalent))
                {
                    retval[decimalEquivalent] += "," + node;
                }
                else
                {
                    retval.Add(decimalEquivalent, String.Format("{0}~{1}", line.Trim(), node));
                }
                node++;
            }
            return retval;
        }

        private int GetDecimal(string bitString)
        {
            bitString = bitString.Replace(" ", "").Trim();
            return Convert.ToInt32(bitString, 2);
        }
    }

}
