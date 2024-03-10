using UnityEngine;

public class WaveMeshGenerator : ScalarField
{
    public WaveMeshGenerator(float width, float length, int nx, int ny) 
        : base(nx, ny, width, length) {}

    public void GenerateSinWaves(int octaves, float time)
    {
        Vector3[] waveDir = {
            new Vector3(1f, 0f),
            new Vector3(-.2f, .6f),
            new Vector3(.3f, -.8f),
            new Vector3(0f, -.6f),
        };

        for(int i = 0; i < nx; i++)
        {
            for(int j = 0; j < ny; j++)
            {
                float totalHeight = 0;
                float wavelength = .08f;
                float amplitude = 1f;
                for(int o = 0; o < octaves; o++)
                {
                    totalHeight += amplitude * Mathf.Sin(Vector2.Dot(waveDir[o], new Vector2(i, j) * wavelength)  + time);

                }
                meshData.vertices[i * nx + j].z = totalHeight;
            }
        }
    }
}
