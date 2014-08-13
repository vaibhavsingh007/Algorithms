using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Algorithms;
using System.IO;

namespace Algorithms
{
    public static class MedianMaintenance
    {
        public static void Execute(string inputFile)
        {
            MaxPriorityQueue<int, int> hLow = new MaxPriorityQueue<int, int>();
            PriorityQueue<int, int> hHigh = new PriorityQueue<int, int>();
            
            string[] lines = File.ReadAllLines(inputFile);
            int medianSum = 0;

            //hHigh.Enqueue(1, 0);
            //hHigh.Enqueue(2, 0);
            //hHigh.Enqueue(3, 0);
            //hHigh.Enqueue(4, 0);

            //hLow.Enqueue(1, 0);
            //hLow.Enqueue(2, 0);
            //hLow.Enqueue(3, 0);
            //hLow.Enqueue(4, 0);

            // Maintain invariant ~ i/2 smallest(or largest) elements in hLow(or hHigh)
            /* If incoming i > hLow, enqueue in hHigh and vice-versa.
             * if count(hLow) > count(hHigh), Dequeue hLow to hHigh and vice-versa.
             */
            foreach (string line in lines)
            {
                int x = Int32.Parse(line);

                // If hLow is empty, enqueue x
                if (hLow.Count == 0)
                {
                    hLow.Enqueue(x, 0);
                    medianSum += hLow.Peek().Key;
                    continue;
                }

                if (x > hLow.Peek().Key)
                {
                    hHigh.Enqueue(x, 0);
                }
                else
                {
                    hLow.Enqueue(x, 0);
                }

                // Balance heaps
                if (hLow.Count > hHigh.Count)
                {
                    if (hLow.Count - hHigh.Count > 1)
                    {
                        hHigh.Enqueue(hLow.Dequeue().Key, 0);
                    }
                }
                else
                {
                    if (hHigh.Count - hLow.Count > 1)
                    {
                        hLow.Enqueue(hHigh.Dequeue().Key, 0);
                    }
                }
                medianSum += ((hLow.Count + hHigh.Count) % 2 == 0) ? hLow.Peek().Key :
                    (hLow.Count > hHigh.Count) ? hLow.Peek().Key : hHigh.Peek().Key;
            }
        }
    }
}
