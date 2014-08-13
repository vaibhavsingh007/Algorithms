using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    /// <summary>
    /// - GREEDY TECHNIQUE -
    /// Minimize weighted sum of completion times in a job
    /// scheduling problem.
    /// Greedy criterion: Decreasing order of the difference (weight - length)
    /// Also implemented using w/l ratio
    /// </summary>
    public class JobScheduler
    {

        private static List<Job> _ties;
        /// <summary>
        /// Will try to sort the jobs in ascending order of schedule - in place
        /// using Quick Sort.
        /// </summary>
        /// <param name="jobs"></param>
        public double ScheduleJobs(string filePath) //IEnumerable<Job> jobs
        {
            // Load graph
            string[] lines = File.ReadAllLines(filePath);
            List<Job> jobsToSchedule = new List<Job>();
            _ties = new List<Job>();

            foreach (string line in lines)
            {
                string[] content = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                jobsToSchedule.Add(new Job(float.Parse(content[0]), float.Parse(content[1])));
            }

            double weightedSum = 0;        // This variable contians the final answer!!!
            double completionTimeAggregate = 0;
            //float previousRat = 0;      // Recording ties (just for debugging)
            //List<Job> jobsToSchedule = new List<Job>(new Job[] {
            //    new Job(74, 59), new Job(31, 73), new Job(45, 79), new Job(90, 75)});
            PerformQuickSort(jobsToSchedule, 0, jobsToSchedule.Count() - 1);

            // Once the jobs are sorted, traverse from the back to schedule in the
            // order of decreasing weights.
            for (int i = jobsToSchedule.Count - 1; i >= 0; i--)
            {
                completionTimeAggregate += jobsToSchedule[i].L; // Completion time/length of current job (decreasing order)
                double currentCost = completionTimeAggregate * jobsToSchedule[i].W;
                weightedSum += currentCost;

                //if (jobsToSchedule[i].Diff == previousRat)    // Uncomment to record ties
                //{
                //    _ties.Add(jobsToSchedule[i]);
                //}
                //previousRat = jobsToSchedule[i].Diff;
            }
            return weightedSum;
        }

        private static void PerformQuickSort(List<Job> array, int l, int r)
        {
            //if ((r - l) == 0) { return; }
            if (l < r)
            {
                int pivotIndex = Partition(array, l, r);
                PerformQuickSort(array, l, (pivotIndex - 1));
                PerformQuickSort(array, pivotIndex + 1, r);
            }
        }

        private static int Partition(List<Job> array, int l, int r)
        {
            Job p = array[r];
            int i = l;
            Job temp;

            for (int j = l; j <= r - 1; j++)
            {
                // Second condition to break ties scheduling the job with higher 
                // weight first (array will be finally in ascending order and should be traversed
                // from the last to schedule in decreasing order of diffs.
                if (array[j].Diff < p.Diff || ((array[j].Diff == p.Diff) && (array[j].W < p.W)))
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

    /// <summary>
    /// Switch between diff and ratio as required.
    /// Adjust Quicksort accordingly.
    /// </summary>
    public struct Job
    {
        public float L;
        public float W;
        public float Diff;
        //public float Ratio;
        public Job(float weight, float length)
        {
            this.W = weight;
            this.L = length;
            this.Diff = weight - length;
        }
    }
}
