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
    }

    public bool TakeOne()
    {
        if (amountOfGems >= 1)
        {
            amountOfGems -= 1;
            bankController.GoldenGemTaken();
            return true;
        }
        else
        {
            return false;
        }
    }
}
