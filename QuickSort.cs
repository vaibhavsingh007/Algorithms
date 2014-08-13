using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public static class QuickSort
    {
        public static int GetComparisons 
        { 
            get 
            {
                return comparisons;
            }
            
        }
        private static int comparisons = 0;
        public static void PerformQuickSort(List<int> array, int l, int r)
        {
            //if ((r - l) == 0) { return; }
            if (l < r)
            {
                int pivotIndex = Partition(array, l, r);
                PerformQuickSort(array, l, (pivotIndex - 1));
                PerformQuickSort(array, pivotIndex + 1, r);
            }
        }

        private static int Partition(List<int> array, int l, int r)
        {
            int p = array[r];
            int length = ((r - l) + 1);
            int i = l;
            int temp;

            // add comparisons: pivot compared only once with all other elements in array
            comparisons += (length - 1);

            for (int j = l; j <= r-1; j++)
            {
                if (array[j] < p)
                {
                    // swap j with i, increment i & j
                    temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                    i++;
                }
            }
            // position pivot
            temp = array[r];
            array[r] = array[i];
            array[i] = temp;

            // return pivot's index
            return (i);
        }
    }
}
