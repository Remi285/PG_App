using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorleyUIController : MonoBehaviour
{
    [SerializeField] private TMP_InputField size_x;
    [SerializeField] private TMP_InputField size_y;
    [SerializeField] private TMP_InputField cellNum;
    [SerializeField] private TMP_InputField normalizationPower;
    [SerializeField] private Slider waterProbability;
    [SerializeField] private TMP_InputField waterProbability_text;
    [SerializeField] private Worley worley;
    [SerializeField] private MainMenuController mainMenuController;

    private void Start()
    {
        worley.OnGenerate += GetValues;
        waterProbability.onValueChanged.AddListener(UpdateText);
    }
    private void UpdateText(float value)
    {
        waterProbability_text.text = value.ToString();
    }

    private bool GetValues()
    {
        try
        {
            worley.textureWidth = Convert.ToInt32(size_x.text);
            worley.textureHeight = Convert.ToInt32(size_y.text);
            worley.numCells = Convert.ToInt32(cellNum.text);
            //worley.normalizationPower = float.Parse(normalizationPower.text);
            worley.waterProbability = waterProbability.value;
        }
        catch
        {
            mainMenuController.ShowError();
            return false;
        }
        return true;
    }

}
