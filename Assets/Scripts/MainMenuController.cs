using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject perlin;
    [SerializeField] private GameObject diamonSquare;
    [SerializeField] private GameObject voronoi;
    [SerializeField] private GameObject ai;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject errorText;

    public void EnablePerlin()
    {
        perlin.SetActive(true);
        mainMenu.SetActive(false);
    }
    public void EnableDiamondSquare()
    {
        diamonSquare.SetActive(true);
        mainMenu.SetActive(false);
    }
    public void EnableVoronoi()
    {
        voronoi.SetActive(true);
        mainMenu.SetActive(false);
    }
    public void EnableAi()
    {
        ai.SetActive(true);
        mainMenu.SetActive(false);
    }
    public void GoBack()
    {
        perlin.SetActive(false);
        diamonSquare.SetActive(false);
        voronoi.SetActive(false);
        ai.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ShowError()
    {
        StartCoroutine(ErrorTextHandling());
    }

    public IEnumerator ErrorTextHandling()
    {
        errorText.SetActive(true);
        yield return new WaitForSeconds(3f);
        errorText.SetActive(false);
    }
    
}
