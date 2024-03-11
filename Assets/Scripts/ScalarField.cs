using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering;

public struct MeshData
{
    public Vector3[] vertices;
    public Vector2[] uvs;
    public List<int> triangles;

    public MeshData(int nx, int ny)
    {
        vertices = new Vector3[nx * ny];
        uvs = new Vector2[nx * ny];
        triangles = new List<int>();
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles.Add(a);
        triangles.Add(b);
        triangles.Add(c);
    }

    public Mesh CreateMesh()
    {
        Mesh m = new Mesh();
        m.indexFormat = IndexFormat.UInt32;
        m.vertices = vertices;
        m.uv = uvs;
        m.triangles = triangles.ToArray();
        m.RecalculateNormals();

        return m;
    }
}

public class ScalarField
{
    protected int nx;
    protected int ny;
    protected float width;
    protected float length;
    public MeshData meshData {get;}

    public ScalarField(int nx, int ny, float width, float length)
    {
        this.nx = nx;
        this.ny = ny;
        this.width = width;
        this.length = length;

        // initialize flat mesh
        meshData = new MeshData(nx, ny);
        GenerateFlat();
    }

    public ScalarField(int nxy, float size) : this(nxy, nxy, size, size) {}

    public void GenerateFlat()
    {
        for(int i = 0; i < nx; i++)
        {
            for(int j = 0; j < ny; j++)
            {
                int index = i * nx + j;
                meshData.vertices[index] = new Vector3(i / width, 0, j / length);
                meshData.uvs[index] = new Vector2(i / (float) nx, j / (float) ny);

                if (i < nx - 1 && j < ny - 1)
                {
                    meshData.AddTriangle(index, index + ny, index + ny + 1);
                    meshData.AddTriangle(index, index + ny + 1, index + 1);
                }
            }
        }
    }

    public Mesh CreateMesh()
    {
        return meshData.CreateMesh();
    }
}