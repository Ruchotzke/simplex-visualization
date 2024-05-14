using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Matrices;

public class MatrixTests
{
    [Test]
    public void MatrixTestsSquare()
    {
        Matrix m = new Matrix("1 2 3; 4 5 6 ; 7 8 9;");
        
        Assert.AreEqual(m.Size.rows, 3);
        Assert.AreEqual(m.Size.cols, 3);

        Assert.AreEqual(m[0, 0], 1);
        Assert.AreEqual(m[0, 1], 2);
        Assert.AreEqual(m[0, 2], 3);
        
        Assert.AreEqual(m[1, 0], 4);
        Assert.AreEqual(m[1, 1], 5);
        Assert.AreEqual(m[1, 2], 6);
        
        Assert.AreEqual(m[2, 0], 7);
        Assert.AreEqual(m[2, 1], 8);
        Assert.AreEqual(m[2, 2], 9);
    }
    
    [Test]
    public void MatrixTestsColumn()
    {
        Matrix m = new Matrix("1; 2; 3; 4; 5;");

        Assert.AreEqual(m.Size.rows, 5);
        Assert.AreEqual(m.Size.cols, 1);

        Assert.AreEqual(m[0, 0], 1);
        Assert.AreEqual(m[1, 0], 2);
        Assert.AreEqual(m[2, 0], 3);
        Assert.AreEqual(m[3, 0], 4);
        Assert.AreEqual(m[4, 0], 5);
    }
    
    [Test]
    public void MatrixUpperTriSingular()
    {
        Matrix m = new Matrix("1 2 3; 4 5 6 ; 7 8 9;");

        Assert.Throws<Exception>(delegate { m.UpperTriangularOnes(); });
    }
    
    [Test]
    public void MatrixUpperTriNormal()
    {
        Matrix m = new Matrix("1 2 3; 4 5 6 ; 7 8 10;");

        m = m.UpperTriangularOnes();

        Assert.AreEqual(1,m[0, 0]);
        Assert.AreEqual(2, m[0, 1]);
        Assert.AreEqual(3, m[0, 2]);
        
        Assert.AreEqual(0, m[1, 0]);
        Assert.AreEqual(1, m[1, 1]);
        Assert.AreEqual(2, m[1, 2]);
        
        Assert.AreEqual(0, m[2, 0]);
        Assert.AreEqual(0, m[2, 1]);
        Assert.AreEqual(1, m[2, 2]);
    }

    [Test]
    public void MatrixDeterminantNormal()
    {
        Matrix m = new Matrix("1 2 3; 4 5 6 ; 7 8 10;");

        Assert.AreEqual(-3, m.Determinant());
    }
    
    [Test]
    public void MatrixDeterminantNotSquare()
    {
        Matrix m = new Matrix("1 2 3 4; 4 5 6 65; 7 8 9 9;");

        Assert.Throws<Exception>(delegate { m.Determinant(); });
    }
    
    [Test]
    public void MatrixDeterminantZero()
    {
        Matrix m = new Matrix("1 2 3; 4 5 6 ; 7 8 9;");

        Assert.AreEqual(0, m.Determinant());
    }
    
    [Test]
    public void MatrixMultiply2X2()
    {
        Matrix a = new Matrix("1 2; 3 4;");
        Matrix b = new Matrix("4 3; 2 1;");

        Matrix ab = a * b;
        Matrix ba = b * a;
        
        Assert.AreEqual(8, ab[0,0]);
        Assert.AreEqual(5, ab[0,1]);
        Assert.AreEqual(20, ab[1,0]);
        Assert.AreEqual(13, ab[1,1]);
        
        Assert.AreEqual(13, ba[0,0]);
        Assert.AreEqual(20, ba[0,1]);
        Assert.AreEqual(5, ba[1,0]);
        Assert.AreEqual(8, ba[1,1]);
    }
    
    [Test]
    public void MatrixMultiply3X2()
    {
        Matrix a = new Matrix("1 2 3; 4 5 6;");
        Matrix b = new Matrix("1 2; 3 4; 5 6;");

        Matrix ab = a * b;
        
        Assert.AreEqual(2, ab.Size.rows);
        Assert.AreEqual(2, ab.Size.cols);
        Assert.AreEqual(22, ab[0,0]);
        Assert.AreEqual(28, ab[0,1]);
        Assert.AreEqual(49, ab[1,0]);
        Assert.AreEqual(64, ab[1,1]);
        
        Matrix ba = b * a;
        
        Assert.AreEqual(3, ba.Size.rows);
        Assert.AreEqual(3, ba.Size.cols);
        Assert.AreEqual(9, ba[0,0]);
        Assert.AreEqual(12, ba[0,1]);
        Assert.AreEqual(15, ba[0,2]);
        Assert.AreEqual(19, ba[1,0]);
        Assert.AreEqual(26, ba[1,1]);
        Assert.AreEqual(33, ba[1,2]);
        Assert.AreEqual(29, ba[2,0]);
        Assert.AreEqual(40, ba[2,1]);
        Assert.AreEqual(51, ba[2,2]);
    }

