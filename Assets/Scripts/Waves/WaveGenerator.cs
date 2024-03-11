using System;
using System.Collections;
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
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WaveGenerator : MonoBehaviour
{

    public float oceanSize = 10f;
    public int oceanResolution = 256;

    public List<WaveParameter> waveParameters;

    public Shader waterShader;

    public bool useSharpSine = false;

    private Material waterMaterial;
    private ComputeBuffer wavesBuffer;

    // Update is called once per frame
    void Update()
    {
        
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

        if (useSharpSine)
            waterMaterial.EnableKeyword("SHARP_SINE");
        else
            waterMaterial.EnableKeyword("SINE");

        GetComponent<MeshRenderer>().material = waterMaterial;
    }

    void GenerateWaveBuffer()
    {
        wavesBuffer = new ComputeBuffer(16, SizeOf(typeof(WaveParameter)));

        wavesBuffer.SetData(waveParameters);
    }

    private void OnEnable() {
        GenerateWaterMesh();
        GenerateWaveBuffer();
        GenerateWaterMaterial();
    }

    private void OnDisable() {
        if(wavesBuffer != null)
            wavesBuffer.Release();
    }
}
