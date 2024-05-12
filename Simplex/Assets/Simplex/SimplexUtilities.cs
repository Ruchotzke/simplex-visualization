using System.Collections.Generic;
using System.Linq;

namespace Simplex
{
    public class SimplexUtilities
    {
        public static List<(List<int> B, List<int> N)> GenerateAllPartitions(int numVars, int numConstraints)
        {
            /* First get the upper size, including slack variables */
            int max = numVars + numConstraints;
            
            /* Generate all combinations where len(B) = m and len(N) = n */
            /* Generate basic partitions */
            var bs = Combinations(max, numConstraints);
            
            /* Now generate the final output list */
            List<(List<int> B, List<int> N)> output = new List<(List<int> B, List<int> N)>();
            foreach (var basic in bs)
            {
                List<int> nonBasic = new List<int>();
                for (int i = 0; i < max; i++)
                {
                    if (!basic.Contains(i)) nonBasic.Add(i);
                }
                output.Add((basic, nonBasic));
            }

            return output;
        }

        static List<List<int>> Combinations(int maxNum, int size)
        {
            List<List<int>> result = new List<List<int>>();
        
            void Backtrack(int start, List<int> currComb)
            {
                if (currComb.Count == size)
                {
                    var sortedComb = currComb.OrderBy(x => x).ToList();
                    if (!result.Any(c => c.SequenceEqual(sortedComb))) // Check for duplicates
                    {
                        result.Add(sortedComb);
                    }
                    return;
                }
                for (int i = start; i <= maxNum; i++)
                {
                    currComb.Add(i);
                    Backtrack(i + 1, currComb);
                    currComb.RemoveAt(currComb.Count - 1);
                }
            }

            Backtrack(1, new List<int>());
            return result;
        }
    }
}