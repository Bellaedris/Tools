using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScifiCubicTerrainMeshGenerator : ScalarField
{
    public ScifiCubicTerrainMeshGenerator(float width, float height, int nx, int ny)
        : base(nx, ny, width, height) {}

    public void GenerateScifiCubic(int numberOfSquares, int squaresSize, float squareHeightMax)
    {
        for(int i = 0; i < numberOfSquares; i++)
        {
            int half_size = squaresSize / 2;
            int randX = Random.Range(half_size, nx - half_size);
            int randY = Random.Range(half_size, ny - half_size);
            float newHeight = Random.Range(-squareHeightMax, squareHeightMax);
            updateSquare(half_size, newHeight, (randX * nx + randY), nx);
        }
    }

    // creates a squared area that is offsetted in z axis.
    public void updateSquare(int size, float newHeight, int seed, int nx)
    {
        //add new vertices on the border of the square
        
        for(int i = -size; i < size; i++)
        {
            for(int j = -size; j < size; j++)
            {
                meshData.vertices[seed + i + j * nx].z = newHeight;
            }
        }

        //TODO link them with the existing border vertices that we moved
    }

}
