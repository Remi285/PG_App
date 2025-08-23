using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class NNUIController : MonoBehaviour
{
    [SerializeField] private Toggle enable_blur;
    [SerializeField] private NNGeneration nNGeneration;

    private void Start()
    {
        nNGeneration.OnGenerate += GetValues;
    }

    private void GetValues()
    {
        nNGeneration.applyBlur = enable_blur.isOn;
    }
}
