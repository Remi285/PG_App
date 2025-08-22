using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
public class DiamonSquareUIController : MonoBehaviour
{
    [SerializeField] private TMP_InputField size;
    [SerializeField] private TMP_InputField roughnessText;
    [SerializeField] private Slider roughness;
    [SerializeField] private DiamondSquare diamondSquare;
    [SerializeField] private MainMenuController mainMenuController;

    private void Start()
    {
        diamondSquare.OnGenerate += GetValues;
        roughness.onValueChanged.AddListener(UpdateText);
    }
    private void UpdateText(float value)
    {
        roughnessText.text = value.ToString();
    }

    private bool GetValues()
    {
        if (!Mathf.IsPowerOfTwo(Convert.ToInt32(size.text) - 1))
        {
            mainMenuController.ShowError();
            return false;
        }
        try
        {
            diamondSquare.size = Convert.ToInt32(size.text);
            diamondSquare.roughness = roughness.value;
        }
        catch
        {
            mainMenuController.ShowError();
            return false;
        }
        return true;
    }
}
