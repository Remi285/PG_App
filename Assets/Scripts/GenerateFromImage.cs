using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateFromImage : MonoBehaviour
{
    public Texture2D texture;
    private Texture2D colorTexture;
    public RawImage hightVisualizationUI;
    public RawImage regionVisualizationUI;
    public float visualizationHeightScale = 5f;
    TerrainType[] regions;
    public void Generate(TerrainType[] _regions)
    {
        regions = _regions;
        GenerateColorTexture();
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        hightVisualizationUI.texture = texture;
        colorTexture.filterMode = FilterMode.Point;
        colorTexture.Apply();
        regionVisualizationUI.texture = colorTexture;

        MapDisplay mapDisplay = GetComponent<MapDisplay>();
        mapDisplay.DrawMesh(MeshGenerator.GenerateMesh(texture, visualizationHeightScale), colorTexture);
    }

    private void GenerateColorTexture()
    {
        int x_count = 0;
        int y_count = 0;
        Debug.LogError(texture.GetPixel(200, 200));
        colorTexture = new Texture2D(texture.width, texture.height);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                float dist = texture.GetPixel(x, y).grayscale;
                //Debug.Log("X: " + x + " Y: " + y + " Dist: " + dist);
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
        Debug.LogError("X: " + x_count + " Y: " + y_count);
    }

}
