using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class OneD : MonoBehaviour
{
    [SerializeField] private int length_x = 100;
    [SerializeField] private int length_y = 100;
    [SerializeField] private float amplitude = 1f;
    [SerializeField] private float scale = 10f;
    [SerializeField] private float offset = 1f;
    public GameObject visualizationCube;
    public float visualizationHeightScale = 5f;
    [SerializeReference] private GameObject visualizationParent;
    private List<MeshFilter> meshFilters = new();

    private void Start() 
    {

    }
    public void Generate() //do poprawy
    {
        offset = Random.Range(0, 99999f);
        var map = new float[length_x, length_y];
        var noise = Generate1DNoise(length_x, scale, amplitude, offset);
        VisualizeNoise(noise);
        // map = GenerateTheRestOfTheMap(noise);
        // VisualizeMap(map);
        CombineCubes();
    }

    private float[,] GenerateTheRestOfTheMap(float[] _noise)
    {
        var _map = new float[length_x, length_y];
        for(int x = 0; x < length_x; x++)
        {
            for(int y = 0; y < length_y; y++)
            {
                if(x == 0)
                    _map[x, y] = _noise[y];
                else
                {
                    _noise[y] = GenerateNextRow(_noise)[y];
                    _map[x, y] = _noise[y];
                }
            }
        }
        return _map;
    }

    public float[] GenerateNextRow(float[] _noise)
    {
        var _new_noise = new float[length_x];
        for(int i = 0; i < length_x; i++)
        {
            _new_noise[i] = _noise[i] + Random.Range(-1,1);
        }
        return _new_noise;
    }

    private float[] Generate1DNoise(int _length, float _scale, float _amplitude, float _offset)
    {
        float[] noise = new float[_length];

        for (int i = 0; i < _length; i++)
        {
            float x = i / _scale + _offset;
            noise[i] = Mathf.PerlinNoise1D(x) * _amplitude;
        }

        return noise;
    }
    private void VisualizeNoise(float[] _noise)
    {
        Debug.LogError(_noise.Length);
        for(int x = 0; x < _noise.Length; x++)
        {
            GameObject clone = Instantiate(visualizationCube, new Vector3(x, _noise[x] * visualizationHeightScale, 0) + transform.position, transform.rotation);
            meshFilters.Add(clone.GetComponent<MeshFilter>());
            clone.transform.SetParent(visualizationParent.transform);
        }
    }
    private void VisualizeMap(float[,] _map)
    {
        for(int x = 0; x < length_x; x++)
        {
            for(int y = 0; y < length_y; y++)
            {
                GameObject clone = Instantiate(visualizationCube, new Vector3(x, _map[x, y] * visualizationHeightScale, y) + transform.position, transform.rotation);
                meshFilters.Add(clone.GetComponent<MeshFilter>());
                clone.transform.SetParent(visualizationParent.transform);
            }
        }
    }
    public void ClearVisualization()
    {
        Destroy(visualizationParent.GetComponent<MeshFilter>());
        Destroy(visualizationParent.GetComponent<MeshRenderer>());
        meshFilters.Clear();
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
