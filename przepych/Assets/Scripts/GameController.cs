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

    public List<GameObject> players;
    public GameObject currentPlayer;
    public GameObject nextPlayerOne;
    public GameObject nextPlayerTwo;
    public GameObject nextPlayerThree;

    public GameObject boughtCards;
    public GameObject openBoughtCards;

    // Start is called before the first frame update
    private void Start()
    {
        boardController = board.GetComponent<BoardController>();
        availableCardsController = board.GetComponent<AvailableCardsController>();

        availableCardsController.LoadCardsFromExcel("Assets/ExternalResources/KartyWykaz.xlsx");

        boardController.SetDecks(availableCardsController.level1Cards, availableCardsController.level2Cards, availableCardsController.level3Cards);
        boardController.SetCardsInStacks();
        boardController.CreateCardObjectsOnStart();
        this.CreateFourPlayersDataOnInit();

        this.players = new List<GameObject> { currentPlayer, nextPlayerOne, nextPlayerTwo, nextPlayerThree };
        this.FillPlayersWithData();

        this.AddEventListeners();
    }

    private void AddEventListeners()
    {
        UnityEngine.UI.Button openBoughtCardsButton = this.openBoughtCards.GetComponent<UnityEngine.UI.Button>();
        openBoughtCardsButton.onClick.AddListener(HandleOpenBoughtCards);
    }

    private void CreateFourPlayersDataOnInit()
    {
        for(int i = 0; i<4; i++)
        {
            List<CardController> initHand = new List<CardController>();

            this.playerIdToHand.Add(i, initHand);
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

        this.ChangeOpenButtonVisibility(false);
    }

    public void ChangeOpenButtonVisibility(bool visibility)
    {
        this.openBoughtCards.SetActive(visibility);
    }
}
