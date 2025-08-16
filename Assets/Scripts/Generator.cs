using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public static Generator instance;
    private bool canGenerate = true;
    public PerlinNoise perlinNoiseScript;
    public DiamondSquare diamondSquare;
    public Worley worley;
    public GenerateFromImage generateFromImage;
    public NNGeneration nnGeneration;
    public TerrainType[] regions;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }
    public void GenerateDiamonSquare()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        diamondSquare.Generate(regions);
        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        UnityEngine.Debug.Log("Generation time: " + elapsedMilliseconds + " ms");

    }

    public void GeneratePerlin()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        perlinNoiseScript.Generate(regions);
        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        UnityEngine.Debug.Log("Generation time: " + elapsedMilliseconds + " ms");
    }

    public void GenerateWorley()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        worley.Generate(regions);
        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        UnityEngine.Debug.Log("Generation time: " + elapsedMilliseconds + " ms");
    }
    public void GenerateFromImage()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        generateFromImage.Generate(regions);
        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        UnityEngine.Debug.Log("Generation time: " + elapsedMilliseconds + " ms");
    }
    public void GenerateNN()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        nnGeneration.Generate(regions);
        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        UnityEngine.Debug.Log("Generation time: " + elapsedMilliseconds + " ms");
    }
}

[Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}