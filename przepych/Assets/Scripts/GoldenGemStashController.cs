using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenGemStashController : MonoBehaviour
{
    public GemColor color = GemColor.GOLDEN;
    public int amountOfGems;
    public GameObject bank;
    public BankController bankController;


    void Start()
    {
        bankController = bank.GetComponent<BankController>();
        amountOfGems = 5;


        UnityEngine.UI.Button openBoughtCardsButton = this.gameObject.GetComponent<UnityEngine.UI.Button>();
        openBoughtCardsButton.onClick.AddListener(() => OnClick());
    }

    public void OnClick()
    {
        if (amountOfGems >= 1)
        {
            amountOfGems -= 1;
            bankController.GoldenGemTaken();
        }
    }
}
