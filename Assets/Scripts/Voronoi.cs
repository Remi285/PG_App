using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI; // Potrzebne do pracy z komponentami UI

public class Voronoi : MonoBehaviour
{
    public int textureWidth = 256;
    public int textureHeight = 256;
    public int numCells = 20;

    public float normalizationPower = 2f;
    public RawImage rawImage;
    public GameObject visualizationCube;
    public float visualizationHeightScale = 5f;
    private List<MeshFilter> meshFilters = new();
    Texture2D texture;
    [SerializeReference] private GameObject visualizationParent;

    public void ClearVisualization()
    {
        Destroy(visualizationParent.GetComponent<MeshFilter>());
        Destroy(visualizationParent.GetComponent<MeshRenderer>());
        meshFilters.Clear();
    }
    public void Generate()
    {
        if (rawImage == null)
        {
            Debug.LogError("Raw Image nie jest przypisany!");
            return;
        }
        Texture2D voronoiTexture = GenerateVoronoiTexture();
        rawImage.texture = voronoiTexture;

        VisualizeGrid();
        CombineCubes();
    }

    Texture2D GenerateVoronoiTexture()
    {
        texture = new Texture2D(textureWidth, textureHeight);

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
                float normalizedDistance = Mathf.InverseLerp(textureWidth / 2, 0, minDistance);
                normalizedDistance = Mathf.Pow(normalizedDistance, normalizationPower);
                
                Color pixelColor = Color.Lerp(Color.black, Color.white, normalizedDistance);
                texture.SetPixel(x, y, pixelColor);
            }
        }

        // Zastosowanie zmian w teksturze
        texture.Apply();
        return texture;
    }

    private void VisualizeGrid()
    {
        for(int x = 0; x < textureWidth; x++)
        {
            for(int y = 0; y < textureHeight; y++)
            {
                GameObject clone = Instantiate(visualizationCube, 
                new Vector3(x, SampleStep(x, y) * visualizationHeightScale, y) + 
                transform.position, transform.rotation);
                meshFilters.Add(clone.GetComponent<MeshFilter>());
                clone.transform.SetParent(visualizationParent.transform);
            }
        }   
    }

    private float SampleStep(int x, int y)
    {
        float sampledFloat = texture.GetPixel(x, y).grayscale;
        return sampledFloat;
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
