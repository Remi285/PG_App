using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.TerrainUtils;
using UnityEngine.UI;

public class PerlinNoise : MonoBehaviour
{
    //Based on https://www.youtube.com/watch?v=sUDPfC1nH_E
    //and https://docs.unity3d.com/ScriptReference/Mathf.PerlinNoise.html
    public static PerlinNoise instance;
    public int textureSizeX;
    public int textureSizeY;
    public bool randomizeNoiseOffest;
    public Vector2 offset;
    public float noiseScale = 1f;
    public int gridStepSizeX;
    public int gridStepSizeY;
    public bool visualizeGrid = false;
    public GameObject visualizationCube;
    public float visualizationHeightScale = 5f;
    public RawImage visualizationUI;
    public GameObject visualizationParent;

    private Texture2D texture;

    private List<MeshFilter> meshFilters = new();



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

    public void ClearVisualization(Transform _transform)
    {
        Destroy(visualizationParent.gameObject);
        visualizationParent = new GameObject("VisualizationParent");
        visualizationParent.transform.parent = _transform;
        meshFilters.Clear();
    }
    public void Generate()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        GenerateNoise();
        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        UnityEngine.Debug.Log("Generation time: " + elapsedMilliseconds + " ms");
        if(visualizeGrid)
        {
            VisualizeGrid();
            CombineCubes();
        }
    }

    private void VisualizeGrid()
    {
        for(int x = 0; x < gridStepSizeX; x++)
        {
            for(int y = 0; y < gridStepSizeY; y++)
            {
                GameObject clone = Instantiate(visualizationCube, new Vector3(x, SampleStepped(x, y) * visualizationHeightScale, y) + transform.position, transform.rotation);
                meshFilters.Add(clone.GetComponent<MeshFilter>());
                clone.transform.SetParent(visualizationParent.transform);
            }
        }
        visualizationParent.transform.position = new Vector3(-gridStepSizeX * 0.5f, -visualizationHeightScale * 0.5f, -gridStepSizeY * 0.5f);
        
    }

    private float SampleStepped(int x, int y)
    {
        int _gridStepSizeX = textureSizeX / gridStepSizeX;
        int _gridStepSizeY = textureSizeY / gridStepSizeY;
        float sampledFloat = texture.GetPixel(Mathf.FloorToInt(x * _gridStepSizeX), Mathf.FloorToInt(y * _gridStepSizeY)).grayscale;
        return sampledFloat;
    }   

    private void GenerateNoise()
    {
        if(randomizeNoiseOffest)
        {
            offset = new Vector2(UnityEngine.Random.Range(0, 99999), UnityEngine.Random.Range(0, 99999));
        }
        texture = new Texture2D(textureSizeX, textureSizeY);
        for(int x = 0; x < textureSizeX; x++)
        {
            for(int y = 0; y < textureSizeY; y++)
            {
                texture.SetPixel(x, y, SampleNoise(x, y));
            }
        }
        texture.Apply();
        visualizationUI.texture = texture;
    }

    private Color SampleNoise(int x, int y)
    {
        float xCoord = (float)x / textureSizeX * noiseScale + offset.x;
        float yCoord = (float)y / textureSizeY * noiseScale + offset.y;
        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        Color color = new Color(sample, sample, sample);
        return color;
    }
    private void CombineCubes()
    {
        CombineInstance[] combine = new CombineInstance[meshFilters.Count];
        for(int i = 0; i < meshFilters.Count; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            Destroy(meshFilters[i].gameObject);
        }
        Mesh combinedMesh = new();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        visualizationParent.AddComponent<MeshFilter>().mesh = combinedMesh;
        visualizationParent.AddComponent<MeshRenderer>().material = visualizationCube.GetComponent<MeshRenderer>().sharedMaterial;
        combinedMesh.CombineMeshes(combine);

    }
}
