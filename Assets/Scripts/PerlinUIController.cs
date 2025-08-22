using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class PerlinUIController : MonoBehaviour
{
    [SerializeField] private TMP_InputField size_x;
    [SerializeField] private TMP_InputField size_y;
    [SerializeField] private TMP_InputField noise_scale;
    [SerializeField] private TMP_InputField octaves;
    [SerializeField] private TMP_InputField lacunarity;
    [SerializeField] private TMP_InputField persistance;
    [SerializeField] private PerlinNoise perlinNoise;
    [SerializeField] private MainMenuController mainMenuController;

    private void Start()
    {
        perlinNoise.OnGenerate += GetValues;
    }

    private bool GetValues()
    {
        try
        {
            perlinNoise.textureSizeX = Convert.ToInt32(size_x.text);
            perlinNoise.textureSizeY = Convert.ToInt32(size_y.text);
            perlinNoise.noiseScale = float.Parse(noise_scale.text);
            perlinNoise.octaves = Convert.ToInt32(octaves.text);
            perlinNoise.lacunarity = float.Parse(lacunarity.text);
            perlinNoise.persistence = float.Parse(persistance.text);
        }
        catch
        {
            mainMenuController.ShowError();
            return false;
        }
        return true;
    }

}
