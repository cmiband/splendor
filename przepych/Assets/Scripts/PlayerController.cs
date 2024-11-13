using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int currentPlayerId;
    public bool BUYING_RESERVED_CARD = true;
    public bool NOT_BUYING_RESERVED_CARD = false;

    private ResourcesController resources = new ResourcesController();
    private ResourcesController bonusResources = new ResourcesController();
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
        this.hand = new List<CardController>();
    }

    public void SetPlayerHand(List<CardController> cards)
    {
        this.hand = cards;
    }

    public List<CardController> GetPlayerHand()
    {
        return this.hand;
    }
}