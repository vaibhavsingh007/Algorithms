using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    /// <summary>
    /// Implements the 0/1 Knapsack Algorithm using Dynamic Programming.
    /// Time complexity: O(nW)
    /// Space complexity: O(W)  <-- Using optimization strategy top-down 1-D knapsack.
    /// </summary>
    public class Knapsack
    {
        public int Run(string filepath)
        {
            // Use a simple 2 column 2-d array to hold the items (value weight)
            int knapsackCapacity = 0;
            int numberOfItems = 0;
            int[,] items = ExtractItems(filepath, out knapsackCapacity, out numberOfItems);

            /* Space-complexity optimization strategy: use 1-D array and perform 
             * in place updates using Top-Down approach.
             * Due to the modified DS, The algorithm is slightly modified as:
             * A[i,x] = max {A[i,x], A[i, x-w(i)] + v(i)}
             * Start with the first item and max weight
             */
            int[] knapsack = new int[knapsackCapacity + 1];
            //x.Initialize();
            try
            {
                for (int i = 0; i < numberOfItems; i++)
                {
                    int currentItemValue = items[i, 0];
                    int currentItemWeight = items[i, 1];
                    for (int x = knapsackCapacity; x >= 0; x--)
                    {
                        // Do the following based on the condition:
                        if (currentItemWeight <= knapsackCapacity)    // Item weight should be less than knapsack capacity.
                        {
                            if (x >= currentItemWeight) // Residual knapsack capacity can accomodate current item.
                            {
                                // Get value of previous item with capacity reduced
                                // ..by current items weight
                                int valueOfPreviousItem = knapsack[x - currentItemWeight];
                                knapsack[x] = Math.Max(knapsack[x], valueOfPreviousItem + currentItemValue);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return knapsack[knapsackCapacity];
        }

        private int[,] ExtractItems(string filePath, out int knapsackCapacity, out int numberOfItems)
        {
            int[,] items;

            string[] lines = File.ReadAllLines(filePath);
            items = new int[lines.Length - 1, 2];

            // Extract knapsack capacity
            string[] firstLine = lines[0].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            knapsackCapacity = Int32.Parse(firstLine[0]);
            numberOfItems = Int32.Parse(firstLine[1]);

            for (int i = 0; i < lines.Length - 1; i++)
            {
                string[] content = lines[i + 1].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                items[i, 0] = Int32.Parse(content[0]);   // Value
                items[i, 1] = Int32.Parse(content[1]);   // Weight
            }
            return items;
        }

    }
}
