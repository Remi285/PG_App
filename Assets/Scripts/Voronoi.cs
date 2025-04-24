using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Voronoi : MonoBehaviour
{
    public int textureWidth = 256;
    public int textureHeight = 256;
    public int numCells = 20;
    public int maxHeight = 20;
    public float normalizationPower = 2f;
    public RawImage hightVisualizationUI;
    public RawImage regionVisualizationUI;
    public GameObject visualizationCube;
    public float visualizationHeightScale = 5f;
    private List<MeshFilter> meshFilters = new();
    private Texture2D texture;
    private Texture2D colorTexture;
    private TerrainType[] regions;
    [SerializeReference] private GameObject visualizationParent;

    public void ClearVisualization()
    {
        Destroy(visualizationParent.GetComponent<MeshFilter>());
        Destroy(visualizationParent.GetComponent<MeshRenderer>());
        meshFilters.Clear();
    }
    public void Generate(TerrainType[] _regions)
    {
        regions = _regions;
        if (hightVisualizationUI == null || regionVisualizationUI == null)
        {
            Debug.LogError("Raw Image nie jest przypisany!");
            return;
        }
        (Texture2D voronoiHeightTexture, Texture2D voronoiRegionTexture) = GenerateVoronoiTexture();
        hightVisualizationUI.texture = voronoiHeightTexture;
        regionVisualizationUI.texture = voronoiRegionTexture;
        MapDisplay mapDisplay = GetComponent<MapDisplay>();
        mapDisplay.DrawMesh(MeshGenerator.GenerateMesh(voronoiHeightTexture, visualizationHeightScale), voronoiRegionTexture);
    }

    (Texture2D, Texture2D) GenerateVoronoiTexture()
    {
        texture = new Texture2D(textureWidth, textureHeight);
        colorTexture = new Texture2D(textureWidth, textureHeight);

        Vector2[] points = new Vector2[numCells];
        for (int i = 0; i < numCells; i++)
        {
            points[i] = new Vector2(Random.Range(0, textureWidth), Random.Range(0, textureHeight));
        }
        for (int x = 0; x < textureWidth; x++)
        {
            for (int y = 0; y < textureHeight; y++)
            {
                float minDistance = float.MaxValue;

                foreach (var point in points)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), point);
                    if (distance < minDistance)
                        minDistance = distance;
                }
                float normalizedDistance = Mathf.InverseLerp(maxHeight, 0, minDistance);
                normalizedDistance = Mathf.Pow(normalizedDistance, normalizationPower);
                
                Color pixelHeightColor = Color.Lerp(Color.black, Color.white, normalizedDistance);
                texture.SetPixel(x, y, pixelHeightColor);
                Color pixelColor = new();
                for(int i = 0; i < regions.Length; i++)
                {
                    if(normalizedDistance <= regions[i].height)
                    {
                        pixelColor = regions[i].color;
                        break;
                    }
                }
                colorTexture.SetPixel(x, y, pixelColor);

            }
        }
        texture.filterMode = FilterMode.Point;
        colorTexture.filterMode = FilterMode.Point;
        colorTexture.Apply();
        texture.Apply();
        return (texture, colorTexture);
    }

    private float SampleStep(int x, int y)
    {
        float sampledFloat = texture.GetPixel(x, y).grayscale;
        return sampledFloat;
    }   
}
