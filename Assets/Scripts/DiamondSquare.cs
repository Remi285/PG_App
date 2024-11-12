using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

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
    [SerializeField] private GameObject visualizationCube;
    [SerializeField] private GameObject visualizationParent;

    private List<MeshFilter> meshFilters = new();
    public void Generate()
    {
        saveRoughness = roughness;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        GenerateDiamondSquare();
        CreateVisualization();
        CombineCubes();
        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        UnityEngine.Debug.Log("Generation time: " + elapsedMilliseconds + " ms");
        roughness = saveRoughness;
    }
    private void GenerateDiamondSquare()
    {
        size = (int)Mathf.Pow(2f, (float)sizePower) + 1;
        map = new float[size, size];
        stepSize = size - 1;
        map[0, 0] = Random.Range(0f, 1f);
        map[0, stepSize] = Random.Range(0f, 1f);
        map[stepSize, 0] = Random.Range(0f, 1f);
        map[stepSize, stepSize]  = Random.Range(0f, 1f);
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
                float avg = (map[x, y] + map[x + stepSize, y] + 
                map[x, y + stepSize] + map[x + stepSize, y + stepSize]) / 4f;
                map[x + stepSize / 2, y + stepSize / 2] = avg + 
                Random.Range(-roughness, roughness);
            }
        }
    }
    private void SquareStep()
    {
        for (int x = 0; x < size - 1; x += stepSize / 2)
        {
            for (int y = (x + stepSize / 2) % stepSize; y < size - 1; y += stepSize)
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

                map[x, y] = sum / count + Random.Range(-roughness, roughness);
            }
        }
    }
    private void CreateVisualization()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float height = map[x, y];
                Vector3 position = new Vector3(x, height * heightScale, y);
                GameObject clone = Instantiate(
                    visualizationCube, position, Quaternion.identity);
                clone.transform.SetParent(visualizationParent.transform);
                meshFilters.Add(clone.GetComponent<MeshFilter>());
            }
        }
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

    public void ClearVisualization()
    {
        Destroy(visualizationParent.GetComponent<MeshFilter>());
        Destroy(visualizationParent.GetComponent<MeshRenderer>());
        meshFilters.Clear();
    }
}
