using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Matrices;
using Simplex;

public class SimplexTests
{
    [Test]
    public void SimplexTestPartition()
    {
        Matrix A = new Matrix("1 0 -3; 7 2 5;");
        Matrix b = new Matrix("0; 1;");
        Matrix c = new Matrix("1; 2; 3;");

        List<int> basic = new List<int>() { 0, 1 };
        List<int> nonBasic = new List<int>() { 2, 3, 4 };

        Dictionary d = new Dictionary(A, b, c, basic, nonBasic, 3);
        Debug.Log(d.Message);
    }
    
    [Test]
    public void SimplexTestPartitionOptimal()
    {
        Matrix A = new Matrix("1 0 0; 20 1 0; 200 20 1;");
        Matrix b = new Matrix("1; 100; 1000;");
        Matrix c = new Matrix("100; 10; 1");

        List<int> basic = new List<int>() { 2, 3, 4};
        List<int> nonBasic = new List<int>() { 0, 1, 5 };

        Dictionary d = new Dictionary(A, b, c, basic, nonBasic, 3);
        Debug.Log(d.Message);
    }
}
