using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    /// <summary>
    /// Also an implementation of Merge Sort with Counting split inversions
    /// </summary>
    public static class CountInversion
    {
        public static long SortAndCount(ref List<long> mergedList)
        {
            int length = mergedList.Count;
            if (length == 1)
            {
                return 0;
            }
            else
            {
                List<long> left;
                List<long> right;

                left = new List<long>();
                right = new List<long>();
                long x, y, z;

                float half = length / 2;

                int m = (int)Math.Floor(half);

                for (int i = 0; i < m; i++)
                {
                    left.Add(mergedList[i]);
                }
                for (int j = m; j < length; j++)
                {
                    right.Add(mergedList[j]);
                }

                x = SortAndCount(ref left);
                y = SortAndCount(ref right);
                //mergedList.Clear();
                z = MergeAndCountSplitInv(left, right, ref mergedList);

                return (x + y + z);
            }
        }

        private static long MergeAndCountSplitInv(List<long> l, List<long> r, ref List<long> mergedList)
        {
            object locker = new object();
            long count = 0;
            int i = 0;
            int j = 0;
            int t;

            while (i < l.Count || j < r.Count)
            {
                if (i == l.Count)
                {
                    mergedList[i + j] = r[j];
                    j++;
                }
                else if (j == r.Count)
                {
                    mergedList[i + j] = l[i];
                    i++;
                }
                else if (l[i] <= r[j])
                {
                    mergedList[i + j] = l[i];
                    i++;
                }
                else
                {
                    mergedList[i + j] = r[j];
                    count += l.Count - i;
                    j++;
                }
            }
            return count;


            //int len = (l.Count + r.Count);
            //if (len == 100000)
            //{
            //    count = 0;
            //}
            //lock (locker)
            //{
            //    for (int k = 0; k < len; k++)
            //    {
            //        if ((i != l.Count) && (j != r.Count))
            //        {
            //            if (l[i] < r[j])
            //            {
            //                mergedList.Add(l[i]);
            //                i++;
            //            }
            //            else if (r[j] < l[i])
            //            {
            //                mergedList.Add(r[j]);
            //                count += l.Count - i;
            //                j++;
            //            }
            //        }
            //    }

            //    // Copy the remianing list to mergedList
            //    if (j < i)
            //    {
            //        for (t = j; t < r.Count; t++)
            //        {
            //            mergedList.Add(r[t]);
            //        }
            //    }
            //    else
            //    {
            //        for (t = i; t < l.Count; t++)
            //        {
            //            mergedList.Add(l[t]);
            //        }
            //    }
            //}

            //return count;
        }
    }
}
