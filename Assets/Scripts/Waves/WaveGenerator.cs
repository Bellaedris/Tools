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
    public float speed;
    public Vector2 direction;

    public WaveParameter(float amp, float wl, float s, float dir) 
    {
        amplitude = amp;
        wavelength = wl;
        speed = s;
        direction = new Vector2(Mathf.Cos(Mathf.Deg2Rad * dir), Mathf.Sin(Mathf.Deg2Rad * dir)).normalized;
    }
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WaveGenerator : MonoBehaviour
{

    public float oceanSize = 10f;
    public int oceanResolution = 256;

    public Color sunColor;
    public Color waterColor;
    public float specularStrength = .5f;
    public int specularReflectance = 32;
    public float fresnelStrength = 5.0f;

    public bool useRandomWaves;
    public int numberOfWaves = 4;
    public float mainDirAngle = 0;
    public float minAmplitude = .5f;
    public float maxAmplitude = 3.0f;
    public float minWavelength = .1f;
    public float maxWavelength = .3f;
    public float minSpeed = 1.0f;
    public float maxSpeed = 5.0f;
    public float angleAmplitude = 30f;
    public List<WaveParameter> waveParameters;

    public Shader waterShader;
    public Texture environmentMap;

    public bool useSharpSine = false;

    private Material waterMaterial;
    private ComputeBuffer wavesBuffer;

    // Update is called once per frame
    void Update()
    {
        //update parameters
        waterMaterial.SetVector("sunDir", -FindObjectOfType<Light>().transform.forward);
        waterMaterial.SetFloat("specularStrength", specularStrength);
        waterMaterial.SetFloat("specularReflectance", specularReflectance);
        waterMaterial.SetFloat("fresnelStrength", fresnelStrength);

        waterMaterial.SetColor("_Color", waterColor);
        waterMaterial.SetColor("sunColor", sunColor);
    }

    void FillWaveParameters()
    {
        waveParameters.Clear();
        for(int i = 0; i < numberOfWaves; i++)
        {
            waveParameters.Add(new WaveParameter(
                UnityEngine.Random.Range(minAmplitude, maxAmplitude),
                UnityEngine.Random.Range(minWavelength, maxWavelength),
                UnityEngine.Random.Range(minSpeed, maxSpeed),
                UnityEngine.Random.Range(mainDirAngle - angleAmplitude, mainDirAngle + angleAmplitude)
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
        waterMaterial.SetVector("sunDir", -FindObjectOfType<Light>().transform.forward);
        waterMaterial.SetColor("sunColor", sunColor);
        waterMaterial.SetFloat("specularStrength", specularStrength);
        waterMaterial.SetFloat("specularReflectance", specularReflectance);
        waterMaterial.SetFloat("fresnelStrength", fresnelStrength);
        waterMaterial.SetTexture("environmentMap", environmentMap);

        waterMaterial.SetColor("_Color", waterColor);

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
