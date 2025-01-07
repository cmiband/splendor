using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageManager : MonoBehaviour
{
    [SerializeField] private GameObject[] pages;
    [SerializeField] private Button leftArrow;
    [SerializeField] private Button rightArrow;
    [SerializeField] private Button exit;
    private int currentPageIndex = 0;
    public void ShowInstruction()
    {
        UpdatePageVisibility();
        UpdateArrowsVisibility();
        exit.gameObject.SetActive(true);
    }
    public void HideInstruction()
    {
        leftArrow.gameObject.SetActive(false);
        rightArrow.gameObject.SetActive(false);
        pages[currentPageIndex].SetActive(false);
        exit.gameObject.SetActive(false);
    }
    public void ShowNextPage()
    {
        if (currentPageIndex < pages.Length - 1)
        {  
            pages[currentPageIndex].SetActive(false);
            currentPageIndex++;
            pages[currentPageIndex].SetActive(true);
            Canvas.ForceUpdateCanvases();
            UpdateArrowsVisibility();
        }
    }
    public void ShowPreviousPage()
    {
        if (currentPageIndex > 0)
        {
            pages[currentPageIndex].SetActive(false);
            currentPageIndex--;
            pages[currentPageIndex].SetActive(true);
            Canvas.ForceUpdateCanvases();
            UpdateArrowsVisibility();
        }
    }

    private void UpdatePageVisibility()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == currentPageIndex);
        }
    }

    private void UpdateArrowsVisibility()
    {
        leftArrow.gameObject.SetActive(currentPageIndex > 0);
        rightArrow.gameObject.SetActive(currentPageIndex < pages.Length - 1);
    }
}
