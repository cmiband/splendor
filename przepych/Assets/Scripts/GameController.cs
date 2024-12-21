using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject board;
    public BoardController boardController;
    public AvailableCardsController availableCardsController;
    public AvailableNoblesController availableNoblesController;
    public Dictionary<int, List<CardController>> playerIdToHand = new Dictionary<int, List<CardController>>();
    public Dictionary<int, List<CardController>> playerIdToReserveHand = new Dictionary<int, List<CardController>>();
    public Dictionary<int, ResourcesController> playerIdToResources = new Dictionary<int, ResourcesController>();
    public Dictionary<int, List<NobleController>> playerIdToNoble = new Dictionary<int, List<NobleController>>();

    public int currentPlayerId;

    public List<GameObject> players;
    public GameObject currentPlayer;
    public GameObject nextPlayerOne;
    public GameObject nextPlayerTwo;
    public GameObject nextPlayerThree;

    public GameObject boughtCards;
    public GameObject openBoughtCards;
    public GameObject pass;
    public GameObject buyCard;
    public GameObject reservedCards;
    public GameObject reserveCard;
    public ReservedCardController reservedCardController;

    public CardController selectedCard;
    public CardStackController selectedStack;

    private void Start()
    {
        boardController = board.GetComponent<BoardController>();
        availableCardsController = board.GetComponent<AvailableCardsController>();

        availableCardsController.LoadCardsFromExcel("Assets/ExternalResources/KartyWykaz.xlsx");

        boardController.SetDecks(availableCardsController.level1Cards, availableCardsController.level2Cards, availableCardsController.level3Cards);
        boardController.SetCardsInStacks();
        boardController.CreateCardObjectsOnStart();
        

        availableNoblesController = board.GetComponent<AvailableNoblesController>();    
        availableNoblesController.LoadNoblesFromExcel("Assets/ExternalResources/NoblesWykaz.xlsx");
        boardController.SetNobles(availableNoblesController.noblesList);
        boardController.CreateNobleObjectOnStart();
        boardController.CopyNobles();




        reservedCardController = this.reservedCards.GetComponent<ReservedCardController>();

        this.players = new List<GameObject> { currentPlayer, nextPlayerOne, nextPlayerTwo, nextPlayerThree };
        this.CreateFourPlayersDataOnInit();
        this.FillPlayersWithData(); 
        this.currentPlayerId = 0;
        this.reserveCard.SetActive(false);
        this.buyCard.SetActive(false);

        this.AddEventListeners();
        this.AssignClickListenersToAllCards();
        this.AssignClickListenersToAllCardStacks();
    }

    private void AddEventListeners()
    {
        Button openBoughtCardsButton = this.openBoughtCards.GetComponent<Button>();
        openBoughtCardsButton.onClick.AddListener(HandleOpenBoughtCards);

        Button passButton = this.pass.GetComponent<Button>();
        passButton.onClick.AddListener(HandlePass);

        Button buyCardButton = this.buyCard.GetComponent<Button>();
        buyCardButton.onClick.AddListener(currentPlayer.GetComponent<PlayerController>().HandleBuyCard);

        Button reserveCardButton = this.reserveCard.GetComponent<Button>();
        reserveCardButton.onClick.AddListener(currentPlayer.GetComponent<PlayerController>().HandleReserveCard);
    }


    
    private void CreateFourPlayersDataOnInit()
    {
        for (int i = 0; i < 4; i++)
        {
            List<CardController> initHand = new List<CardController>();
            List<CardController> initReservedHand = new List<CardController>();
            ResourcesController initResources = new ResourcesController();
            List<NobleController> initNobles = new List<NobleController>();
            initResources.FillDictionaryWithZeros();

            PlayerController targetedPlayerController = this.players[i].GetComponent<PlayerController>();
            targetedPlayerController.SetPlayerId(i);
            this.playerIdToHand.Add(i, initHand);
            this.playerIdToReserveHand.Add(i, initReservedHand);
            this.playerIdToResources.Add(i, initResources);
            this.playerIdToNoble.Add(i, initNobles);
        }
    }

    private void FillPlayersWithData()
    {
        for (int i = 0; i < this.players.Count; i++)
        {
            this.FillPlayerWithData(this.players[i], i);
        }
    }

    private void FillPlayerWithData(GameObject targetedPlayer, int targetedPlayerIndex)
    {
        PlayerController targetedPlayerController = targetedPlayer.GetComponent<PlayerController>();

        targetedPlayerController.SetPlayerId(targetedPlayerIndex);
        targetedPlayerController.SetPlayerHand(this.playerIdToHand[targetedPlayerIndex]);
        targetedPlayerController.SetPlayerReserveHand(this.playerIdToReserveHand[targetedPlayerIndex]);
        targetedPlayerController.SetPlayerResources(this.playerIdToResources[targetedPlayerIndex]);

        if (this.playerIdToNoble.ContainsKey(targetedPlayerIndex))
            targetedPlayerController.SetPlayerNoble(this.playerIdToNoble[targetedPlayerIndex]);
    }


    private void HandleOpenBoughtCards()
    {
        BoughtCardsController boughtCardsController = this.boughtCards.GetComponent<BoughtCardsController>();
        boughtCardsController.OpenModal();

        this.ChangeButtonsVisibility(false);
    }

    public void ChangeButtonsVisibility(bool visibility)
    {
        this.openBoughtCards.SetActive(visibility);
        this.pass.SetActive(visibility);
        this.buyCard.SetActive(visibility);
        this.reserveCard.SetActive(visibility);
    }

    public void HandlePass()
    {
        if (selectedStack != null)
        {
            selectedStack.SetSelected(false);
            selectedStack = null;
        }
        if (selectedCard != null)
        {
            selectedCard.SetSelected(false);
            selectedCard = null;
        }

        this.ChangeTurn();
    }

    public void ChangeTurn()
    {
        if(this.currentPlayerId == 3)
            Debug.Log("new player id   " + 0);
        else
            Debug.Log("new player id   " + (this.currentPlayerId+1));

        PlayerController crntPlayerController = currentPlayer.GetComponent<PlayerController>();
        var availableNoble = crntPlayerController.GetNoble(this, crntPlayerController);

        if (availableNoble is not null)
        {
            var nobleIndex = GetNobleIndex(availableNoble);
            var addedNoble = new NobleController(availableNoble.points, availableNoble.detailedPrice, availableNoble.illustration);
            this.playerIdToNoble[currentPlayerId].Add(addedNoble);
            this.boardController.visibleNoblesCoppied.Remove(this.boardController.visibleNoblesCoppied[nobleIndex]);
        }
        
        this.currentPlayerId = (this.currentPlayerId + 1) % 4;
        int targetedPlayerId = this.currentPlayerId;
        foreach (GameObject player in this.players)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.SetPlayerId(targetedPlayerId);
            playerController.SetPlayerHand(this.playerIdToHand[targetedPlayerId]);
            playerController.SetPlayerReserveHand(this.playerIdToReserveHand[targetedPlayerId]);
            playerController.SetPlayerResources(this.playerIdToResources[targetedPlayerId]);

            if (this.playerIdToNoble.ContainsKey(targetedPlayerId))
                playerController.SetPlayerNoble(this.playerIdToNoble[targetedPlayerId]);
            targetedPlayerId = (targetedPlayerId + 1) % 4;
        }

        reservedCardController.UpdateReservedCards(this.currentPlayerId);

        buyCard.SetActive(false);
        reserveCard.SetActive(false);
    }
    public void UpdateTargetedPlayerResources(int playerId, ResourcesController resources)
    {
        this.playerIdToResources[playerId] = resources;
    }

    public void SelectCard(CardController card)
    {
        if (selectedCard != null && selectedCard != card)
        {
            selectedCard.SetSelected(false);
        }

        if (selectedCard == card)
        {
            selectedCard = null;
            buyCard.SetActive(false);
            reserveCard.SetActive(false);
        }
        else
        {
            selectedCard = card;
            selectedCard.SetSelected(true);
            buyCard.SetActive(true);
            if(selectedCard.isReserved != true) reserveCard.SetActive(true);
        }
    }

    public void SelectStack(CardStackController cardStack)
    {
        if (selectedStack != null && selectedStack != cardStack)
        {
            selectedStack.SetSelected(false);
        }

        if (selectedStack == cardStack)
        {
            selectedStack = null;
            reserveCard.SetActive(false);
        }
        else
        {
            selectedStack = cardStack;
            selectedStack.SetSelected(true);
            reserveCard.SetActive(true);
        }
    }

    private void AssignClickListenersToAllCards()
    {
        Transform visibleCardsRoot = board.transform.Find("VisibleCards");
        if (visibleCardsRoot == null)
        {
            Debug.LogError("VisibleCards root not found!");
            return;
        }

        foreach (Transform levelTransform in visibleCardsRoot)
        {
            Debug.Log($"Assigning listeners for level: {levelTransform.name}");

            foreach (Transform cardTransform in levelTransform)
            {
                Button button = cardTransform.GetComponent<Button>();
                if (button == null)
                {
                    button = cardTransform.gameObject.AddComponent<Button>();
                    button.targetGraphic = cardTransform.GetComponent<Image>();
                }

                button.onClick.AddListener(() => HandleCardClick(cardTransform.gameObject));
            }
        }
    }

    private void HandleCardClick(GameObject card)
    {
        Debug.Log($"Clicked on card: {card.name}");
        CardController cardController = card.GetComponent<CardController>();
        if (cardController != null)
        {
            if(selectedStack != null)
            {
                selectedStack.SetSelected(false);
                selectedStack = null;
            }
            Debug.Log("card price:  " + cardController.detailedPrice.ToString());
            SelectCard(cardController);
        }
    }

    private void AssignClickListenersToAllCardStacks()
    {
        Transform visibleCardStacksRoot = board.transform.Find("CardStacks");
        if (visibleCardStacksRoot == null)
        {
            Debug.LogError("visibleCardStacks root not found!");
            return;
        }

        foreach (Transform stackTransform in visibleCardStacksRoot)
        {
            Debug.Log($"Assigning listeners for stacks: {stackTransform.name}");

            Button button = stackTransform.GetComponent<Button>();
            if (button == null)
            {
                button = stackTransform.gameObject.GetComponent<Button>();
                button.targetGraphic = stackTransform.GetComponent<Image>();
            }

            button.onClick.AddListener(() => HandleCardStackClick(stackTransform.gameObject));
        }
    }

    private void HandleCardStackClick (GameObject cardStack)
    {
        Debug.Log($"Clicked on card: {cardStack.name}");
        CardStackController cardStackController = cardStack.GetComponent<CardStackController>();
        if (cardStackController != null)
        {
            if (selectedCard != null)
            {
                selectedCard.SetSelected(false);
                selectedCard = null;
            }
            buyCard.SetActive(false);
            if (selectedCard != null)selectedCard.isSelected = false;
            SelectStack(cardStackController);
        }
    }
    private int GetNobleIndex(NobleController noble)
    {
        var visibleNobles = boardController.visibleNoblesCoppied;
        for (int i=0; i<visibleNobles.Count; i++)
        {
            if (visibleNobles[i].Equals(noble))
                return i;
        }
        throw new Exception("Noble not found in visible nobles");
    }
}
