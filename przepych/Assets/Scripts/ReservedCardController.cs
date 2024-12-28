using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReservedCardController : MonoBehaviour
{
    private float CARD_X_OFFSET;
    private float CARD_X_OFFSET_OTHERS;

    public GameObject cardContainer;
    public GameController gameController;
    public GameObject game;
    public GameObject cardPrefab;
    public GameObject cardPrefabOthers;
    RectTransform rectTransform;

    public List<GameObject> reservedCards = new List<GameObject>();

    void Start()
    {
        this.CARD_X_OFFSET = this.cardPrefab.GetComponent<RectTransform>().rect.width + 10;
        this.CARD_X_OFFSET_OTHERS = this.cardPrefabOthers.GetComponent<RectTransform>().rect.width + 10;
        this.gameController = this.game.GetComponent<GameController>();
        this.cardContainer = this.gameObject;
        this.rectTransform = this.gameObject.GetComponent<RectTransform>();
    }

    private void initCards(int playerIndex)
    {
        List<CardController> cardControllers = this.gameController.playerIdToReserveHand[playerIndex];
        CreateCards(cardControllers);
    }

    private void CreateCards(List<CardController> cards)
    {
        float startXOffset = this.gameObject.transform.position.x - rectTransform.rect.width/2; 

        float currentXOffset = 0; 
        foreach (CardController card in cards)
        {
            Vector3 cardPosition = new Vector3(
                startXOffset + currentXOffset,
                cardContainer.transform.position.y,
                cardContainer.transform.position.z);

            GameObject cardObject = Instantiate(this.cardPrefab, cardPosition, Quaternion.identity, cardContainer.transform);
            CardController cardController = cardObject.GetComponent<CardController>();
            cardController.isReserved = true;
            cardController.InitCardData(card);
            cardController.gameController = this.gameController;

            this.reservedCards.Add(cardObject);
            currentXOffset += CARD_X_OFFSET;
            AddCardClickListener(cardObject, cardController);
        }
    }

    private void initCardsOthers(int playerIndex)
    {
        List<CardController> cardControllers = this.gameController.playerIdToReserveHand[playerIndex];
        CreateCardsOthers(cardControllers);
    }
    private void CreateCardsOthers(List<CardController> cards)
    {
        float startXOffset = this.gameObject.transform.position.x - rectTransform.rect.width / 2;

        float currentXOffset = 0;
        foreach (CardController card in cards)
        {
            Vector3 cardPosition = new Vector3(
                startXOffset + currentXOffset,
                cardContainer.transform.position.y,
                cardContainer.transform.position.z);

            GameObject cardObject = Instantiate(this.cardPrefabOthers, cardPosition, Quaternion.identity, cardContainer.transform);
            CardController cardController = cardObject.GetComponent<CardController>();
            cardController.isReserved = true;
            cardController.InitCardData(card);
            cardController.gameController = this.gameController;

            this.reservedCards.Add(cardObject);
            currentXOffset += CARD_X_OFFSET_OTHERS;
            AddCardClickListenerToOthers(cardObject, cardController);
        }
    }

    private void AddCardClickListener(GameObject cardGameObject, CardController cardController)
    {
        Button button = cardGameObject.GetComponent<Button>();
        if (button == null)
        {
            button = cardGameObject.AddComponent<Button>();
            button.targetGraphic = cardGameObject.GetComponent<Image>();
        }
        button.onClick.AddListener(() => gameController.SelectCard(cardController));
    }

    private void AddCardClickListenerToOthers(GameObject cardGameObject, CardController cardController)
    {
        Button button = cardGameObject.GetComponent<Button>();
        if (button == null)
        {
            button = cardGameObject.AddComponent<Button>();
            button.targetGraphic = cardGameObject.GetComponent<Image>();
        }
        button.onClick.AddListener(() => Debug.Log(cardController.ToString()));
        button.onClick.AddListener(() => cardGameObject.SetActive(true));
    }

    public void UpdateReservedCards(int playerIndex)
    {
        RemoveCardObjects(); 
        initCards(playerIndex); 
    }
    public void UpdateReservedCardsOthers(int playerIndex)
    {
        RemoveCardObjects();
        initCardsOthers(playerIndex);
    }

    private void RemoveCardObjects()
    {
        foreach (GameObject card in this.reservedCards)
        {
            Destroy(card);
        }

        this.reservedCards.Clear();
    }

}
