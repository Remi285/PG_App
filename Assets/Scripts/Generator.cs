using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public PerlinNoise perlinNoiseScript;
    public void GeneratePerlin()
    {
        perlinNoiseScript.Generate();
    }
    public void ClearVisualization()
    {
        perlinNoiseScript.ClearVisualization();
    }
}
