using ClosedXML.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BankController : MonoBehaviour
{
    public List<GemColor> gemsBeingChosen = new List<GemColor>();
    public bool isPlayerTakingThreeGems;
    public GameObject currentPlayer;
    public PlayerController playerController;
    public ResourcesController resourcesController = new ResourcesController();

    public bool areGemsTaken;
    void Start()
    {
        playerController = currentPlayer.GetComponent<PlayerController>();
        isPlayerTakingThreeGems = false;
        areGemsTaken = false;
        Debug.Log(resourcesController.gems.Count);
        foreach (var item in resourcesController.gems)
        {
            if (item.Key != GemColor.GOLDEN) resourcesController.gems[item.Key] = 7;
            else resourcesController.gems[item.Key] = 5;
        }
    }

    public void ThreeGemsTaken()
    {
        foreach (GemColor color in gemsBeingChosen)
        {
            resourcesController.gems[color] -= 1;
        }
        playerController.TakeThreeTokens(gemsBeingChosen);
        areGemsTaken = true;
        gemsBeingChosen.Clear();
    }

    public void TwoGemsTaken()
    {
        resourcesController.gems[gemsBeingChosen[0]] -= 2;
        areGemsTaken = true;
        playerController.TakeTwoTokens(gemsBeingChosen[0]);
        gemsBeingChosen.Clear(); 
    }

    public void AddGoldengem()
    {
        this.resourcesController.gems[GemColor.GOLDEN] += 1;
    }
}
