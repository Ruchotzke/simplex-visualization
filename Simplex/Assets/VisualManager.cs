using System;
using System.Collections;
using System.Collections.Generic;
using Matrices;
using Simplex;
using UnityEngine;

public class VisualManager : MonoBehaviour
{
    public static VisualManager Instance;

    private void Awake()
    {
        /* Singleton */
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private List<Vector3> cornerPoints = new List<Vector3>();
    private Vector3 optimal;

    private void Start()
    {
        Matrix constraints = new Matrix("1 0 0; 0 1 0; 0 0 1;");
        Matrix bounds = new Matrix("3; 3; 3;");
        Matrix objective = new Matrix("-1; -1; -1;");
        
        /* Generate all partitions */
        var partitions = SimplexUtilities.GenerateAllPartitions(3, 3);
        List<Dictionary> dictionaries = new List<Dictionary>();
        foreach (var partition in partitions)
        {
            var d = new Dictionary(constraints, bounds, objective, partition.B, partition.N, 3);
            dictionaries.Add(d);

            // Debug.Log("Dict B: " + string.Join(" ", d.Basic) + "   Point: " + d.Point);
            // Debug.Log("Feasible: " + d.IsFeasible + " Optimal: " + d.IsOptimal + " Unbounded: " + d.IsUnbounded);
            // Debug.Log(d.Message);
            
            if (d.IsFeasible)
            {
                if (d.IsOptimal)
                {
                    optimal = d.Point;
                }
                else
                {
                    cornerPoints.Add(d.Point);
                }
            }
        }
    }


    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            /* Draw all corner points */
            Gizmos.color = Color.green;
            foreach (var corner in cornerPoints)
            {
                // Debug.Log(corner);
                Gizmos.DrawSphere(corner, 0.1f);
            }
        
            /* Draw optimal point */
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(optimal, 0.1f);
        }
        
    }
}
