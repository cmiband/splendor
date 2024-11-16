using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemStashController : MonoBehaviour
{
    public GemColor color;
    public int amountOfGems;
    public GameObject bank;
    public BankController bankController;


    void Start()
    {
        bankController = bank.GetComponent<BankController>();
        if (color != GemColor.GOLDEN) amountOfGems = 7;
        else amountOfGems = 5;

        bankController.gemStashes.Add(this);

        UnityEngine.UI.Button openBoughtCardsButton = this.gameObject.GetComponent<UnityEngine.UI.Button>();
        openBoughtCardsButton.onClick.AddListener(() => OnClick(new EventArgs()));
    }

    public void OnClick(EventArgs e)
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
            else Console.WriteLine("Nie mo縠sz wzi规 縠tonu o tym kolorze, bo ich nie ma");
        }
        else if (bankController.gemsBeingChosen.Count == 1 && bankController.isPlayerTakingThreeGems)
        {
            if(amountOfGems >= 1)
            {
                bankController.gemsBeingChosen.Add(color);
            }
            else Console.WriteLine("Nie mo縠sz wzi规 縠tonu o tym kolorze, bo ich nie ma");
        }
        else if(bankController.gemsBeingChosen.Count == 1 && !bankController.isPlayerTakingThreeGems)
        {
            if (amountOfGems >= 3)
            {
                bankController.gemsBeingChosen.Add(color);

                bankController.TwoGemsTaken();
            }
            else Console.WriteLine("Nie mo縠sz wzi规 2 縠ton體 tego samego koloru, bo nie ma ich conajmniej 4");
        }
        else if(bankController.gemsBeingChosen.Count == 2)
        {
            if (amountOfGems >= 1)
            {
                bankController.gemsBeingChosen.Add(color);
                bankController.ThreeGemsTaken();
            }
            else Console.WriteLine("Nie mo縠sz wzi规 縠tonu o tym kolorze, bo ich nie ma");
        }

    } 


    // Update is called once per frame
    void Update()
    {
        
    }
}
