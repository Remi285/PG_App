using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public static Generator instance;
    private bool canGenerate = true;
    public PerlinNoise perlinNoiseScript;
    public DiamondSquare diamondSquare;
    public Voronoi voronoi;
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
        diamondSquare.Generate(regions);
    }

    public void GeneratePerlin()
    {
        perlinNoiseScript.Generate(regions);
    }

    public void GenerateVoronoi()
    {
        voronoi.Generate(regions);
    }
    public void GenerateFromImage()
    {
        generateFromImage.Generate(regions);
    }
    public void GenerateNN()
    {
        nnGeneration.Generate(regions);
    }
}

[Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}