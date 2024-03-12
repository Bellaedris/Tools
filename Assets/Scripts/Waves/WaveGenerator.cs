using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using static System.Runtime.InteropServices.Marshal;

[Serializable]
public struct WaveParameter
{
    public float amplitude;
    public float wavelength;
    public Vector2 direction;

    public WaveParameter(float amp, float wl, float dir) 
    {
        amplitude = amp;
        wavelength = wl;
        direction = new Vector2(Mathf.Cos(Mathf.Deg2Rad * dir), Mathf.Sin(Mathf.Deg2Rad * dir)).normalized;
    }
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WaveGenerator : MonoBehaviour
{

    public float oceanSize = 10f;
    public int oceanResolution = 256;

    public Color sunColor;
    public float specularStrength = .5f;

    public bool useRandomWaves;
    public int numberOfWaves = 4;
    public float mainDirAngle = 0;
    public List<WaveParameter> waveParameters;

    public Shader waterShader;

    public bool useSharpSine = false;

    private Material waterMaterial;
    private ComputeBuffer wavesBuffer;

    // Update is called once per frame
    void Update()
    {
        
    }

    void FillWaveParameters()
    {
        waveParameters.Clear();
        for(int i = 0; i < numberOfWaves; i++)
        {
            waveParameters.Add(new WaveParameter(
                UnityEngine.Random.Range(.5f, 3.0f),
                UnityEngine.Random.Range(.1f, .3f),
                UnityEngine.Random.Range(mainDirAngle - 30f, mainDirAngle + 30f)
            ));
        }
    }

    void GenerateWaterMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        ScalarField sf = new ScalarField(oceanResolution, oceanSize);
        meshFilter.sharedMesh = sf.CreateMesh();
    }

    void GenerateWaterMaterial()
    {
        waterMaterial = new Material(waterShader);
        
        waterMaterial.SetInt("numberOfWaves", waveParameters.Count);
        waterMaterial.SetBuffer("waves", wavesBuffer);
        waterMaterial.SetVector("sun_dir", FindObjectOfType<Light>().transform.forward);
        waterMaterial.SetColor("sunColor", sunColor);
        waterMaterial.SetFloat("specularStrength", specularStrength);

        if (useSharpSine)
            waterMaterial.EnableKeyword("SHARP_SINE");
        else
            waterMaterial.EnableKeyword("SINE");

        GetComponent<MeshRenderer>().material = waterMaterial;
    }

    void GenerateWaveBuffer()
    {
        wavesBuffer = new ComputeBuffer(64, SizeOf(typeof(WaveParameter)));

        wavesBuffer.SetData(waveParameters);
    }

    private void OnEnable() {
        if (useRandomWaves)
            FillWaveParameters();
        GenerateWaterMesh();
        GenerateWaveBuffer();
        GenerateWaterMaterial();
    }

    private void OnDisable() {
        if(wavesBuffer != null)
            wavesBuffer.Release();
    }
}
