using System;
using UnityEngine;
using UnityEngine.UI;

public class PerlinNoise : MonoBehaviour
{
    //Based on https://www.youtube.com/watch?v=sUDPfC1nH_E
    //and https://docs.unity3d.com/ScriptReference/Mathf.PerlinNoise.html
    //and https://youtu.be/RDQK1_SWFuc?si=HgZ0zEasuths89J8
    
    public static PerlinNoise instance;
    public int textureSizeX;
    public int textureSizeY;
    public bool randomizeNoiseOffest;
    public Vector2 offset;
    public float noiseScale = 1f;
    public GameObject visualizationCube;
    public float visualizationHeightScale = 5f;
    public RawImage hightVisualizationUI;
    public RawImage regionVisualizationUI;
    [SerializeReference] private GameObject visualizationParent;

    private Texture2D texture;
    private Texture2D colorTexture;
    private TerrainType[] regions;
        
    public int octaves = 4;
    public float lacunarity = 2f;
    public float persistence = 0.5f;

    public event Func<bool> OnGenerate;

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

    public void ClearVisualization()
    {
        Destroy(visualizationParent.GetComponent<MeshFilter>());
        Destroy(visualizationParent.GetComponent<MeshRenderer>());
    }
    public void Generate(TerrainType[] _regions)
    {
        if (OnGenerate != null && OnGenerate.Invoke() == false)
            return;
        regions = _regions;
        GenerateNoise();
        MapDisplay mapDisplay = GetComponent<MapDisplay>();
        mapDisplay.DrawMesh(MeshGenerator.GenerateMesh(texture, visualizationHeightScale), colorTexture);
    }
    
    private void GenerateNoise()
    {
        if (randomizeNoiseOffest)
        {
            offset = new Vector2(UnityEngine.Random.Range(0, 99999),
            UnityEngine.Random.Range(0, 99999));
        }
        texture = new Texture2D(textureSizeX, textureSizeY);
        colorTexture = new Texture2D(textureSizeX, textureSizeY);
        for (int x = 0; x < textureSizeX; x++)
        {
            for (int y = 0; y < textureSizeY; y++)
            {
                texture.SetPixel(x, y, SampleNoise(x, y).Item1);
                colorTexture.SetPixel(x, y, SampleNoise(x, y).Item2);
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        hightVisualizationUI.texture = texture;
        colorTexture.filterMode = FilterMode.Point;
        colorTexture.Apply();
        regionVisualizationUI.texture = colorTexture;
    }

    private (Color, Color) SampleNoise(int x, int y)
    {
        Color hightColor = new();
        Color regionColor = new();
        float amplitude = 1f;
        float frequency = 1f;
        float noiseHeight = 0f;
        for(int i = 0; i < octaves; i++)
        {
            float xCoord = (float)x / textureSizeX * noiseScale * frequency + offset.x;
            float yCoord = (float)y / textureSizeY * noiseScale * frequency + offset.y;
            float sample = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1; // zakres od -1 do 1
            noiseHeight += sample * amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }

        noiseHeight = Remap(noiseHeight, -1f, 1f, 0f, 1f);
        for(int i = 0; i < regions.Length; i++)
        {
            if(noiseHeight <= regions[i].height)
            {
                regionColor = regions[i].color;
                break;
            }
        }
        hightColor = new Color(noiseHeight, noiseHeight, noiseHeight);
        return (hightColor, regionColor);
    }
    
    float Remap(float value, float inMin, float inMax, float outMin, float outMax)
    {
        var result = (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
        if(result > outMax)
        {
            result = outMax;
        }
        if(result < outMin)
        {
            result = outMin;
        }
        return result;
    }
}
