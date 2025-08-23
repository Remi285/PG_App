using System;
using System.Diagnostics;
using UnityEngine;
using TMPro;

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

    [SerializeField] TMP_InputField perlin_gen_time;
    [SerializeField] TMP_InputField ds_gen_time;
    [SerializeField] TMP_InputField worley_gen_time;
    [SerializeField] TMP_InputField ai_gen_time;

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
        ds_gen_time.text = elapsedMilliseconds.ToString();

    }

    public void GeneratePerlin()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        perlinNoiseScript.Generate(regions);
        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        UnityEngine.Debug.Log("Generation time: " + elapsedMilliseconds + " ms");
        perlin_gen_time.text = elapsedMilliseconds.ToString();
    }

    public void GenerateWorley()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        worley.Generate(regions);
        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        UnityEngine.Debug.Log("Generation time: " + elapsedMilliseconds + " ms");
        worley_gen_time.text = elapsedMilliseconds.ToString();
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
        ai_gen_time.text = elapsedMilliseconds.ToString();
    }
}

[Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}