    [Test]
    public void MatrixLU()
    {
        Matrix m = new Matrix("1 4 -3; -2 8 5; 3 4 7;");

        (Matrix L, Matrix U) = m.Decompose();
        
        Assert.IsTrue(L == new Matrix("1 0 0; -2 1 0; 3 -0.5 1;"));
        Assert.IsTrue(U == new Matrix("1 4 -3; 0 16 -1; 0 0 15.5;"));
    }
    
    [Test]
    public void MatrixLURect()
    {
        Matrix m = new Matrix("1 2 3; 4 5 6;");

        (Matrix L, Matrix U) = m.Decompose();
        
        Assert.IsTrue(L == new Matrix("1 0; 4 1;"));
        Assert.IsTrue(U == new Matrix("1 2 3; 0 -3 -6;"));
    }
    
    [Test]
    public void MatrixSolve()
    {
        Matrix m = new Matrix("1 2 3; 4 5 6; 7 8 10;");
        Matrix b = new Matrix("0; 1; 2;");

        var sol = m.Solve(b);
        
        Assert.AreEqual(2.0f / 3.0f, sol[0,0], 0.001f);
        Assert.AreEqual(-1.0f / 3.0f, sol[1,0], 0.001f);
        Assert.AreEqual(0f, sol[2,0], 0.001f);
    }
    
    [Test]
    public void MatrixInverse()
    {
        Matrix m = new Matrix("1 2 3; 4 5 6; 7 8 10;");

        var inverse = m.Inverse();
        
        Assert.AreEqual(-2.0f / 3.0f, inverse[0,0], 0.001f);
        Assert.AreEqual(-4.0f / 3.0f, inverse[0,1], 0.001f);
        Assert.AreEqual(1, inverse[0,2], 0.001f);
        Assert.AreEqual(-2.0f / 3.0f, inverse[1,0], 0.001f);
        Assert.AreEqual(11.0f / 3.0f, inverse[1,1], 0.001f);
        Assert.AreEqual(-2.0f, inverse[1,2], 0.001f);
        Assert.AreEqual(1f, inverse[2,0], 0.001f);
        Assert.AreEqual(-2f, inverse[2,1], 0.001f);
        Assert.AreEqual(1f, inverse[2,2], 0.001f);
    }
    
    [Test]
    public void MatrixInverseSingular()
    {
        Matrix m = new Matrix("1 2 3; 7 8 10; 2 4 6;");


        Assert.Throws<Exception>(delegate { m.Inverse(); });
    }
    
    [Test]
    public void MatrixTransposeSquare()
    {
        Matrix m = new Matrix("1 2 3; 7 8 10; 2 4 6;");

        Assert.IsTrue(new Matrix("1 7 2; 2 8 4; 3 10 6;") == m.Transpose());
    }
    
    [Test]
    public void MatrixTransposeRect()
    {
        Matrix m = new Matrix("1 2 3; 4 5 6;");

        Assert.IsTrue(new Matrix("1 4; 2 5; 3 6;") == m.Transpose());
    }
    
    [Test]
    public void MatrixComposeVertical()
    {
        Matrix m = new Matrix("1 2; 3 4; 5 6;");
        Matrix i = Matrix.Identity(2);

        Assert.IsTrue(new Matrix("1 2; 3 4; 5 6; 1 0; 0 1;") == m.ComposeVertical(i));
    }
    
    [Test]
    public void MatrixComposeHorizontal()
    {
        Matrix m = new Matrix("1 2 3; 4 5 6; 7 8 9;");
        Matrix i = Matrix.Identity(3);
        
        Assert.IsTrue(new Matrix("1 2 3 1 0 0; 4 5 6 0 1 0; 7 8 9 0 0 1;") == m.ComposeHorizontal(i));
    }
    
    [Test]
    public void MatrixSelectCols()
    {
        Matrix m = new Matrix("1 2 3; 4 5 6; 7 8 9;");
        Matrix c = m.SelectColumns(new List<int>() { 0, 2 });
        
        Assert.IsTrue(new Matrix("1 3; 4 6; 7 9;") == c);
    }

    [Test]
    public void MatrixSolveSwapping()
    {
        Matrix m = new Matrix("0 0 1; 1 0 0; 0 1 0;");
        Matrix b = new Matrix("1; 0; 0;");

        Matrix solved = m.Solve(b);
        
        Assert.IsTrue(new Matrix("0; 0; 1;") == solved);
    }
    
    [Test]
    public void MatrixInverseSwapping()
    {
        Matrix m = new Matrix("0 0 1; 1 0 0; 0 1 0;");
        Matrix inv = m.Inverse();
        
        Assert.IsTrue(new Matrix("0 1 0; 0 0 1; 1 0 0;") == inv);
    }
}
