using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

//na podstawie https://www.youtube.com/watch?v=4GuAV1PnurU
//oraz https://en.wikipedia.org/wiki/Diamond-square_algorithm
public class DiamondSquare : MonoBehaviour
{
    [SerializeField] private int sizePower;
    [SerializeField] private int size;
    [SerializeField, Range(0, 1)] private float scale;
    [SerializeField] private float heightScale = 1;
    private float[,] map;
    private int stepSize;
    [SerializeField] private GameObject visualizationCube;
    public void Generate()
    {
        GenerateDiamondSquare();
        CreateVisualization();
    }
    private void GenerateDiamondSquare()
    {
        
        size = (int)Mathf.Pow(2f, (float)sizePower) + 1; //2^n + 1
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
            scale /= 2;
        }
    }
    private void DiamondStep()
    {
        for(int x = 0; x < size - 1; x += stepSize)
        {
            for(int y = 0; y < size - 1; y += stepSize)
            {
                Debug.LogError("Diamond: " + x + " " + y);
                float avg = (map[x, y] + map[x + stepSize, y] + map[x, y + stepSize] + map[x + stepSize, y + stepSize]) / 4f; // średnia
                Debug.LogWarning("Diamond avg: " + avg);
                map[x + stepSize / 2, y + stepSize / 2] = avg + Random.Range(-scale, scale);
            }
        }
    }
    private void SquareStep()
    {
        for (int x = 0; x < size - 1; x += stepSize / 2)
        {
            for (int y = (x + stepSize / 2) % stepSize; y < size - 1; y += stepSize)
            {
                Debug.LogError("Square: " + x + " " + y);
                float avg = (map[(x - stepSize / 2 + size - 1) % (size - 1), y] + map[(x + stepSize / 2) % (size - 1), y] + map[x, (y + stepSize / 2) % (size - 1)] + map[x, (y - stepSize / 2 + size - 1) % (size - 1)]) / 4f;
                Debug.LogWarning("Square avg: " + avg);
                avg = avg + Random.Range(-scale, scale);
                map[x, y] = avg;

                if (x == 0) map[size - 1, y] = avg;
                if (y == 0) map[x, size - 1] = avg;
            }
        }
    }
    void CreateVisualization()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float height = map[x, y];
                Vector3 position = new Vector3(x, height * heightScale, y);
                GameObject terrainBlock = Instantiate(visualizationCube, position, Quaternion.identity);
                //terrainBlock.transform.localScale = new Vector3(1, height * heightScale, 1);
            }
        }
    }
}
