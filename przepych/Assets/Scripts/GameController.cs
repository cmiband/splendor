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
    public Dictionary<int, List<CardController>> playerIdToHand = new Dictionary<int, List<CardController>>();
    public Dictionary<int, ResourcesController> playerIdToResources = new Dictionary<int, ResourcesController>();
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
    public GameObject reserveCard;

    public CardController selectedToBuyCard;

    private void Start()
    {
        boardController = board.GetComponent<BoardController>();
        availableCardsController = board.GetComponent<AvailableCardsController>();

        availableCardsController.LoadCardsFromExcel("Assets/ExternalResources/KartyWykaz.xlsx");

        boardController.SetDecks(availableCardsController.level1Cards, availableCardsController.level2Cards, availableCardsController.level3Cards);
        boardController.SetCardsInStacks();
        boardController.CreateCardObjectsOnStart();

        this.players = new List<GameObject> { currentPlayer, nextPlayerOne, nextPlayerTwo, nextPlayerThree };
        this.CreateFourPlayersDataOnInit();
        this.FillPlayersWithData();
        this.currentPlayerId = 0;

        this.AddEventListeners();
        this.AssignClickListenersToAllCards();
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
        buyCardButton.onClick.AddListener(currentPlayer.GetComponent<PlayerController>().HandleReserveCard);
    }


    
    private void CreateFourPlayersDataOnInit()
    {
        for (int i = 0; i < 4; i++)
        {
            List<CardController> initHand = new List<CardController>();
            ResourcesController initResources = new ResourcesController();
            initResources.FillDictionaryWithZeros();

            PlayerController targetedPlayerController = this.players[i].GetComponent<PlayerController>();
            targetedPlayerController.SetPlayerId(i);
            this.playerIdToHand.Add(i, initHand);
            this.playerIdToResources.Add(i, initResources);
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
        targetedPlayerController.SetPlayerHand(this.playerIdToHand[targetedPlayerIndex]);
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
    }

    public void HandlePass()
    {
        this.ChangeTurn();
    }

    public void ChangeTurn()
    {
        Debug.Log("old player id:  " + this.currentPlayerId);
        this.currentPlayerId = (this.currentPlayerId + 1) % 4;

        int targetedPlayerId = this.currentPlayerId;
        Debug.Log("new player id   " + targetedPlayerId);
        foreach (GameObject player in this.players)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.SetPlayerId(targetedPlayerId);
            playerController.SetPlayerHand(this.playerIdToHand[targetedPlayerId]);
            playerController.SetPlayerResources(this.playerIdToResources[targetedPlayerId]);

            targetedPlayerId = (targetedPlayerId + 1) % 4;
        }

        buyCard.SetActive(false);
    }

    public void UpdateTargetedPlayerResources(int playerId, ResourcesController resources)
    {
        this.playerIdToResources[playerId] = resources;
    }

    public void SelectCard(CardController card)
    {
        if (selectedToBuyCard != null && selectedToBuyCard != card)
        {
            selectedToBuyCard.SetSelected(false);
        }

        if (selectedToBuyCard == card)
        {
            selectedToBuyCard = null;
            buyCard.SetActive(false);
        }
        else
        {
            selectedToBuyCard = card;
            selectedToBuyCard.SetSelected(true);
            buyCard.SetActive(true);
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
            Debug.Log("card price:  " + cardController.detailedPrice.ToString());
            SelectCard(cardController);
        }
    }
}
