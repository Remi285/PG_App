using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField] private PerlinNoise perlinNoiseScript;
    public void GeneratePerlin()
    {
        perlinNoiseScript.Generate();
    }
}
