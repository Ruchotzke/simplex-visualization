using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor.UI;
using UnityEngine;

namespace Matrices
{
    /// <summary>
    /// An arbitrarily sized matrix.
    /// </summary>
    public class Matrix
    {
        public (uint rows, uint cols) Size;

        public float[,] Data;

        /// <summary>
        /// Construct a new blank matrix.
        /// </summary>
        /// <param name="size"></param>
        public Matrix((uint rows, uint cols) size)
        {
            Size = size;
            Data = new float[size.rows, size.cols];
        }

        /// <summary>
        /// Construct a new matrix from a string representation.
        /// </summary>
        /// <param name="str"></param>
        public Matrix(string str)
        {
            string[] fullRows = str.Split(";");
            List<List<float>> data = new List<List<float>>();

            /* Parse the data */
            for(int r = 0; r < fullRows.Length; r++)
            {
                string[] tokens = fullRows[r].Trim().Split(" ");
                if (tokens[0] != "")
                {
                    data.Add(new List<float>());
                    foreach (var token in tokens)
                    {
                        data[r].Add(float.Parse(token.Trim()));
                    }
                }
            }
            
            /* Make sure input is appropriately sized */
            (uint rows, uint cols) size = ((uint)data.Count, (uint)data[0].Count);
            foreach (var row in data)
            {
                if (row.Count != size.cols) throw new Exception("Matrix rows cannot be jagged. Expected: " + size.cols + " Got: " + row.Count);
            }
            
            /* Generate the matrix */
            Size = size;
            Data = new float[size.rows, size.cols];
            
            for (int r = 0; r < size.rows; r++)
            {
                for (int c = 0; c < size.cols; c++)
                {
                    Data[r, c] = data[r][c];
                }
            }
        }

        /// <summary>
        /// Index this matrix.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public float this[uint row, uint col]
        {
            get => Data[row, col];
            set => Data[row, col] = value;
        }

        /// <summary>
        /// Generate a new matrix filled with ones.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Matrix Ones((uint rows, uint cols) size)
        {
            var mat = new Matrix(size);

            for (uint r = 0; r < size.rows; r++)
            {
                for (uint c = 0; c < size.cols; c++)
                {
                    mat[r, c] = 1.0f;
                }
            }

            return mat;
        }
        
        /// <summary>
        /// Create a new matrix filled with zeroes.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Matrix Zeroes((uint rows, uint cols) size)
        {
            var mat = new Matrix(size);

            return mat;
        }

        /// <summary>
        /// Generate a new, square identity matrix.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Matrix Identity(uint size)
        {
            var mat = new Matrix((size, size));

            for (uint i = 0; i < size; i++)
            {
                mat[i, i] = 1.0f;
            }
            
            return mat;
        }

        /// <summary>
        /// Generate a deep copy of this matrix.
        /// </summary>
        /// <returns></returns>
        public Matrix Copy()
        {
            Matrix m = new Matrix(Size);
            
            for (uint r = 0; r < Size.rows; r++)
            {
                for (uint c = 0; c < Size.cols; c++)
                {
                    m[r, c] = Data[r, c];
                }
            }

            return m;
        }

        public override string ToString()
        {
            string ret = "Matrix (" + Size.rows + "," + Size.cols + "): ";
            for (uint row = 0; row < Size.rows; row++)
            {
                for (uint col = 0; col < Size.cols; col++)
                {
                    ret += Data[row, col] + " ";
                }

                ret += "; ";
            }

            return ret;
        }

        /// <summary>
        /// Calculate the upper triangular with 1s in the diagonal.
        /// </summary>
        /// <returns></returns>
        public Matrix UpperTriangularOnes()
        {
            /* Make a copy */
            Matrix m = Copy();

            /* Iterate through each pivot position */
            for (uint pivot = 0; pivot < Size.cols; pivot++)
            {
                /* Calculate the starting row for this pivot */
                uint row = pivot;
                
                /* "fix" this row so the pivot is 1 */
                if (m[row, pivot] != 1.0f)
                {
                    /* Before anything, if the pivot is zero, this matrix is degenerate */
                    if (m[row, pivot] == 0)
                    {
                        throw new Exception("Matrix state is degenerate, cannot finish triangulation.");
                    }
                    float divisor = m[row, pivot];
                    for (uint i = pivot; i < Size.cols; i++)
                    {
                        m[row, i] /= divisor;
                    }
                }
                
                /* Now subtract this from other rows to clear the rest of the matrix */
                for (row = row + 1; row < Size.rows; row++)
                {
                    /* Figure out how much needs to be subtracted */
                    float mult = m[row, pivot];
                    for (uint i = pivot; i < Size.cols; i++)
                    {
                        /* Update all elements */
                        m[row, i] -= mult * m[pivot, i];
                    }
                }
            }

            return m;
        }
        
