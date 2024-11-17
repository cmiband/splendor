using DocumentFormat.OpenXml.Office.CustomUI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    // Start is called before the first frame update
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
    }

    private void AddEventListeners()
    {
        UnityEngine.UI.Button openBoughtCardsButton = this.openBoughtCards.GetComponent<UnityEngine.UI.Button>();
        openBoughtCardsButton.onClick.AddListener(HandleOpenBoughtCards);

        UnityEngine.UI.Button passButton = this.pass.GetComponent<UnityEngine.UI.Button>();
        passButton.onClick.AddListener(HandlePass);
    }

    private void CreateFourPlayersDataOnInit()
    {
        for(int i = 0; i<4; i++)
        {
            List<CardController> initHand = new List<CardController>();
            ResourcesController initResources = new ResourcesController();

            PlayerController targetedPlayerController = this.players[i].GetComponent<PlayerController>();
            targetedPlayerController.SetPlayerId(i);
            this.playerIdToHand.Add(i, initHand);
            this.playerIdToResources.Add(i, initResources);
        }
    }

    private void FillPlayersWithData()
    {
        for(int i = 0; i<this.players.Count; i++)
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
    }

    public void HandlePass()
    {
        this.ChangeTurn();
    }

    public void ChangeTurn()
    {
        this.currentPlayerId = (this.currentPlayerId + 1) % 4;

        int targetedPlayerId = this.currentPlayerId;
        foreach(GameObject player in this.players)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.SetPlayerId(targetedPlayerId);
            playerController.SetPlayerHand(this.playerIdToHand[targetedPlayerId]);
            playerController.SetPlayerResources(this.playerIdToResources[targetedPlayerId]);

            targetedPlayerId = (targetedPlayerId + 1) % 4;
        }
    }

    public void UpdateTargetedPlayerResources(int playerId, ResourcesController resources)
    {
        this.playerIdToResources[playerId] = resources;
    }
}
