using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            List<int> options = new List<int>();
            for (int i = 0; i < max; i++) options.Add(i);
            var bs = ListChooseN(options, numConstraints);
            
            /* Now generate the final output list */
            List<(List<int> B, List<int> N)> output = new List<(List<int> B, List<int> N)>();
            foreach (var basic in bs)
            {
                // Debug.Log("Basic: " + string.Join(" ", basic));
                List<int> nonBasic = new List<int>();
                for (int i = 0; i < max; i++)
                {
                    if (!basic.Contains(i)) nonBasic.Add(i);
                }
                output.Add((basic, nonBasic));
                // Debug.Log("Non Basic: " + string.Join(" ", nonBasic));
            }

            return output;
        }

        static List<List<int>> ListChooseN(List<int> choices, int choose)
        {
            List<List<int>> combinations = new List<List<int>>();
            
            /* Base case: choose = 1. Return all items individually */
            if (choose == 1)
            {
                foreach (var choice in choices)
                {
                    combinations.Add(new List<int>(){choice});
                }

                return combinations;
            }

            /* Recursive case: generate and merge all choices for this array */
            foreach (var element in choices)
            {
                /* Generate a new choice list */
                List<int> newChoices = new List<int>();
                foreach (var item in choices)
                {
                    if (item != element) newChoices.Add(item);
                }
                
                /* Get all arrays starting with this element */
                List<List<int>> ends = ListChooseN(newChoices, choose - 1);
                
                /* Merge, only keeping sorted arrays to maintain no duplicates */
                foreach (var list in ends)
                {
                    /* make sure the first element is greater than our element */
                    if (list[0] < element) continue;
                    
                    /* append into a list */
                    list.Insert(0, element);
                    
                    /* Store this */
                    combinations.Add(list);
                }
            }
            
            /* Return the completed lists */
            return combinations;
        }
    }
}