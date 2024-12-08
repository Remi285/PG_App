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
    public OneD oneD;

    private void Awake() 
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
    }
    public void GenerateDiamonSquare()
    {
        if(canGenerate)
        {
            diamondSquare.Generate();
            canGenerate = false;
        }
        else
        {
            Debug.LogError("Clear generation first");
        }

    }

    public void Generate1D()
    {
        if(canGenerate)
        {
            oneD.Generate();
            canGenerate = false;
        }
        else
        {
            Debug.LogError("Clear generation first");
        }

    }
    public void GeneratePerlin()
    {
        if(canGenerate)
        {
            perlinNoiseScript.Generate();
            canGenerate = false;
        }
        else
        {
            Debug.LogError("Clear generation first");
        }
    }
    public void ClearVisualization()
    {
        perlinNoiseScript.ClearVisualization();
        diamondSquare.ClearVisualization();
        oneD.ClearVisualization();
        canGenerate = true;
    }
}
