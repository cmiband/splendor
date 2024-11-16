using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerId;
    public bool BUYING_RESERVED_CARD = true;
    public bool NOT_BUYING_RESERVED_CARD = false;

    private ResourcesController resources = new ResourcesController();
    private ResourcesController bonusResources = new ResourcesController();

    public Dictionary<GemColor, int> pom = new Dictionary<GemColor, int>();
    public ResourcesController BonusResources
    {
        get { return bonusResources; }
        set { bonusResources = value; }

    }
    public List<CardController> hand;
    private List<CardController> reservedCards = new List<CardController>();
    private int reservedCardsCounter = 0;
    public int points;
    public int Points { get => points; set => points = value; }

    public int ReservedCardsCounter
    {
        set => reservedCardsCounter = value;
        get { return reservedCardsCounter; }
    }

    private void Start()
    {
        pom = resources.gems;
        this.hand = new List<CardController>();
    }

    public void TakeTwoTokens(GemColor color)
    {

        if (this.resources.gems.ContainsKey(color))
        {
            this.resources.gems[color] += 2;

        }
        else this.resources.gems.Add(color, 2);
    }
    public void TakeThreeTokens(List<GemColor> colors)
    {
        for (int i = 0; i < 3; i++)
        {
            if (this.resources.gems.ContainsKey(colors[i]))
            {
                this.resources.gems[colors[i]] += 1;

            }
            else this.resources.gems.Add(colors[i], 1);
        }
    }


    public void SetPlayerHand(List<CardController> cards)
    {
        this.hand = cards;
    }

    public List<CardController> GetPlayerHand()
    {
        return this.hand;
    }

    public void SetPlayerId(int index)
    {
        this.playerId = index;
    }
}
