using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateFromImage : MonoBehaviour
{
    public Texture2D texture;
    //private Texture2D colorTexture;
    public RawImage hightVisualizationUI;
    public RawImage regionVisualizationUI;
    public float visualizationHeightScale = 5f;
    TerrainType[] regions;
    public void Generate(TerrainType[] _regions)
    {
        regions = _regions;
        var colorTexture = GenerateColorTexture(texture);
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        hightVisualizationUI.texture = texture;
        colorTexture.filterMode = FilterMode.Point;
        colorTexture.Apply();
        regionVisualizationUI.texture = colorTexture;

        MapDisplay mapDisplay = GetComponent<MapDisplay>();
        mapDisplay.DrawMesh(MeshGenerator.GenerateMesh(texture, visualizationHeightScale), colorTexture);
    }
    public void Generate(TerrainType[] _regions, Texture2D heightMap)
    {
        regions = _regions;
        var colorTexture = GenerateColorTexture(heightMap);
        heightMap.filterMode = FilterMode.Point;
        heightMap.Apply();
        hightVisualizationUI.texture = heightMap;
        colorTexture.filterMode = FilterMode.Point;
        colorTexture.Apply();
        regionVisualizationUI.texture = colorTexture;

        MapDisplay mapDisplay = GetComponent<MapDisplay>();
        mapDisplay.DrawMesh(MeshGenerator.GenerateMesh(heightMap, visualizationHeightScale), colorTexture);
    }

    private Texture2D GenerateColorTexture(Texture2D _texture)
    {
        int x_count = 0;
        int y_count = 0;
        Texture2D colorTexture = new Texture2D(_texture.width, _texture.height);
        for (int x = 0; x < _texture.width; x++)
        {
            for (int y = 0; y < _texture.height; y++)
            {
                float dist = _texture.GetPixel(x, y).grayscale;
                Color pixelColor = new();
                for (int i = 0; i < regions.Length; i++)
                {
                    if (dist <= regions[i].height)
                    {
                        pixelColor = regions[i].color;
                        break;
                    }
                }
                y_count++;
                colorTexture.SetPixel(x, y, pixelColor);
            }
            x_count++;
        }
        return colorTexture;
    }
}
