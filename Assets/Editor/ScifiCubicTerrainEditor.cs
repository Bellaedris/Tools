using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class ScifiCubicTerrainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TerrainGenerator generator = (TerrainGenerator) target;

        if (DrawDefaultInspector()) {
            
        }

        if(GUILayout.Button("Generate Scifi"))
        {
            generator.GenerateScifiCubicTerrain();
        }

        if(GUILayout.Button("Generate sin waves"))
        {
            generator.GenerateOceanTerrain();
        }

        if(GUILayout.Button("Generate flat terrain"))
        {
            generator.GenerateFlatTerrain();
        }
    }
}
