using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GemStashController : MonoBehaviour
{
    public GemColor color;
    public int amountOfGems;
    public GameObject bank;
    public BankController bankController;
    public TextMeshProUGUI amountInfo;


    void Start()
    {
        bankController = bank.GetComponent<BankController>();
        amountOfGems = 7;

        bankController.gemStashes.Add(this);

        UnityEngine.UI.Button openBoughtCardsButton = this.gameObject.GetComponent<UnityEngine.UI.Button>();
        openBoughtCardsButton.onClick.AddListener(() => OnClick());

        amountInfo.SetText(this.amountOfGems.ToString());
    }
    private void Update()
    {
        amountInfo.SetText(this.amountOfGems.ToString());
    }

    public void OnClick()
    {
        if (bankController.gemsBeingChosen.Count == 1 && bankController.gemsBeingChosen[0] != color)
        {
            bankController.isPlayerTakingThreeGems = true;
        }

        if(bankController.gemsBeingChosen.Count == 0)
        {
            if (amountOfGems >= 1)
            {
                bankController.gemsBeingChosen.Add(color);
            }
            else Debug.Log("Nie mo縠sz wzi规 縠tonu o tym kolorze, bo ich nie ma");
        }
        else if (bankController.gemsBeingChosen.Count == 1 && bankController.isPlayerTakingThreeGems)
        {
            if(amountOfGems >= 1)
            {
                bankController.gemsBeingChosen.Add(color);
            }
            else Debug.Log("Nie mo縠sz wzi规 縠tonu o tym kolorze, bo ich nie ma");
        }
        else if(bankController.gemsBeingChosen.Count == 1 && !bankController.isPlayerTakingThreeGems)
        {
            if (amountOfGems >= 4)
            {
                bankController.gemsBeingChosen.Add(color);
                amountOfGems -= 2;
                bankController.TwoGemsTaken();
            }
            else Debug.Log("Nie mo縠sz wzi规 2 縠ton體 tego samego koloru, bo nie ma ich conajmniej 4");
        }
        else if(bankController.gemsBeingChosen.Count == 2)
        {
            if (amountOfGems >= 1 && bankController.gemsBeingChosen[0] != color && bankController.gemsBeingChosen[1] != color)
            {
                bankController.gemsBeingChosen.Add(color);
                bankController.ThreeGemsTaken();
            }
            else Debug.Log("Nie mo縠sz wzi规 縠tonu o tym kolorze, bo ich nie ma");
        }
    } 
}
