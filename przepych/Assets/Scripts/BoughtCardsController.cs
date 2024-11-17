using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office.CustomUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

public class BoughtCardsController : MonoBehaviour
{
    private float CARD_X_OFFSET;

    public List<GameObject> players;
    public GameObject playerOneCards;
    public GameObject playerTwoCards;
    public GameObject playerThreeCards;
    public GameObject playerFourCards;
    public GameObject game;
    public GameObject cardPrefab;

    public GameController gameController;
    public GameObject closeButton;

    List<GameObject> cardsCreated;

    private void InitComponents()
    {
        this.CARD_X_OFFSET = this.cardPrefab.GetComponent<RectTransform>().rect.width+10;
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

    public void OpenModal()
    {
        this.InitComponents();

        this.gameObject.SetActive(true);

        this.CreateCardObjects();
    }

    public void CloseModal()
    {
        this.gameObject.SetActive(false);
        this.gameController.ChangeButtonsVisibility(true);

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
        float currentXOffset = 0;
        foreach(CardController card in cards)
        {
            Vector3 cardPosition = new Vector3(targetedPlayerPlace.transform.position.x + currentXOffset, targetedPlayerPlace.transform.position.y, targetedPlayerPlace.transform.position.z);

            GameObject cardObject = Instantiate(this.cardPrefab, cardPosition, Quaternion.identity, targetedPlayerPlace.transform);
            CardController cardController = cardObject.GetComponent<CardController>();
            cardController.InitCardData(card);

            this.cardsCreated.Add(cardObject);
            currentXOffset += CARD_X_OFFSET;
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
