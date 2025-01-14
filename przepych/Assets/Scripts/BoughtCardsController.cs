using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

public class BoughtCardsController : MonoBehaviour
{
    private float CARD_X_OFFSET;
    private const int MAXIMUM_CARD_AMOUNT_IN_ROW = 8;

    public List<GameObject> players;
    public GameObject playerOneCards;
    public GameObject playerTwoCards;
    public GameObject playerThreeCards;
    public GameObject playerFourCards;
    public GameObject game;
    public GameObject cardPrefab;

    public Image playerOneAvatar;
    public Image playerTwoAvatar;
    public Image playerThreeAvatar;
    public Image playerFourAvatar;

    public GameController gameController;
    public GameObject closeButton;

    private List<GameObject> cardsCreated;

    private void InitComponents()
    {
        this.CARD_X_OFFSET = this.cardPrefab.GetComponent<RectTransform>().rect.width+5;
        this.gameController = this.game.GetComponent<GameController>();
        this.gameObject.SetActive(false);

        this.players = new List<GameObject> { playerOneCards, playerTwoCards, playerThreeCards, playerFourCards };
        this.cardsCreated = new List<GameObject>();

        this.AddListeners();
    }

    private void AddListeners()
    {
        UnityEngine.UI.Button button = this.closeButton.GetComponent<UnityEngine.UI.Button>();
        button.onClick.AddListener(HandleCloseButtonOnclick);
    }

    public void OpenModal(Dictionary<int, string> playerIdToAvatar)
    {
        this.InitComponents();

        if (this.gameController.selectedCard != null)
        {
            this.gameController.selectedCard.SetSelected(false);
            this.gameController.selectedCard = null;
        }

        this.gameObject.SetActive(true);

        this.CreateCardObjects();
        this.SetPlayersAvatars(playerIdToAvatar);
    }

    private void SetPlayersAvatars(Dictionary<int, string> playerIdToAvatar)
    {
        this.SetPlayerAvatar(playerIdToAvatar[0], playerOneAvatar);
        this.SetPlayerAvatar(playerIdToAvatar[1], playerTwoAvatar);
        this.SetPlayerAvatar(playerIdToAvatar[2], playerThreeAvatar);
        this.SetPlayerAvatar(playerIdToAvatar[3], playerFourAvatar);
    }

    private void SetPlayerAvatar(string spritePath, Image playerImage)
    {
        Sprite playerSprite = UnityEngine.Resources.Load<Sprite>(spritePath);

        playerImage.sprite = playerSprite;
    }

    public void CloseModal()
    {
        this.gameObject.SetActive(false);
        this.gameController.openBoughtCards.SetActive(true);
        this.gameController.pass.SetActive(true);
        this.gameController.buyCard.SetActive(false);
        this.gameController.reserveCard.SetActive(false);
        this.gameController.gameInfo.SetActive(true);
        this.gameController.reservedCards.SetActive(true);
        this.RemoveCardObjects();
    }

    private void CreateCardObjects()
    {
        for (int i = 0; i<4; i++)
        {
            List<CardController> cardControllers = this.gameController.playerIdToHand[i];

            this.CreateCardObjectsForOneLevel(cardControllers, i);
        }
    }

    private void CreateCardObjectsForOneLevel(List<CardController> cards, int playerIndex)
    {
        GameObject targetedPlayerPlace = this.players[playerIndex];
        RectTransform targetedPlayerPlaceRect = targetedPlayerPlace.GetComponent<RectTransform>();
        int allCardCreated = 0;
        float currentXOffset = 0;
        float currentYOffset = 0;
        int cardInRowCounter = 0;
        foreach(CardController card in cards)
        {
            if(allCardCreated > 24)
            {
                return;
            }
            if(cardInRowCounter == MAXIMUM_CARD_AMOUNT_IN_ROW)
            {
                cardInRowCounter = 0;
                currentXOffset = 0;
                currentYOffset -= (targetedPlayerPlaceRect.rect.height + 10);
            }
            GameObject cardObject = Instantiate(this.cardPrefab, new Vector3(0,0,0), Quaternion.identity, targetedPlayerPlace.transform);
            CardController cardController = cardObject.GetComponent<CardController>();
            cardController.InitCardData(card);
            cardController.gameController = this.gameController;

            RectTransform cardRectTransform = cardObject.GetComponent<RectTransform>();
            cardRectTransform.localPosition = new Vector3(currentXOffset, currentYOffset, 0);
            
            this.cardsCreated.Add(cardObject);
            currentXOffset += CARD_X_OFFSET;

            cardInRowCounter++;
            allCardCreated++;
        }
    }

    private void RemoveCardObjects()
    {
        foreach(GameObject card in this.cardsCreated)
        {
            Destroy(card);
        }

        this.cardsCreated.Clear();
    }

    private void HandleCloseButtonOnclick()
    {
        this.CloseModal();
    }
}
