using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardController : MonoBehaviour
{
    public const int AMOUNT_OF_CARDS_VISIBLE_PER_LEVEL = 4;
    public const int AMOUNT_OF_VISIBLE_NOBLES = 5;
    public const float GAP_SIZE = 10;
    /*private Noble[] nobles;
    private static Noble[] visibleNobles;
    public static Noble[] VisibleNobles
    {
        get => visibleNobles;
        set => visibleNobles = value;
    }*/
    public GameObject cardPrefab;
    public GameObject gemPrefab;
    public GameObject noblePrefab;
    public GameObject level1Stack;
    public GameObject level2Stack;
    public GameObject level3Stack;
    public GameObject nobles;
    public GameObject level1VisibleCards;
    public GameObject level2VisibleCards;
    public GameObject level3VisibleCards;
    public GameObject whiteGemsStack;
    public GameObject blackGemsStack;
    public GameObject greenGemsStack;
    public GameObject blueGemsStack;
    public GameObject redGemsStack;
    public GameObject goldenGemsStack;

    public CardStackController level1StackController;
    public CardStackController level2StackController;
    public CardStackController level3StackController;

    private List<CardController> level1LoadedCardControllers = new List<CardController>();
    private List<CardController> level2LoadedCardControllers = new List<CardController>();
    private List<CardController> level3LoadedCardControllers = new List<CardController>();
    public List<CardController> level1VisibleCardControllers = new List<CardController>();
    public List<CardController> level2VisibleCardControllers = new List<CardController>();
    public List<CardController> level3VisibleCardControllers = new List<CardController>();

    public List<NobleController> loadedNoblesListControllers = new List<NobleController>();
    public List<NobleController> visibleNoblesListControllers = new List<NobleController>();
    public List<NobleController> visibleNoblesCoppied = new List<NobleController>();

    public GameController gameController;

    private void Start()
    {
        this.level1StackController = this.level1Stack.GetComponent<CardStackController>();
        this.level2StackController = this.level2Stack.GetComponent<CardStackController>();
        this.level3StackController = this.level3Stack.GetComponent<CardStackController>();
    }

    public void SetDecks(List<CardController> level1Deck, List<CardController> level2Deck, List<CardController> level3Deck)
    {
        this.level1LoadedCardControllers = level1Deck;
        this.level2LoadedCardControllers = level2Deck;
        this.level3LoadedCardControllers = level3Deck;

        this.ShuffleDecks();
    }

    public void SetNobles(List<NobleController> noblesList)
    {
        this.loadedNoblesListControllers = noblesList;
        ShuffleNobles(this.loadedNoblesListControllers);
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

    public void CreateNobleObjectOnStart()
    {
        this.CreateNobles(this.nobles, this.visibleNoblesListControllers);
    }

    private void CreateCardObjectsInSelectedContainer(GameObject cardsContainer, CardStackController targetedStack, List<CardController> visibleCards)
    {
        float currentXOffset = 0;
        for(int i = 0; i < AMOUNT_OF_CARDS_VISIBLE_PER_LEVEL; i++)
        {
            CardController cardToInsert = targetedStack.PopCardFromStack();

            this.CreateCardObject(cardToInsert, cardsContainer, currentXOffset);
            currentXOffset += this.cardPrefab.GetComponent<RectTransform>().rect.width + GAP_SIZE;

            visibleCards.Add(cardToInsert);
        }
    }

    public void CreateCardObject(CardController targetedCard, GameObject targetedVisibleCardsContainer, float xOffset)
    {
        Vector3 cardPosition = new Vector3(targetedVisibleCardsContainer.transform.position.x + xOffset, targetedVisibleCardsContainer.transform.position.y, targetedVisibleCardsContainer.transform.position.z);

        GameObject cardObject = Instantiate(this.cardPrefab, cardPosition, Quaternion.identity, targetedVisibleCardsContainer.transform);
        CardController cardController = cardObject.GetComponent<CardController>();
        cardController.InitCardData(targetedCard);
        cardController.gameController = this.gameController;
        RectTransform cardRectTransform = cardObject.GetComponent<RectTransform>();
        cardObject.GetComponent<RectTransform>().localPosition = new Vector3(xOffset, cardRectTransform.localPosition.y, cardRectTransform.localPosition.z);
    }

    private void CreateNobles(GameObject noblesContainer, List<NobleController> visibleNobles)
    {
        float currentXOffset = 0;
        for (int i = 0; i < AMOUNT_OF_VISIBLE_NOBLES; i++)
        {
            NobleController nobleToInsert = this.loadedNoblesListControllers[i];

            this.CreateNobleObject(nobleToInsert, noblesContainer, currentXOffset);
            currentXOffset += 80;

            visibleNobles.Add(nobleToInsert);
        }
    }

    private void CreateNobleObject(NobleController targetedNoble, GameObject targetedVisibleNoblesContainer, float xOffset)
    {
        Vector3 noblePosition = new Vector3 (targetedVisibleNoblesContainer.transform.position.x + xOffset, targetedVisibleNoblesContainer.transform.position.y, targetedVisibleNoblesContainer.transform.position.z);
        GameObject nobleObject = Instantiate(this.noblePrefab, noblePosition, Quaternion.identity, targetedVisibleNoblesContainer.transform);
        NobleController nobleController = nobleObject.GetComponent<NobleController>();
        nobleController.InitNobleData(targetedNoble);
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
    private void ShuffleNobles(List<NobleController> deck)
    {
        System.Random random = new System.Random();

        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);

            NobleController temporary = deck[i];
            deck[i] = deck[j];
            deck[j] = temporary;
        }
    }

    public void SetGems()
    {
        SetOneGemStack(whiteGemsStack);
        SetOneGemStack(blueGemsStack);
        SetOneGemStack(greenGemsStack);
        SetOneGemStack(redGemsStack);
        SetOneGemStack(blackGemsStack);
    }

    private void SetOneGemStack(GameObject gameObject)
    {
        for (int i = 0; i < 7; i++)
        {
            Instantiate(gemPrefab).transform.parent = gameObject.transform;
        }
    }
    public void CopyNobles()
    {
        foreach (var item in visibleNoblesListControllers)
        {
            visibleNoblesCoppied.Add(item);
        }
    }
}
