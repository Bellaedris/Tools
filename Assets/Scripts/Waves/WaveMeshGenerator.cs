using UnityEngine;

public class WaveMeshGenerator : ScalarField
{
    public WaveMeshGenerator(float width, float length, int nx, int ny) 
        : base(nx, ny, width, length) {}

    public void GenerateSinWaves(int octaves)
    {
        for(int i = 0; i < nx; i++)
        {
            for(int j = 0; j < ny; j++)
            {
                Vector3 waveDir = new Vector3(1f, 0f);

                float totalHeight = 0;
                float wavelength = .08f;
                float amplitude = 1f;
                float falloff = 0.5f;
                for(int o = 0; o < octaves; o++)
                {
                    totalHeight += amplitude * Mathf.Sin(Vector2.Dot(waveDir, new Vector2(i, j) * wavelength) /* + t * speed*/);
                    amplitude *= falloff;
                    wavelength *= 2;

                    waveDir = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
                }
                meshData.vertices[i * nx + j].z = totalHeight;
            }
        }
    }
}
