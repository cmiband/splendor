using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI stageInfo;
    private int stageNumber = 1;

    public TextMeshProUGUI timerText;
    private float elapsedTime = 0f; 
    private bool isTimerRunning = false;

    public TextMeshProUGUI countdownText;
    public float countdownTime = 600f;
    private float remainingTime;

    public bool actionIsTaken = false;
    public GameObject board;
    public BoardController boardController;
    public BankController bankController;
    public AvailableCardsController availableCardsController;
    public AvailableNoblesController availableNoblesController;
    public Dictionary<int, List<CardController>> playerIdToHand = new Dictionary<int, List<CardController>>();
    public Dictionary<int, List<CardController>> playerIdToReserveHand = new Dictionary<int, List<CardController>>();
    public Dictionary<int, ResourcesController> playerIdToResources = new Dictionary<int, ResourcesController>();

    public Dictionary<int, List<NobleController>> playerIdToNoble = new Dictionary<int, List<NobleController>>();


    public Dictionary<int, ResourcesController> playerIdToBonusResources = new Dictionary<int, ResourcesController>();
    public Dictionary<int, int> playerIdToPoints = new Dictionary<int, int>();

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

    public GameObject NextPlyerOneReservedCards;
    public GameObject NextPlyerTwoReservedCards;
    public GameObject NextPlyerThreeReservedCards;

    public GameObject reserveCard;
    public ReservedCardController reservedCardController;

    public ReservedCardController NextPlayerOneReservedCardController;
    public ReservedCardController NextPlayerTwoReservedCardController;
    public ReservedCardController NextPlayerThreeReservedCardController;

    public CardController selectedCard;
    public CardStackController selectedStack;

    private void Start()
    {
        stageInfo.SetText(stageNumber.ToString());
        isTimerRunning = true;
        remainingTime = countdownTime;

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

        NextPlayerOneReservedCardController = this.NextPlyerOneReservedCards.GetComponent<ReservedCardController>();
        NextPlayerTwoReservedCardController = this.NextPlyerTwoReservedCards.GetComponent<ReservedCardController>();
        NextPlayerThreeReservedCardController = this.NextPlyerThreeReservedCards.GetComponent<ReservedCardController>();

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

    private void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;

            int hours = Mathf.FloorToInt(elapsedTime / 3600F);      
            int minutes = Mathf.FloorToInt((elapsedTime % 3600F) / 60F);
            int seconds = Mathf.FloorToInt(elapsedTime % 60F);

            timerText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }

        if (isTimerRunning && remainingTime > 0)
        {
            remainingTime -= Time.deltaTime; 
            if (remainingTime <= 0)
            {
                remainingTime = 0; 
                isTimerRunning = false; 
            }
            UpdateTimerText();
        }

        if(remainingTime == 0)
        {
            HandlePass();
        }
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

            ResourcesController initBonusResources = new ResourcesController();

            initResources.FillDictionaryWithZeros();
            initBonusResources.FillDictionaryWithZeros();

            PlayerController targetedPlayerController = this.players[i].GetComponent<PlayerController>();
            targetedPlayerController.SetPlayerId(i);
            this.playerIdToHand.Add(i, initHand);
            this.playerIdToReserveHand.Add(i, initReservedHand);
            this.playerIdToResources.Add(i, initResources);

            this.playerIdToNoble.Add(i, initNobles);

            this.playerIdToBonusResources.Add(i, initBonusResources);
            this.playerIdToPoints.Add(i, 0);

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

        targetedPlayerController.SetPlayerResources(this.playerIdToResources[targetedPlayerIndex], this.playerIdToBonusResources[targetedPlayerIndex]);

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
        if(actionIsTaken)
        {
            return;
        }

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

        if (this.currentPlayerId == 3)
            Debug.Log("new player id   " + 0);
        else
            Debug.Log("new player id   " + (this.currentPlayerId + 1));

        PlayerController crntPlayerController = currentPlayer.GetComponent<PlayerController>();

        var availableNoble = crntPlayerController.GetNoble(this, crntPlayerController);

        if (availableNoble is not null)
        {
            var nobleIndex = GetNobleIndex(availableNoble);

            NobleController addedNoble = currentPlayer.AddComponent<NobleController>();

            addedNoble.Init(availableNoble.points, availableNoble.detailedPrice, availableNoble.illustration);

            this.playerIdToNoble[this.currentPlayerId].Add(addedNoble);
            Debug.Log($"Noble added to player with id: {currentPlayerId}");

            this.boardController.visibleNoblesCoppied.RemoveAt(nobleIndex);
            Debug.Log($"Noble removed from visibleNobleCopied");

            availableNoble.assignedPlayer = crntPlayerController;
        }
        this.currentPlayerId = (this.currentPlayerId + 1) % 4;
        Debug.Log($"Current player id: {this.currentPlayerId}");

        ResetCountdown();
        if(this.currentPlayerId == 0)
        {
            stageNumber++;
            stageInfo.SetText(stageNumber.ToString());
        }


        int targetedPlayerId = this.currentPlayerId;

        foreach (GameObject player in this.players)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.SetPlayerId(targetedPlayerId);
            playerController.SetPlayerHand(this.playerIdToHand[targetedPlayerId]);
            playerController.SetPlayerReserveHand(this.playerIdToReserveHand[targetedPlayerId]);

            playerController.SetPlayerResources(this.playerIdToResources[targetedPlayerId], this.playerIdToBonusResources[targetedPlayerId]);

            if (this.playerIdToNoble.ContainsKey(targetedPlayerId))
                playerController.SetPlayerNoble(this.playerIdToNoble[targetedPlayerId]);

            targetedPlayerId = (targetedPlayerId + 1) % 4;

            playerController.PointsCounter(playerController);
        }

        reservedCardController.UpdateReservedCards(this.currentPlayerId);
        NextPlayerOneReservedCardController.UpdateReservedCardsOthers((this.currentPlayerId + 1) % 4);
        NextPlayerTwoReservedCardController.UpdateReservedCardsOthers((this.currentPlayerId + 2) % 4);
        NextPlayerThreeReservedCardController.UpdateReservedCardsOthers((this.currentPlayerId + 3) % 4);
        this.bankController.ClearSelectedGems();


        buyCard.SetActive(false);
        reserveCard.SetActive(false);

    }

    public void UpdateTargetedPlayerResources(int playerId, ResourcesController resources)
    {
        this.playerIdToResources[playerId] = resources;
    }

    public void UpdateTargetedPlayerBonusResources(int playerId, ResourcesController resources)
    {
        this.playerIdToBonusResources[playerId] = resources;
    }

    public void UpdateTargetedPlayerPoints(int playerId, int points)
    {
        this.playerIdToPoints[playerId] = points;
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
        CardController cardController = card.GetComponent<CardController>();
        if (cardController != null)
        {
            if(selectedStack != null)
            {
                selectedStack.SetSelected(false);
                selectedStack = null;
            }
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


    private void ResetCountdown()
    {
        remainingTime = countdownTime;
        isTimerRunning = true;
        UpdateTimerText();
    }
    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60F);
        int seconds = Mathf.FloorToInt(remainingTime % 60F);
        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds); 

    }
}
