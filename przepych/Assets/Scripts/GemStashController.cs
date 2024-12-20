using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

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
        if(bankController.currentMode == MODE.GIVE)
        {

        } else if(bankController.currentMode == MODE.TAKE && this.color != GemColor.GOLDEN)
        {
            this.HandleTakeGem();
        }
    } 

    private void HandleGiveGem()
    {

    }

    private void HandleTakeGem()
    {
        List<GemColor> colorsAlreadyChosen = bankController.gemsBeingChosen;
        ResourcesController gemsInBank = bankController.resourcesController;
        int amountOfGemsChosen = colorsAlreadyChosen.Count;
        int amountOfAvailableStacks = this.CheckAmountOfAvailableStashes(gemsInBank);
        Debug.Log("amount of av stacks  " + amountOfAvailableStacks);

        if (amountOfGemsChosen == 0 && this.amountOfGems > 0)
        {
            bankController.gemsBeingChosen.Add(this.color);

            if (this.amountOfGems < 4 && amountOfAvailableStacks == 1)
            {
                bankController.TakeGems();

                return;
            }

            return;
        }
        if (amountOfGemsChosen == 1)
        {
            if (colorsAlreadyChosen[0] == this.color && this.amountOfGems >= 4)
            {
                bankController.gemsBeingChosen.Add(this.color);
                bankController.TakeGems();

                return;
            }

            if (colorsAlreadyChosen[0] != this.color && this.amountOfGems >= 1)
            {
                bankController.gemsBeingChosen.Add(this.color);

                if (amountOfAvailableStacks == 2)
                {
                    bankController.TakeGems();

                    return;
                }

                return;
            }
        }

        if (amountOfGemsChosen == 2)
        {
            if (colorsAlreadyChosen.Contains(this.color))
            {
                this.HandleInvalidOperation();

                return;
            }

            if (this.amountOfGems >= 1)
            {
                bankController.gemsBeingChosen.Add(this.color);
                bankController.TakeGems();

                return;
            }
        }

        this.HandleInvalidOperation();
    }

    private void HandleInvalidOperation()
    {
        bankController.gemsBeingChosen.Clear();

        Debug.Log("Nieodpowiednia operacja, �etony nie zosta�y pobrane");
    }

    private int CheckAmountOfAvailableStashes(ResourcesController resources)
    {
        int result = 0;
        foreach(GemColor color in resources.gems.Keys)
        {
            if(color == GemColor.GOLDEN || color == GemColor.NONE)
            {
                continue;
            }

            if(resources.gems.ContainsKey(color))
            {
                result += resources.gems[color] > 0 ? 1 : 0;
            }
        }
        return result;
    }
}
