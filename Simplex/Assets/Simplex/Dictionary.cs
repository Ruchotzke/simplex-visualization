using System;
using System.Collections;
using System.Collections.Generic;
using Matrices;
using UnityEngine;

namespace Simplex
{
    /// <summary>
    /// A simplex dictionary.
    /// </summary>
    public class Dictionary
    {
        public float Zeta;
        public Matrix BasicVarValues;

        public Matrix ZetaNonBasicVars;
        public Matrix NonBasicVarCoeff;

        public List<int> Basic;
        public List<int> NonBasic;

        public string Message;
        public bool IsValid = false;
        public bool IsBasic = false;
        public bool IsFeasible = false;
        public bool IsOptimal = false;
        public bool IsUnbounded = false;
        
        /// <summary>
        /// Generate a new simplex dictionary.
        /// </summary>
        /// <param name="A">The coefficient matrix.</param>
        /// <param name="b">The bounds for the constraints.</param>
        /// <param name="c">The objective function.</param>
        /// <param name="B">The basic partition.</param>
        /// <param name="N">The non-basic partition.</param>
        public Dictionary(Matrix A, Matrix b, Matrix c, List<int> B, List<int> N)
        {
            /* Copy basic/non-basic */
            Basic = new List<int>();
            NonBasic = new List<int>();
            foreach (var entry in B) Basic.Add(entry);
            foreach (var entry in N) NonBasic.Add(entry);
            
            /* Generate the dictionary */
            ProcessDictionary(A, b, c);
        }

        /// <summary>
        /// A helper function used to generate the dictionary.
        /// </summary>
        /// <param name="A">The coefficient matrix.</param>
        /// <param name="b">The bounds for the constraints.</param>
        /// <param name="c">The objective function.</param>
        private void ProcessDictionary(Matrix A, Matrix b, Matrix c)
        {
            /* Generate variables with slack */
            uint m = A.Size.rows;
            uint n = A.Size.cols;
            var aBar = A.ComposeHorizontal(Matrix.Identity(A.Size.rows));
            var cBar = c.ComposeVertical(new Matrix((m, 1)));
            
            /* Check partitions */
            if (Basic.Count != m)
            {
                Message = "Error. B partition expected " + m + " elements but got " + Basic.Count;
                return;
            }
            if (NonBasic.Count != n)
            {
                Message = "Error. N partition expected " + n + " elements but got " + NonBasic.Count;
                return;
            }
            for (int i = 0; i < m + n; i++)
            {
                if (!(Basic.Contains(i) || NonBasic.Contains(i)))
                {
                    Message = "Error. Missing partition element " + i;
                    return;
                }
            }

            IsValid = true; //we have passed initial checks
            
            /* Attempt to invert. A failure implies this is not basic */
            Matrix invA = null;
            try
            {
                invA = aBar.SelectColumns(Basic).Inverse();
            }
            catch (Exception e)
            {
                Message = "Partition is not basic, A cannot be inverted.";
                return;
            }

            IsBasic = true;
            
            /* At this point, we can compute the dictionary */
            Zeta = (cBar.SelectRows(Basic).Transpose() * invA * b)[0,0];
            BasicVarValues = invA * b;
            ZetaNonBasicVars = cBar.SelectRows(NonBasic).Transpose() - (cBar.SelectRows(Basic).Transpose() * invA * aBar.SelectColumns(NonBasic));
            NonBasicVarCoeff = invA * aBar.Scale(-1.0f).SelectColumns(NonBasic);
            
            /* Feasibility: are all basic variables zero or greater? */
            for (uint r = 0; r < BasicVarValues.Size.rows; r++)
            {
                if (BasicVarValues[r, 0] < 0.0f)
                {
                    Message = "Partition is not feasible, negative basic variable value.";
                    return;
                }
            }

            IsFeasible = true;
            
            /* Optimality: Are all non-basic coeffs in zeta negative? */
            bool positive = false;
            for (uint col = 0; col < ZetaNonBasicVars.Size.cols; col++)
            {
                if (ZetaNonBasicVars[0, col] > 0)
                {
                    positive = true;
                    break;
                }
            }

            IsOptimal = !positive;
            
            /* Unboundedness: If we aren't optimal, check coefficients for unboundedness */
            if (!IsOptimal)
            {
                for (uint col = 0; col < ZetaNonBasicVars.Size.cols; col++)
                {
                    if (ZetaNonBasicVars[0, col] > 0)
                    {
                        /* Check whole column */
                        bool negative = false;
                        for (uint row = 0; row < NonBasicVarCoeff.Size.rows; row++)
                        {
                            if (NonBasicVarCoeff[row, col] <= 0)
                            {
                                negative = true;
                                break;
                            }
                        }

                        if (!negative)
                        {
                            IsUnbounded = true;
                            break;
                        }
                    }
                }
                
                /* If unbounded, perform final analysis */
                if (IsUnbounded)
                {
                    /* TODO */
                }
            }
        }
    }
}

