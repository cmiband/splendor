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
    public GameObject cardPrefab;
    public GameObject level1Stack;
    public GameObject level2Stack;
    public GameObject level3Stack;
    public GameObject level1VisibleCards;
    public GameObject level2VisibleCards;
    public GameObject level3VisibleCards;

    private CardStackController level1StackController;
    private CardStackController level2StackController;
    private CardStackController level3StackController;
    private List<CardController> level1LoadedDeck = new List<CardController>();
    private List<CardController> level2LoadedDeck = new List<CardController>();
    private List<CardController> level3LoadedDeck = new List<CardController>();
    private List<CardController> level1VisibleCardControllers = new List<CardController>();
    private List<CardController> level2VisibleCardControllers = new List<CardController>();
    private List<CardController> level3VisibleCardControllers = new List<CardController>();

    private void Start()
    {
        this.level1StackController = this.level1Stack.GetComponent<CardStackController>();
        this.level2StackController = this.level2Stack.GetComponent<CardStackController>();
        this.level3StackController = this.level3Stack.GetComponent<CardStackController>();

    }

    public void SetVisibleCards(List<CardController> level1VisibleCards, List<CardController> level2VisibleCards, List<CardController> level3VisibleCards)
    {
        this.level1VisibleCardControllers = level1VisibleCards;
        this.level1VisibleCardControllers = level2VisibleCards;
        this.level1VisibleCardControllers = level3VisibleCards;
    }

    public void SetDecks(List<CardController> level1Deck, List<CardController> level2Deck, List<CardController> level3Deck)
    {
        this.level1LoadedDeck = level1Deck;
        this.level2LoadedDeck = level2Deck;
        this.level3LoadedDeck = level3Deck;
    }

    public void SetCardsInStacks()
    {
        this.level1StackController.SetCardsInStack(this.level1LoadedDeck);
        this.level2StackController.SetCardsInStack(this.level2LoadedDeck);
        this.level3StackController.SetCardsInStack(this.level3LoadedDeck);
    }

    public void CreateCardObjects()
    {

    }

    private void CreateCardObject(CardController card, GameObject targetedVisibleCardsContainer, float xOffset)
    {
        Vector3 cardPosition = new Vector3(targetedVisibleCardsContainer.transform.position.x + xOffset, targetedVisibleCardsContainer.transform.position.y, targetedVisibleCardsContainer.transform.position.z);

        GameObject cardObject = Instantiate(this.cardPrefab, cardPosition, Quaternion.identity, targetedVisibleCardsContainer.transform);
        CardController cardController = card.GetComponent<CardController>();
        cardController = card;
    }
}