        /// <summary>
        /// Perform gauss-jordan elimination on this matrix.
        /// Generates an upper-triangular matrix.
        /// </summary>
        /// <returns></returns>
        public Matrix UpperTriangular()
        {
            /* Make a copy */
            Matrix m = Copy();

            /* Iterate through each pivot position */
            for (uint pivot = 0; pivot < Size.cols; pivot++)
            {
                /* Calculate the starting row for this pivot */
                uint row = pivot;
                
                /* Before anything, if the pivot is zero, this matrix is degenerate */
                if (m[row, pivot] == 0)
                {
                    throw new Exception("Matrix state is degenerate, cannot finish triangulation.");
                }
                
                /* Now subtract this from other rows to clear the rest of the matrix */
                for (row += 1; row < Size.rows; row++)
                {
                    /* Figure out how much needs to be subtracted */
                    float mult = m[row, pivot];
                    for (uint i = pivot; i < Size.cols; i++)
                    {
                        /* Update all elements */
                        m[row, i] -= mult * m[pivot, i];
                    }
                }
            }

            return m;
        }

        /// <summary>
        /// Compute the lower and upper decomposition of this matrix.
        /// </summary>
        /// <returns></returns>
        public (Matrix L, Matrix U) Decompose()
        {
            /* Generate an L and U */
            Matrix L = Matrix.Identity(Size.rows);
            Matrix U = Copy();
            
            /* Use multiplier method to decompose U into L and U */
            for (uint pivot = 0; pivot < U.Size.rows; pivot++)
            {
                /* Make sure the matrix is not degenerate, try and swap */
                if (U[pivot, pivot] == 0)
                {
                    for (uint swaprow = pivot + 1; swaprow < U.Size.rows; swaprow++)
                    {
                        if (U[swaprow, pivot] != 0.0f)
                        {
                            for (uint i = 0; i < U.Size.cols; i++)
                            {
                                (U[swaprow, i], U[pivot, i]) = (U[pivot, i], U[swaprow, i]);
                                (L[swaprow, i], L[pivot, i]) = (L[pivot, i], L[swaprow, i]);
                            }

                            break;
                        }
                    }
                }
                
                /* If still zero, the matrix is degenerate */
                if (U[pivot, pivot] == 0)
                {
                    throw new Exception("Matrix state is degenerate, cannot finish triangulation.");
                }
                
                /* Ensure only the pivot row has a non-zero value in the pivot position */
                for (uint row = pivot + 1; row < U.Size.rows; row++)
                {
                    if (U[row, pivot] != 0)
                    {
                        /* Compute how many of the pivot row to add to this row */
                        float multiplier = -1 * U[row, pivot] / U[pivot, pivot];
                        
                        /* The corresponding L entry will be the inverse of this multiplier */
                        L[row, pivot] = -multiplier;
                        
                        /* Perform the operation */
                        for (uint col = pivot; col < U.Size.cols; col++)
                        {
                            U[row, col] += multiplier * U[pivot, col];
                        }
                    }
                }
            }
            
            /* Return the decomposition */
            return (L, U);
        }

