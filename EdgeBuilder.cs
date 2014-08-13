using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public static class EdgeBuilder
    {
        /// <summary>
        /// Builds edges from the file path supplied.
        /// The format of the text file should be based on the used convention.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<Edge1> BuildEdges(string filePath)
        {
            List<Edge1> edges = new List<Edge1>();

            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] content = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                edges.Add(new Edge1(Int32.Parse(content[0]), Int32.Parse(content[1]), Int32.Parse(content[2])));
            }

            return edges;
        }

        /// <summary>
        /// Returns union (distinct) of vertices from the give edges.
        /// </summary>
        /// <param name="edges"></param>
        /// <returns></returns>
        public static List<int> ExtractVertices(List<Edge1> edges)
        {
            HashSet<int> vertices = new HashSet<int>();

            foreach (Edge1 edge in edges)   // O(m)
            {
                if (!vertices.Contains(edge.U))
                    vertices.Add(edge.U);
                if (!vertices.Contains(edge.V))
                    vertices.Add(edge.V);
            }
            return vertices.ToList();
        }
    }
}
