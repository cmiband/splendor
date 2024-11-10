using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public const int AMOUNT_OF_CARDS_VISIBLE_PER_LEVEL = 5;
    public const int CARD_X_OFFSET = 70;
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
    private List<CardController> level1LoadedCardControllers = new List<CardController>();
    private List<CardController> level2LoadedCardControllers = new List<CardController>();
    private List<CardController> level3LoadedCardControllers = new List<CardController>();
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
        this.level1LoadedCardControllers = level1Deck;
        this.level2LoadedCardControllers = level2Deck;
        this.level3LoadedCardControllers = level3Deck;

        this.ShuffleDecks();
    }

    public void SetCardsInStacks()
    {
        this.level1StackController.SetCardsInStack(this.level1LoadedCardControllers);
        this.level2StackController.SetCardsInStack(this.level2LoadedCardControllers);
        this.level3StackController.SetCardsInStack(this.level3LoadedCardControllers);
    }

    public void CreateCardObjectsOnStart()
    {
        this.CreateCardObjectsInSelectedContainer(this.level1VisibleCards, this.level1StackController, this.level1VisibleCardControllers);
        this.CreateCardObjectsInSelectedContainer(this.level2VisibleCards, this.level2StackController, this.level2VisibleCardControllers);
        this.CreateCardObjectsInSelectedContainer(this.level3VisibleCards, this.level3StackController, this.level3VisibleCardControllers);
    }

    private void CreateCardObjectsInSelectedContainer(GameObject cardsContainer, CardStackController targetedStack, List<CardController> visibleCards)
    {
        int currentXOffset = 0;
        for(int i = 0; i < AMOUNT_OF_CARDS_VISIBLE_PER_LEVEL; i++)
        {
            CardController cardToInsert = targetedStack.PopCardFromStack();

            this.CreateCardObject(cardToInsert, cardsContainer, currentXOffset);
            currentXOffset += CARD_X_OFFSET;

            visibleCards.Add(cardToInsert);
        }
    }

    private void CreateCardObject(CardController targetedCard, GameObject targetedVisibleCardsContainer, float xOffset)
    {
        Vector3 cardPosition = new Vector3(targetedVisibleCardsContainer.transform.position.x + xOffset, targetedVisibleCardsContainer.transform.position.y, targetedVisibleCardsContainer.transform.position.z);

        GameObject cardObject = Instantiate(this.cardPrefab, cardPosition, Quaternion.identity, targetedVisibleCardsContainer.transform);
        CardController cardController = cardObject.GetComponent<CardController>();
        cardController.InitCardData(targetedCard);
    }

    private void ShuffleDecks()
    {
        Shuffle(this.level1LoadedCardControllers);
        Shuffle(this.level2LoadedCardControllers);
        Shuffle(this.level3LoadedCardControllers);
    }

    private void Shuffle(List<CardController> deck)
    {
        System.Random random = new System.Random();

        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);

            CardController temporary = deck[i];
            deck[i] = deck[j];
            deck[j] = temporary;
        }
    }
}
