using DocumentFormat.OpenXml.Presentation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerGemInfoController : MonoBehaviour
{
    public GemColor gemColor;
    public int amountOfGems;
    public TextMeshProUGUI amountInfo;

    private void Start()
    {
        UnityEngine.UI.Button openBoughtCardsButton = this.gameObject.GetComponent<UnityEngine.UI.Button>();
        openBoughtCardsButton.onClick.AddListener(() => OnClick());
    }
    public void SetAmountOfGems(int amountOfGems)
    {
        this.amountOfGems = amountOfGems;

        amountInfo.SetText(this.amountOfGems.ToString());
    }

    private void OnClick()
    {
        if (amountOfGems > 0)
        {
            amountOfGems--;
            amountInfo.SetText(amountOfGems.ToString());
            // Tutaj dodatkowo odwo³ujemy siê do PlayerController, aby oddaæ gem do banku
            FindObjectOfType<PlayerController>().ReturnGemToBank(gemColor);
        }
    }

}
