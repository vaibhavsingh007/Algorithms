using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public static class TwoSumProblem
    {
        public static void Execute(string inputFile)
        {
            int count = 0;
            string[] lines = File.ReadAllLines(inputFile);
            HashSet<int> hashSet = new HashSet<int>();
            StringBuilder sb = new StringBuilder();

            // Load the file in the Hashtable
            foreach (string line in lines)
            {
                hashSet.Add(Int32.Parse(line));
            }

            // now, foreach x in hashset, find t-x so their sum lies in the range 2500-4000

            for (int t = 2500; t <= 4000; t++)
            {
                foreach (int x in hashSet)
                {

                    //if (t < x) continue;
                    int y = t - x;
                    if (hashSet.Contains(y) && y != x)    //  
                    {
                        // found distinct x+y = t, mark both visited
                        
                        count++;

                        //sb.Append(String.Format("{0} + {1} = {2}", x, y, t));
                        //sb.Append(Environment.NewLine);
                        break;
                    }
                }
            
            }
            StreamWriter writer = new StreamWriter("D:\\output.txt");
            writer.WriteLine("Count = " + count);
            writer.WriteLine(sb.ToString());
            writer.Dispose();
        }
    }
}
