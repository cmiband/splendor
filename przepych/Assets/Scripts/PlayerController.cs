using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int playerId;
    public GameObject game;
    public GameController mainGameController;
    public string resourcesInfo = "";

    private ResourcesController resources = new ResourcesController();
    private ResourcesController bonusResources = new ResourcesController();

    public ResourcesController BonusResources
    {
        get { return bonusResources; }
        set { bonusResources = value; }
    }
    public ResourcesController Resources { get => resources; set => resources = value; }
    public List<CardController> hand;
    public int points;
    public int Points { get => points; set => points = value; }

    private void Start()
    {
        this.hand = new List<CardController>();
        this.mainGameController = this.game.GetComponent<GameController>();
    }
    
    private void AddEventListeners()
    {
        
    }

    public void HandleBuyCard()
    {
        int cardLevel = mainGameController.selectedToBuyCard.level;
        PlayerController player = mainGameController.currentPlayer.GetComponent<PlayerController>();
        Vector3 vector = mainGameController.selectedToBuyCard.transform.position;

        if (!CanAffordCardWithGolden(mainGameController.selectedToBuyCard) && !CanAffordCard(mainGameController.selectedToBuyCard))
        {
            Debug.Log("Nie staæ ciê na tê kartê!");
            return;
        }

        var copiedCard = CloneCard();
        player.hand.Add(copiedCard);

        switch (cardLevel)
        {
            case 1:
                mainGameController.boardController.level1VisibleCardControllers.Remove(mainGameController.selectedToBuyCard);
                Debug.Log("Kupiono kartê 1 poziomu");
                Destroy(mainGameController.selectedToBuyCard.gameObject);
                GameObject gameObject1 = Instantiate(mainGameController.boardController.cardPrefab, vector, Quaternion.identity, mainGameController.boardController.level1VisibleCards.transform);
                gameObject1.name = "Card_Level_" + cardLevel;
                CardController cardController1 = gameObject1.GetComponent<CardController>();
                cardController1.InitCardData(mainGameController.boardController.level1StackController.PopCardFromStack());
                AddCardClickListener(gameObject1, cardController1);
                break;

            case 2:
                mainGameController.boardController.level2VisibleCardControllers.Remove(mainGameController.selectedToBuyCard);
                Debug.Log("Kupiono kartê 2 poziomu");
                Destroy(mainGameController.selectedToBuyCard.gameObject);
                GameObject gameObject2 = Instantiate(mainGameController.boardController.cardPrefab, vector, Quaternion.identity, mainGameController.boardController.level2VisibleCards.transform);
                gameObject2.name = "Card_Level_" + cardLevel;
                CardController cardController2 = gameObject2.GetComponent<CardController>();
                cardController2.InitCardData(mainGameController.boardController.level1StackController.PopCardFromStack());
                AddCardClickListener(gameObject2, cardController2);
                break;

            case 3:
                mainGameController.boardController.level3VisibleCardControllers.Remove(mainGameController.selectedToBuyCard);
                Debug.Log("Kupiono kartê 3 poziomu");
                Destroy(mainGameController.selectedToBuyCard.gameObject);
                GameObject gameObject3 = Instantiate(mainGameController.boardController.cardPrefab, vector, Quaternion.identity, mainGameController.boardController.level3VisibleCards.transform);
                gameObject3.name = "Card_Level_" + cardLevel;
                CardController cardController3 = gameObject3.GetComponent<CardController>();
                cardController3.InitCardData(mainGameController.boardController.level1StackController.PopCardFromStack());
                AddCardClickListener(gameObject3, cardController3);
                break;
        }

        mainGameController.ChangeTurn();
    }

    private void AddCardClickListener(GameObject cardGameObject, CardController cardController)
    {
        Button button = cardGameObject.GetComponent<Button>();
        if (button == null)
        {
            button = cardGameObject.AddComponent<Button>();
            button.targetGraphic = cardGameObject.GetComponent<Image>();
        }
        button.onClick.AddListener(() => mainGameController.SelectCard(cardController));
    }

    public void TakeTwoTokens(GemColor color)
    {

        if (this.resources.gems.ContainsKey(color))
        {
            this.resources.gems[color] += 2;
        }
        else
        {
            this.resources.gems.Add(color, 2);
        }

        this.ConfirmPlayerMove();
    }
    public void TakeThreeTokens(List<GemColor> colors)
    {
        for (int i = 0; i < 3; i++)
        {
            if (this.resources.gems.ContainsKey(colors[i]))
            {
                this.resources.gems[colors[i]] += 1;
            }
            else
            {
                this.resources.gems.Add(colors[i], 1);
            }
        }

        this.ConfirmPlayerMove();
    }

    private void UpdatePlayersResources()
    {
        this.mainGameController.UpdateTargetedPlayerResources(this.playerId, this.resources);
    }

    private void ConfirmPlayerMove()
    {
        this.UpdatePlayersResources();

        this.mainGameController.ChangeTurn();
    }

    public void SetPlayerHand(List<CardController> cards)
    {
        this.hand = cards;
    }

    public void SetPlayerResources(ResourcesController resources)
    {
        this.resources = resources;

        this.resourcesInfo = this.resources.ToString();
    }

    public List<CardController> GetPlayerHand()
    {
        return this.hand;
    }

    public void SetPlayerId(int index)
    {
        this.playerId = index;
    }
    public void AddBonusResource(GemColor bonusColor)
    {
        if (bonusColor != GemColor.NONE)
        {
            BonusResources.AddResource(bonusColor);
            Debug.Log($"Dodano bonusowy zasób: {bonusColor}");
        }
    }
    private bool CanAffordCard(CardController cardToBuy)
    {
        var cardPrice = cardToBuy.DetailedPrice.gems;
        var playerResources = this.resources.gems;
        var playerBonuses = this.bonusResources.gems;

        foreach (var colorOnCard in cardPrice)
        {
            GemColor color = colorOnCard.Key;
            int requiredAmount = colorOnCard.Value;

            int playerAmount = playerResources.ContainsKey(color) ? playerResources[color] : 0;
            int bonusAmount = playerBonuses.ContainsKey(color) ? playerBonuses[color] : 0;
            int totalPlayersCoins = playerAmount + bonusAmount;

            if (totalPlayersCoins < requiredAmount)
                return false;
        }
        return true;
    }
    private bool CanAffordCardWithGolden(CardController cardToBuy)
    {
        var cardPrice = cardToBuy.DetailedPrice.gems;
        var playerResources = this.resources.gems;
        var playerBonuses = this.bonusResources.gems;

        int goldTokens = playerResources.ContainsKey(GemColor.GOLDEN) ? playerResources[GemColor.GOLDEN] : 0;

        foreach (var colorOnCard in cardPrice)
        {
            GemColor color = colorOnCard.Key;
            int requiredAmount = colorOnCard.Value;

            int playerAmount = playerResources.ContainsKey(color) ? playerResources[color] : 0;
            int bonusAmount = playerBonuses.ContainsKey(color) ? playerBonuses[color] : 0;
            int totalPlayersCoins = playerAmount + bonusAmount;

            if (totalPlayersCoins < requiredAmount)
            {
                int deficit = requiredAmount - totalPlayersCoins;
                if (goldTokens >= deficit)
                    goldTokens -= deficit;
                else
                    return false;
            }
        }
        return true;
    }
    private CardController CloneCard()
    {
        GameObject cardObject = Instantiate(mainGameController.selectedToBuyCard.gameObject);

        CardController clonedCard = cardObject.GetComponent<CardController>();

        clonedCard.InitCardData(mainGameController.selectedToBuyCard);

        return clonedCard;
    }

}
