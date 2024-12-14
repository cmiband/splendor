using ClosedXML.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BankController : MonoBehaviour
{
    public List<GemColor> gemsBeingChosen = new List<GemColor>();
    public List<GemColor> gemsBeingReturned = new List<GemColor>();
    public bool isPlayerTakingThreeGems;
    public GameObject currentPlayer;
    public PlayerController playerController;
    public ResourcesController resourcesController = new ResourcesController();
    public List<GemStashController> gemStashes = new List<GemStashController>();

    public string amountOfGemsInfo = "";

    void Start()
    {
        playerController = currentPlayer.GetComponent<PlayerController>();
        isPlayerTakingThreeGems = false;
        Debug.Log(resourcesController.gems.Count);
        foreach (var item in resourcesController.gems.ToList())
        {
            if (item.Key != GemColor.GOLDEN)
                resourcesController.gems[item.Key] = 7;
            else
                resourcesController.gems[item.Key] = 5;
        }

        this.amountOfGemsInfo = this.resourcesController.ToString();
    }

    public void ThreeGemsTaken()
    {
        foreach (GemColor color in gemsBeingChosen)
        {
            resourcesController.gems[color] -= 1;

            GemStashController stash = gemStashes.FirstOrDefault(s => s.color == color);
            if (stash != null)
            {
                stash.amountOfGems -= 1;
            }
        }
        playerController.TakeThreeTokens(gemsBeingChosen);
        gemsBeingChosen.Clear();
        isPlayerTakingThreeGems = false;

        this.amountOfGemsInfo = this.resourcesController.ToString();
    }

    public void TwoGemsTaken()
    {
        resourcesController.gems[gemsBeingChosen[0]] -= 2;
        playerController.TakeTwoTokens(gemsBeingChosen[0]);

        gemsBeingChosen.Clear();
        this.amountOfGemsInfo = this.resourcesController.ToString();
    }

    public void GoldenGemTaken()
    {
        resourcesController.gems[GemColor.GOLDEN] -= 1;
        playerController.TakeGoldenGem();

        this.amountOfGemsInfo = this.resourcesController.ToString();
    }
    public void AddGems(Dictionary<GemColor, int> resourcesUsed)
    {
        foreach (var resource in resourcesUsed)
        {
            if (resource.Value > 0)
            {
                this.resourcesController.AddResource(resource.Key, resource.Value);
                GemStashController matchingStash = gemStashes.Find(byColor => byColor.color == resource.Key);
                if (matchingStash != null)
                {
                    matchingStash.amountOfGems += resource.Value;
                }
                else
                {
                    Debug.LogWarning($"Brak odpowiedniego GemStashController dla koloru {resource.Key}");
                }
            }
        }

        this.amountOfGemsInfo = this.resourcesController.ToString();
    }

    public void AddGoldengem()
    {
        this.resourcesController.gems[GemColor.GOLDEN] += 1;

        this.amountOfGemsInfo = this.resourcesController.ToString();
    }
}