        /// <summary>
        /// Attempt to decompose this matrix to a diagonal. If successful, return true.
        /// </summary>
        /// <returns></returns>
        public bool CanSolve()
        {
            try
            {
                Solve(new Matrix((Size.rows, 1)));
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Compute the determinant of this matrix.s
        /// </summary>
        /// <returns></returns>
        public float Determinant()
        {
            if (Size.rows != Size.cols)
            {
                throw new Exception("Non-square matrices do not have determinants.");
            }
            
            try
            {
                var lu = Decompose();

                float det = 1;
                for (uint i = 0; i < lu.L.Size.rows; i++)
                {
                    det *= lu.L[i, i];
                    det *= lu.U[i, i];
                }

                return det;
            }
            catch(Exception e)
            {
                return 0;
            }
        }

        /// <summary>
        /// Override for multiplication.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix operator *(Matrix a, Matrix b)
        {
            /* Before anything, ensure sizing is correct */
            if (a.Size.cols != b.Size.rows)
            {
                throw new Exception("Matrices cannot be multiplied. " + a.Size + " vs " + b.Size);
            }
            
            /* Allocate a new matrix */
            Matrix ab = new Matrix((a.Size.rows, b.Size.cols));
            
            /* Fill the matrix */
            for (uint r = 0; r < ab.Size.rows; r++)
            {
                for (uint c = 0; c < ab.Size.cols; c++)
                {
                    float sum = 0.0f;
                    for (uint i = 0; i < a.Size.cols; i++)
                    {
                        sum += a[r, i] * b[i, c];
                    }

                    ab[r, c] = sum;
                }
            }

            return ab;
        }
        
        /// <summary>
        /// Override for subtraction.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix operator -(Matrix a, Matrix b)
        {
            /* Before anything, ensure sizing is correct */
            if (a.Size.cols != b.Size.cols || a.Size.rows != b.Size.rows)
            {
                throw new Exception("Matrices cannot be subtracted. " + a.Size + " vs " + b.Size);
            }
            
            /* Allocate a new matrix */
            Matrix ab = new Matrix((a.Size.rows, b.Size.cols));
            
            /* Fill the matrix */
            for (uint r = 0; r < ab.Size.rows; r++)
            {
                for (uint c = 0; c < ab.Size.cols; c++)
                {
                    ab[r, c] = a[r,c] - b[r,c];
                }
            }

            return ab;
        }
        
        /// <summary>
        /// Override for subtraction.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix operator +(Matrix a, Matrix b)
        {
            /* Before anything, ensure sizing is correct */
            if (a.Size.cols != b.Size.cols || a.Size.rows != b.Size.rows)
            {
                throw new Exception("Matrices cannot be subtracted. " + a.Size + " vs " + b.Size);
            }
            
            /* Allocate a new matrix */
            Matrix ab = new Matrix((a.Size.rows, b.Size.cols));
            
            /* Fill the matrix */
            for (uint r = 0; r < ab.Size.rows; r++)
            {
                for (uint c = 0; c < ab.Size.cols; c++)
                {
                    ab[r, c] = a[r,c] + b[r,c];
                }
            }

            return ab;
        }

        /// <summary>
        /// Determine if two matrices are equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Matrix a, Matrix b)
        {
            if (a.Size.rows != b.Size.rows || a.Size.cols != b.Size.cols) return false;

            for (uint row = 0; row < a.Size.rows; row++)
            {
                for (uint col = 0; col < a.Size.cols; col++)
                {
                    if (a[row, col] != b[row, col]) return false;
                }
            }

            return true;
        }
        
        /// <summary>
        /// Determine if two matrices are not equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Matrix a, Matrix b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Using this matrix as a set of constraints, solve a system with
        /// respect to the equality matrix. Each column is one system.
        ///
        /// Uses gauss-jordan elimination.
        /// </summary>
        /// <param name="equality"></param>
        /// <returns></returns>
        public Matrix Solve(Matrix equality)
        {
            /* Make sure dimensions are correct */
            if (equality.Size.rows != Size.rows)
            {
                throw new Exception("Solver requires identical number of rows between equals and constraints.");
            }
            
            /* Get a copy of the equals part */
            var b = equality.Copy();
            
            /* Perform gauss jordan to generate an upper-triangular, keeping the equality updated */
            /* Make a copy */
            Matrix m = Copy();

            /* Iterate through each pivot position */
            for (uint pivot = 0; pivot < Size.cols; pivot++)
            {
                /* Calculate the starting row for this pivot */
                uint row = pivot;
                
                /* if the pivot is zero, attempt to swap two rows */
                if (m[row, pivot] == 0.0f)
                {
                    for (uint swaprow = row + 1; swaprow < Size.rows; swaprow++)
                    {
                        /* Check if we can use this row, and swap if so */
                        if (m[swaprow, pivot] != 0.0f)
                        {
                            for (uint i = 0; i < Size.cols; i++)
                            {
                                (m[swaprow, i], m[row, i]) = (m[row, i], m[swaprow, i]);
                            }

                            for (uint c = 0; c < b.Size.cols; c++)
                            {
                                (b[swaprow, c], b[row, c]) = (b[row, c], b[swaprow, c]);
                            }
                            
                            break;
                        }
                    }
                }
                
                /* at this point, if the pivot is still zero, the matrix is degenerate */
                if (m[row, pivot] == 0)
                {
                    throw new Exception("Matrix state is degenerate, cannot finish solving.");
                }
                
                /* "fix" this row so the pivot is 1 */
                if (m[row, pivot] != 1.0f)
                {
                    float divisor = m[row, pivot];
                    for (uint i = pivot; i < Size.cols; i++)        // update m
                    {
                        m[row, i] /= divisor;
                    }
                    for (uint i = 0; i < b.Size.cols; i++)      // update b
                    {
                        b[row, i] /= divisor;
                    }
                }
                
                /* Now subtract this from other rows to clear the rest of the matrix */
                for (row = row + 1; row < Size.rows; row++)
                {
                    /* Figure out how much needs to be subtracted */
                    float mult = m[row, pivot];
                    for (uint i = pivot; i < Size.cols; i++)        // update m
                    {
                        /* Update all elements */
                        m[row, i] -= mult * m[pivot, i];
                    }
                    for (uint i = 0; i < b.Size.cols; i++)        // update b
                    {
                        /* Update all elements */
                        b[row, i] -= mult * b[pivot, i];
                    }
                }
            }
            
            /* Now use back-substitution to finish solving the upper-triangular matrix */
            for (int pivot = (int)Size.rows - 1; pivot >= 0; pivot--)
            {
                /* subtract this from other rows to clear the rest of the matrix */
                for (int row = pivot - 1; row >= 0; row--)
                {
                    /* Figure out how much needs to be subtracted */
                    float mult = m[(uint)row, (uint)pivot];
                    for (int i = pivot; i >= 0; i--)        // update m
                    {
                        /* Update all elements */
                        m[(uint)row, (uint)i] -= mult * m[(uint)pivot, (uint)i];
                    }
                    for (uint i = 0; i < b.Size.cols; i++)        // update b
                    {
                        /* Update all elements */
                        b[(uint)row, i] -= mult * b[(uint)pivot, i];
                    }
                }
            }
            
            /* Return the solution */
            return b;
        }

        /// <summary>
        /// Compute the inverse of this matrix.
        /// </summary>
        /// <returns></returns>
        public Matrix Inverse()
        {
            /* First make sure the matrix is invertible */
            if (Size.rows != Size.cols)
            {
                throw new Exception("Unable to invert a non-square matrix.");
            }
            
            /* Solve a system with an identity */
            try
            {
                return Solve(Identity(Size.rows));
            }
            catch (Exception e)
            {
                throw new Exception("Unable to invert a singular matrix.");
            }
            
        }
        
        /// <summary>
        /// Transpose this matrix.
        /// </summary>
        /// <returns></returns>
        public Matrix Transpose()
        {
            Matrix m = new Matrix((Size.cols, Size.rows));
            
            for (uint row = 0; row < Size.rows; row++)
            {
                for (uint col = 0; col < Size.cols; col++)
                {
                    m[col, row] = Data[row, col];
                }
            }

            return m;
        }

        /// <summary>
        /// Compose this matrix with another horizontally, attaching them side by side.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Matrix ComposeHorizontal(Matrix other)
        {
            if (Size.rows != other.Size.rows)
            {
                throw new Exception("Cannot compose matrices with different row sizes horizontally.");
            }

            Matrix o = new Matrix((Size.rows, Size.cols + other.Size.cols));
            
            for (uint row = 0; row < Size.rows; row++)
            {
                for (uint col = 0; col < Size.cols; col++)
                {
                    o[row, col] = Data[row, col];
                }
                for (uint col = 0; col < other.Size.cols; col++)
                {
                    o[row, col + Size.cols] = other[row, col];
                }
            }

            return o;
        }
        
        /// <summary>
        /// Compose this matrix with another vertically, attaching them bottom to top.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Matrix ComposeVertical(Matrix other)
        {
            if (Size.cols != other.Size.cols)
            {
                throw new Exception("Cannot compose matrices with different column sizes vertically.");
            }

            Matrix o = new Matrix((Size.rows + other.Size.rows, Size.cols));
            
            for (uint row = 0; row < Size.rows; row++)
            {
                for (uint col = 0; col < Size.cols; col++)
                {
                    o[row, col] = Data[row, col];
                }
            }
            for (uint row = 0; row < other.Size.rows; row++)
            {
                for (uint col = 0; col < Size.cols; col++)
                {
                    o[row + Size.rows, col] = other[row, col];
                }
            }

            return o;
        }

        /// <summary>
        /// Generate a new matrix from a specific set of columns.
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Matrix SelectColumns(List<int> columns)
        {
            Matrix m = new Matrix((Size.rows, (uint)columns.Count));
            
            columns.Sort();

            for (uint row = 0; row < Size.rows; row++)
            {
                for (int col = 0;  col < columns.Count; col++)
                {
                    uint column = (uint)columns[col];
                    m[row, (uint)col] = Data[row, column];
                }
            }

            return m;
        }
        
        /// <summary>
        /// Generate a new matrix from a specific set of rows.
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Matrix SelectRows(List<int> rows)
        {
            Matrix m = new Matrix(((uint)rows.Count, Size.cols));
            
            rows.Sort();

            for (uint col = 0; col < Size.cols; col++)
            {
                for (int r = 0;  r < rows.Count; r++)
                {
                    uint row = (uint)rows[r];
                    m[(uint)r, col] = Data[row, col];
                }
            }

            return m;
        }
        
        /// <summary>
        /// Scale all elements in  this matrix by a given factor.
        /// </summary>
        /// <returns></returns>
        public Matrix Scale(float scalar)
        {
            Matrix m = new Matrix((Size.rows, Size.cols));
            
            for (uint col = 0; col < Size.cols; col++)
            {
                for (uint row = 0;  row < Size.rows; row++)
                {
                    m[row, col] = scalar * Data[row, col];
                }
            }

            return m;
        }
    }
}

