using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    /*private Noble[] nobles;
    private static Noble[] visibleNobles;
    public static Noble[] VisibleNobles
    {
        get => visibleNobles;
        set => visibleNobles = value;
    }*/
    public GameObject level1Stack;
    public GameObject level2Stack;
    public GameObject level3Stack;
    private CardStackController level1StackController;
    private CardStackController level2StackController;
    private CardStackController level3StackController;
    private List<CardController> level1LoadedDeck = new List<CardController>();
    private List<CardController> level2LoadedDeck = new List<CardController>();
    private List<CardController> level3LoadedDeck = new List<CardController>();
    private List<CardController> level1VisibleCards;
    private List<CardController> level2VisibleCards;
    private List<CardController> level3VisibleCards;

    private void Start()
    {
        this.level1StackController = this.level1Stack.GetComponent<CardStackController>();
        this.level2StackController = this.level2Stack.GetComponent<CardStackController>();
        this.level3StackController = this.level3Stack.GetComponent<CardStackController>();

    }

    public List<CardController> Level1VisibleCards { get => level1VisibleCards; }
    public List<CardController> Level2VisibleCards { get => level2VisibleCards; }
    public List<CardController> Level3VisibleCards { get => level3VisibleCards; }

    public void SetVisibleCards(List<CardController> level1VisibleCards, List<CardController> level2VisibleCards, List<CardController> level3VisibleCards)
    {
        this.level1VisibleCards = level1VisibleCards;
        this.level2VisibleCards = level2VisibleCards;
        this.level3VisibleCards = level3VisibleCards;
    }

    public void SetDecks(List<CardController> level1Deck, List<CardController> level2Deck, List<CardController> level3Deck)
    {
        Debug.Log("in set");
        this.level1LoadedDeck = level1Deck;
        this.level2LoadedDeck = level2Deck;
        this.level3LoadedDeck = level3Deck;
        Debug.Log("loaded decks");
        Debug.Log("set card decks");
    }

    public void SetCardsInStacks()
    {
        this.level1StackController.SetCardsInStack(this.level1LoadedDeck);
        this.level2StackController.SetCardsInStack(this.level2LoadedDeck);
        this.level3StackController.SetCardsInStack(this.level3LoadedDeck);
    }
}
