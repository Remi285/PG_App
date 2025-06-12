using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//na podstawie https://www.youtube.com/watch?v=4GuAV1PnurU
//oraz https://en.wikipedia.org/wiki/Diamond-square_algorithm
public class DiamondSquare : MonoBehaviour
{
    [SerializeField] private int sizePower;
    [SerializeField] private int size;
    [SerializeField, Range(0, 1)] private float roughness;
    private float saveRoughness;
    [SerializeField] private float heightScale = 1;
    private float[,] map;
    private int stepSize;
    private Texture2D texture;
    private Texture2D colorTexture;
    private TerrainType[] regions;
    public RawImage hightVisualizationUI;
    public RawImage regionVisualizationUI;
    public void Generate(TerrainType[] _regions)
    {
        regions = _regions;
        saveRoughness = roughness;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        GenerateDiamondSquare();
        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        UnityEngine.Debug.Log("Generation time: " + elapsedMilliseconds + " ms");
        roughness = saveRoughness;
        texture.filterMode = FilterMode.Point;
        colorTexture.filterMode = FilterMode.Point;
        texture.Apply();
        colorTexture.Apply();
        MapDisplay mapDisplay = GetComponent<MapDisplay>();
        mapDisplay.DrawMesh(MeshGenerator.GenerateMesh(texture, heightScale), colorTexture);
        hightVisualizationUI.texture = texture;
        regionVisualizationUI.texture = colorTexture;
    }
    private void GenerateDiamondSquare()
    {
        size = (int)Mathf.Pow(2f, (float)sizePower) + 1;
        texture = new Texture2D(size, size);
        colorTexture = new Texture2D(size, size);
        map = new float[size, size];
        stepSize = size - 1;
        map[0, 0] = Random.Range(0f, 1f);
        map[0, stepSize] = Random.Range(0f, 1f);
        map[stepSize, 0] = Random.Range(0f, 1f);
        map[stepSize, stepSize]  = Random.Range(0f, 1f);

        float dist = map[0, 0];
        Color pixelHeightColor = Color.Lerp(Color.black, Color.white, dist);
        texture.SetPixel(0, 0, pixelHeightColor);
        Color pixelColor = new();
        for(int i = 0; i < regions.Length; i++)
        {
            if(dist <= regions[i].height)
            {
                pixelColor = regions[i].color;
                break;
            }
        }
        colorTexture.SetPixel(0, 0, pixelColor);

        dist = map[0, stepSize];
        pixelHeightColor = Color.Lerp(Color.black, Color.white, dist);
        texture.SetPixel(0, stepSize, pixelHeightColor);
        pixelColor = new();
        for(int i = 0; i < regions.Length; i++)
        {
            if(dist <= regions[i].height)
            {
                pixelColor = regions[i].color;
                break;
            }
        }
        colorTexture.SetPixel(0, stepSize, pixelColor);

        dist = map[stepSize, 0];
        pixelHeightColor = Color.Lerp(Color.black, Color.white, dist);
        texture.SetPixel(stepSize, 0, pixelHeightColor);
        pixelColor = new();
        for(int i = 0; i < regions.Length; i++)
        {
            if(dist <= regions[i].height)
            {
                pixelColor = regions[i].color;
                break;
            }
        }
        colorTexture.SetPixel(stepSize, 0, pixelColor);

        dist = map[stepSize, stepSize];
        pixelHeightColor = Color.Lerp(Color.black, Color.white, dist);
        texture.SetPixel(stepSize, stepSize, pixelHeightColor);
        pixelColor = new();
        for(int i = 0; i < regions.Length; i++)
        {
            if(dist <= regions[i].height)
            {
                pixelColor = regions[i].color;
                break;
            }
        }
        colorTexture.SetPixel(stepSize, stepSize, pixelColor);

        while(stepSize > 1)
        {
            DiamondStep();
            SquareStep();
            stepSize /= 2;
            roughness /= 2;
        }
    }
    private void DiamondStep()
    {
        for(int x = 0; x < size - 1; x += stepSize)
        {
            for(int y = 0; y < size - 1; y += stepSize)
            {
                float avg = (map[x, y] + map[x + stepSize, y] + map[x, y + stepSize] + map[x + stepSize, y + stepSize]) / 4f;
                //map[x + stepSize / 2, y + stepSize / 2] = avg + Random.Range(-roughness, roughness);
                map[x + stepSize / 2, y + stepSize / 2] = Mathf.Clamp01(avg + Random.Range(-roughness, roughness));

                float dist = map[x + stepSize / 2, y + stepSize / 2];
                Color pixelHeightColor = Color.Lerp(Color.black, Color.white, dist);
                texture.SetPixel(x + stepSize / 2, y + stepSize / 2, pixelHeightColor);
                Color pixelColor = new();
                for(int i = 0; i < regions.Length; i++)
                {
                    if(dist <= regions[i].height)
                    {
                        pixelColor = regions[i].color;
                        break;
                    }
                }
                colorTexture.SetPixel(x + stepSize / 2, y + stepSize / 2, pixelColor);
            }
        }
    }
    private void SquareStep()
    {
        for (int x = 0; x <= size - 1; x += stepSize / 2)
        {
            for (int y = (x + stepSize / 2) % stepSize; y <= size - 1; y += stepSize)
            {
                float sum = 0;
                int count = 0;

                if (x - stepSize / 2 >= 0)
                {
                    sum += map[x - stepSize / 2, y];
                    count++;
                }
                if (x + stepSize / 2 <= size - 1)
                {
                    sum += map[x + stepSize / 2, y];
                    count++;
                }
                if (y - stepSize / 2 >= 0)
                {
                    sum += map[x, y - stepSize / 2];
                    count++;
                }
                if (y + stepSize / 2 <= size - 1)
                {
                    sum += map[x, y + stepSize / 2];
                    count++;
                }

                //map[x, y] = sum / count + Random.Range(-roughness, roughness);
                map[x, y] = Mathf.Clamp01(sum / count + Random.Range(-roughness, roughness));

                float dist = map[x, y];
                Color pixelHeightColor = Color.Lerp(Color.black, Color.white, dist);
                texture.SetPixel(x, y, pixelHeightColor);
                Color pixelColor = new();
                for(int i = 0; i < regions.Length; i++)
                {
                    if(dist <= regions[i].height)
                    {
                        pixelColor = regions[i].color;
                        break;
                    }
                }
                colorTexture.SetPixel(x, y, pixelColor);
            }
        }
    }
}
