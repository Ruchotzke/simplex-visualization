using System;
using System.Collections;
using System.Collections.Generic;
using GK;
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

    public MeshFilter HullMesh;

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

            if (d.IsFeasible)
            {
                if (d.IsOptimal)
                {
                    cornerPoints.Add(d.Point);
                    optimal = d.Point;
                }
                else
                {
                    if(!cornerPoints.Contains(d.Point)) cornerPoints.Add(d.Point);
                }
            }
        }
        
        /* Compute the feasible region (a convex hull) from the points */
        ConvexHullCalculator c = new ConvexHullCalculator();
        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();
        List<Vector3> norms = new List<Vector3>();
        c.GenerateHull(cornerPoints, false, ref verts, ref indices, ref norms);

        Mesh mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);
        mesh.SetNormals(norms);

        HullMesh.mesh = mesh;
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
