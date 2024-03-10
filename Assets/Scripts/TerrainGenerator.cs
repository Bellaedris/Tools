using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{

    public int numberOfSubCubes = 10;
    public int squareSize = 2;
    public float squareHeightMax = 1.0f;
    
    public int octaves = 4;
    public float time = 0f;
    public Renderer targetMesh; // mesh we will put our terrain on

    public void GenerateScifiCubicTerrain()
    {
        ScifiCubicTerrainMeshGenerator gen = new ScifiCubicTerrainMeshGenerator(10, 10, 256, 256);
        gen.GenerateScifiCubic(numberOfSubCubes, squareSize, squareHeightMax);
        targetMesh.GetComponent<MeshFilter>().sharedMesh = gen.CreateMesh();
    }

    public void GenerateOceanTerrain()
    {
        WaveMeshGenerator gen = new WaveMeshGenerator(10, 10, 256, 256);
        gen.GenerateSinWaves(octaves, time);
        targetMesh.GetComponent<MeshFilter>().sharedMesh = gen.CreateMesh();
    }

    public void GenerateFlatTerrain()
    {
        ScalarField gen = new ScalarField(10, 10, 256, 256);
        targetMesh.GetComponent<MeshFilter>().sharedMesh = gen.CreateMesh();
    }

}
