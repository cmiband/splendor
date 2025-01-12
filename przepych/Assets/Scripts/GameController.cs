using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public int WIN_THRESHOLD = 15;

    public bool chooseNobleMode = false;
    public bool skipGettingNobles = false;
    public bool blockAction = false;
    public Text stageInfo;
    private int stageNumber = 1;

    public Text timerText;
    private float elapsedTime = 0f; 
    private bool isTimerRunning = false;

    public Text countdownText;
    public float countdownTime = 600f;
    private float remainingTime;

    public GameObject clickedCard;

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
    public Dictionary<int, string> playerIdToAvatar = new Dictionary<int, string>();

    private List<string> availablePlayerAvatars;

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

    public GameObject gameInfo;

    public GameObject reserveCard;
    public ReservedCardController reservedCardController;

    public ReservedCardController NextPlayerOneReservedCardController;
    public ReservedCardController NextPlayerTwoReservedCardController;
    public ReservedCardController NextPlayerThreeReservedCardController;

    public CardController selectedCard;
    public CardStackController selectedStack;

    public GameObject gameEndModal;
    public GameObject nobleChoiceInfoModal;

    private void Start()
    {
        stageInfo.text = stageNumber.ToString();
        isTimerRunning = true;
        remainingTime = countdownTime;

        boardController = board.GetComponent<BoardController>();
        availableCardsController = board.GetComponent<AvailableCardsController>();

        availableCardsController.LoadCardsFromExcel("Assets/ExternalResources/KartyWykaz.xlsx");

        this.FillAvailablePlayerAvatars();
        this.AssignPlayersToRandomAvatars();
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

    private void FillAvailablePlayerAvatars()
    {
        this.availablePlayerAvatars = new List<string>();

        for(int i = 1; i<=11; i++)
        {
            availablePlayerAvatars.Add("Avatars/awatar" + i);
        }
    }

    private void AssignPlayersToRandomAvatars()
    {
        this.playerIdToAvatar.Clear();
        List<string> shuffledAvatars = this.ShuffleAvatars(this.availablePlayerAvatars);

        for(int i = 0; i<4; i++)
        {
            this.playerIdToAvatar.Add(i, shuffledAvatars[i]);
        }
    }

    private List<string> ShuffleAvatars(List<string> avatars)
    {
        List<string> listCopy = new List<string>(avatars);

        System.Random rng = new System.Random();

        int n = listCopy.Count;
        while(n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            string value = listCopy[k];
            listCopy[k] = listCopy[n];
            listCopy[n] = value;
        }

        return listCopy;
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
        targetedPlayerController.SetPlayerPoints(this.playerIdToPoints[targetedPlayerIndex]);
        targetedPlayerController.SetPlayerAvatar(this.playerIdToAvatar[targetedPlayerIndex]);

        if (this.playerIdToNoble.ContainsKey(targetedPlayerIndex))
            targetedPlayerController.SetPlayerNoble(this.playerIdToNoble[targetedPlayerIndex]);

    }

    private void HandleOpenBoughtCards()
    {
        if(this.blockAction)
        {
            return;
        }

        BoughtCardsController boughtCardsController = this.boughtCards.GetComponent<BoughtCardsController>();
        boughtCardsController.OpenModal(this.playerIdToAvatar);
    }

    public void HandlePass()
    {
        if(actionIsTaken || blockAction)
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
        if(this.blockAction)
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

        PlayerController crntPlayerController = currentPlayer.GetComponent<PlayerController>();

        crntPlayerController.HideTooManyGemsInformation();

        bool allowChange = true;

        if(!this.skipGettingNobles)
        {
            allowChange = GettingNobles(crntPlayerController);
        }
        
        if(!allowChange)
        {
            this.skipGettingNobles = true;
            return;
        }

        this.currentPlayerId = (this.currentPlayerId + 1) % 4;

        ResetCountdown();
        if (this.currentPlayerId == 0)
        {
            stageNumber++;
            stageInfo.text = stageNumber.ToString();

            if(this.CheckWinRequirements())
            {
                this.HandleGameEnd();
                return;
            }
        }


        int targetedPlayerId = this.currentPlayerId;

        foreach (GameObject player in this.players)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.SetPlayerId(targetedPlayerId);
            playerController.SetPlayerHand(this.playerIdToHand[targetedPlayerId]);
            playerController.SetPlayerReserveHand(this.playerIdToReserveHand[targetedPlayerId]);
            playerController.SetPlayerPoints(this.playerIdToPoints[targetedPlayerId]);
            playerController.SetPlayerResources(this.playerIdToResources[targetedPlayerId], this.playerIdToBonusResources[targetedPlayerId]);
            playerController.SetPlayerAvatar(this.playerIdToAvatar[targetedPlayerId]);
            
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
        this.skipGettingNobles = false;

    }

    private void HandleGameEnd()
    {
        this.blockAction = true;
        List<int> sortedPlayerIds = this.GetSortedPlayers();

        this.gameEndModal.SetActive(true);
        this.gameEndModal.GetComponent<GameEndModalController>().InitModal(this.stageNumber, sortedPlayerIds, this.playerIdToPoints);
    }

    private bool CheckWinRequirements()
    {
        foreach(int i in this.playerIdToPoints.Keys)
        {
            if (this.playerIdToPoints[i] >= WIN_THRESHOLD)
            {
                return true;
            }
        }

        return false;
    }

    private List<int> GetSortedPlayers()
    {
        List<int> result = new List<int>();

        List<int> points = new List<int>();

        foreach(int point in this.playerIdToPoints.Values)
        {
            if(!points.Contains(point))
            {
                points.Add(point);
            }
        }
        points.Sort(delegate(int x, int y) {  return y - x; });
        
        foreach(int x in points)
        {
            result.AddRange(FindKeysByValue(this.playerIdToPoints, x));
        }

        return result;
    }

    private List<int> FindKeysByValue(Dictionary<int, int> collection, int value)
    {
        List<int> keys = new List<int>();

        foreach(int key in collection.Keys)
        {
            if (collection[key] == value)
            {
                keys.Add(key);
            }
        }

        return keys;
    }

    private bool GettingNobles(PlayerController crntPlayerController)
    {
        List<GameObject> availableNobles = crntPlayerController.GetNoble(this, crntPlayerController);

        if(availableNobles.Count == 0)
        {
            return true;
        } 


        if (availableNobles.Count == 1)
        {
            this.ChooseAndTakeOneNoble(availableNobles[0], crntPlayerController);
            return true;
        } 
        else
        {
            this.HightlightNoblesAndDisplayInfo(availableNobles);
            return false;
        }
    }

    private void ChooseAndTakeOneNoble(GameObject nobleGameObject, PlayerController crntPlayerController)
    {
        NobleController availableNobleController = nobleGameObject.GetComponent<NobleController>();
        var nobleIndex = GetNobleIndex(nobleGameObject);

        this.playerIdToNoble[this.currentPlayerId].Add(availableNobleController);

        this.boardController.createdAvailableNoblesGameObjects.RemoveAt(nobleIndex);
        crntPlayerController.points += 3;
        this.playerIdToPoints[this.currentPlayerId] += 3;

        availableNobleController.SetPlayerImage(crntPlayerController.avatar);
    }

    private void HightlightNoblesAndDisplayInfo(List<GameObject> nobles)
    {
        this.nobleChoiceInfoModal.SetActive(true);
        Color highlightColor = new Color(255, 218, 0, 74);

        for (int i = 0; i<nobles.Count; i++)
        {
            GameObject targetedNoble = nobles[i];
            Image nobleImage = targetedNoble.GetComponent<Image>();
            
            nobleImage.color = highlightColor;

            targetedNoble.GetComponent<NobleController>().canBeChosen = true;
            this.chooseNobleMode = true;
        }

        this.blockAction = true;
    }

    private void UnhighlightNobles()
    {
        List<GameObject> allNobles = this.boardController.createdAvailableNoblesGameObjects;
        Color defaultColor = new Color(255, 255, 255, 255);

        for (int i = 0; i<allNobles.Count; i++)
        {
            GameObject nobleObject = allNobles[i];

            nobleObject.GetComponent<Image>().color = defaultColor;

            nobleObject.GetComponent<NobleController>().canBeChosen = false;
        }
    }

    public void HandleNobleChoice(GameObject chosenNoble)
    {
        PlayerController currentPlayer = this.currentPlayer.GetComponent<PlayerController>();

        this.UnhighlightNobles();
        this.ChooseAndTakeOneNoble(chosenNoble, currentPlayer);

        this.nobleChoiceInfoModal.SetActive(false);
        this.blockAction = false;
        this.ChangeTurn();
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

    public void ShowClickedCard(CardController card)
    {
        if(this.blockAction)
        {
            return;
        }

        this.clickedCard.GetComponent<ClickedCardController>().SetCard(card);
        this.clickedCard.SetActive(true);
    }

    public void SelectCard(CardController card)
    {
        if(this.blockAction)
        {
            return;
        }

        if (selectedCard != null && selectedCard != card)
        {
            selectedCard.SetSelected(false);
        }

        if (selectedCard == card)
        {
            selectedCard.SetSelected(false);
            selectedCard = null;
            buyCard.SetActive(false);
            reserveCard.SetActive(false);
        }
        else if((card.ownerId==currentPlayerId && card.isReserved)||!card.isReserved)
        {
            if (selectedStack != null)
            {
                selectedStack.SetSelected(false);
                selectedStack = null;
            }
            selectedCard = card;
                selectedCard.SetSelected(true);
                buyCard.SetActive(true);
                if (selectedCard.isReserved != true) reserveCard.SetActive(true);
                else reserveCard.SetActive(false);
        }


    }

    public void SelectStack(CardStackController cardStack)
    {
        if(this.blockAction)
        {
            return;
        }

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
        if(this.blockAction)
        {
            return;
        }

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
        if(this.blockAction)
        {
            return;
        }

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

    private int GetNobleIndex(GameObject nobleObject)
    {
        for (int i=0; i<this.boardController.createdAvailableNoblesGameObjects.Count; i++)
        {
            if (this.boardController.createdAvailableNoblesGameObjects[i].Equals(nobleObject))
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
    private int GetNobleIndexInVisible(NobleController noble)
    {
        var visibleNobles = boardController.visibleNoblesListControllers;
        for (int i = 0; i < visibleNobles.Count; i++)
        {
            if (visibleNobles[i].Equals(noble))
                return i;
        }
        throw new Exception("Noble not found in visible nobles");
    }
    public void PauseGame()
    {
        isTimerRunning = false;
    }
    public void ResumeGame()
    {
        isTimerRunning = true;
    }
}
