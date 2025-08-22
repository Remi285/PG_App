using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Worley : MonoBehaviour
{
    public int textureWidth = 256;
    public int textureHeight = 256;
    public int numCells = 20;
    public int maxHeight = 20;
    public float normalizationPower = 0.75f;
    public float waterProbability = 0.25f;
    public RawImage hightVisualizationUI;
    public RawImage regionVisualizationUI;
    public GameObject visualizationCube;
    public float visualizationHeightScale = 5f;
    private List<MeshFilter> meshFilters = new();
    private Texture2D texture;
    private Texture2D colorTexture;
    private TerrainType[] regions;
    [SerializeReference] private GameObject visualizationParent;
    int[,] cellIndexMap;

    private Vector2[] points;
    public event System.Func<bool> OnGenerate;

    (Texture2D, Texture2D) GenerateVoronoi()
    {
        texture = new Texture2D(textureWidth, textureHeight);
        colorTexture = new Texture2D(textureWidth, textureHeight);
        texture.filterMode = FilterMode.Point;
        colorTexture.filterMode = FilterMode.Point;

        cellIndexMap = new int[textureWidth, textureHeight];
        points = new Vector2[numCells];
        bool[] isLand = new bool[numCells];
        float[] cellMaxHeight = new float[numCells];

        for (int i = 0; i < numCells; i++)
        {
            points[i] = new Vector2(Random.Range(0, textureWidth), Random.Range(0, textureHeight));
            isLand[i] = Random.value > waterProbability;
            cellMaxHeight[i] = Random.Range(0.8f, 1f);
        }

        for (int x = 0; x < textureWidth; x++)
        {
            for (int y = 0; y < textureHeight; y++)
            {
                int closestIndex = 0;
                float minDistance = float.MaxValue;

                for (int i = 0; i < numCells; i++)
                {
                    float dist = Vector2.SqrMagnitude(new Vector2(x, y) - points[i]);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        closestIndex = i;
                    }
                }
                cellIndexMap[x, y] = closestIndex;
            }
        }

        for (int x = 0; x < textureWidth; x++)
        {
            for (int y = 0; y < textureHeight; y++)
            {
                int index = cellIndexMap[x, y];
                float heightValue = 0f;

                if (isLand[index])
                {
                    float distToWater = GetDistanceToWater(x, y, isLand);
                    float normalized = Mathf.Clamp01(distToWater / 15f);
                    heightValue = Mathf.Pow(normalized, normalizationPower);
                    heightValue *= cellMaxHeight[index];
                }
                texture.SetPixel(x, y, new Color(heightValue, heightValue, heightValue));
                Color terrainColor = GetColorFromRegions(heightValue);
                colorTexture.SetPixel(x, y, terrainColor);
            }
        }

        texture.filterMode = FilterMode.Point;
        colorTexture.filterMode = FilterMode.Point;
        texture.Apply();
        colorTexture.Apply();
        regionVisualizationUI.texture = colorTexture;
        hightVisualizationUI.texture = texture;
        return (texture, colorTexture);
    }

    float GetDistanceToWater(int x, int y, bool[] isLand)
    {
        for (int r = 0; r < Mathf.Max(textureWidth, textureHeight); r++)
        {
            for (int dx = -r; dx <= r; dx++)
            {
                for (int dy = -r; dy <= r; dy++)
                {
                    int nx = x + dx;
                    int ny = y + dy;
                    if (nx >= 0 && nx < textureWidth && ny >= 0 && ny < textureHeight)
                    {
                        if (!isLand[cellIndexMap[nx, ny]])
                            return r;
                    }
                }
            }
        }
        return 0;
    }

    Color GetColorFromRegions(float height)
    {
        for (int i = 0; i < regions.Length; i++)
        {
            if (height <= regions[i].height)
                return regions[i].color;
        }
        return regions[regions.Length - 1].color;
    }

    public void ClearVisualization()
    {
        Destroy(visualizationParent.GetComponent<MeshFilter>());
        Destroy(visualizationParent.GetComponent<MeshRenderer>());
        meshFilters.Clear();
    }
    public void Generate(TerrainType[] _regions)
    {
        if (OnGenerate != null && OnGenerate.Invoke() == false)
            return;
        regions = _regions;
        (Texture2D voronoiHeightTexture, Texture2D voronoiRegionTexture) = GenerateVoronoi();
        MapDisplay mapDisplay = GetComponent<MapDisplay>();
        mapDisplay.DrawMesh(MeshGenerator.GenerateMesh(voronoiHeightTexture, visualizationHeightScale), voronoiRegionTexture);
    }
}
