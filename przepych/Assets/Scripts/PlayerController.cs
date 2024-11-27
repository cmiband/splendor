using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int playerId;
    public GameObject game;
    public GameController mainGameController;
    public BankController bankController;
    public string resourcesInfo = "";

    public Dictionary<GemColor, GameObject> gemColorToResourceGameObject = new Dictionary<GemColor, GameObject>();
    public GameObject whiteGems;
    public GameObject redGems;
    public GameObject greenGems;
    public GameObject blackGems;
    public GameObject blueGems;
    public GameObject goldGems;

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

        this.InitGemDictionary();
        this.bankController = FindObjectOfType<BankController>();
    }
    

    public void HandleBuyCard()
    {
        int cardLevel = mainGameController.selectedToBuyCard.level;
        PlayerController player = mainGameController.currentPlayer.GetComponent<PlayerController>();
        Vector3 vector = mainGameController.selectedToBuyCard.transform.position;

        if (!CanAffordCardWithGolden(mainGameController.selectedToBuyCard) && !CanAffordCard(mainGameController.selectedToBuyCard))
        {
            Debug.Log("Nie sta� ci� na t� kart�!");
            return;
        }

        Dictionary<GemColor, int> price = mainGameController.selectedToBuyCard.detailedPrice.gems;
        
        foreach(KeyValuePair<GemColor,int> keyValue in price)
        {
            Debug.Log(keyValue);
            if (keyValue.Value == 0) continue;
            else if (!player.resources.gems.ContainsKey(keyValue.Key))
            {
                RemoveGemsOneColor(GemColor.GOLDEN, keyValue.Value);
                Debug.Log($"Zap�acono z�tym �etonem w ilo�ci {keyValue.Value}");
                continue;
            }
            else if(keyValue.Value > player.resources.gems[keyValue.Key])
            {
                int requiredGoldenGems = keyValue.Value - player.resources.gems[keyValue.Key];
                if (player.resources.gems.ContainsKey(GemColor.GOLDEN))
                {
                    int goldenAmount = player.resources.gems[GemColor.GOLDEN];
                    if(goldenAmount >= requiredGoldenGems)
                    {
                        RemoveGemsOneColor(GemColor.GOLDEN, requiredGoldenGems);
                        Debug.Log($"Zap�acono z�tym �etonem w ilo�ci {requiredGoldenGems}");
                    }
                    Debug.Log($"Zap�acono normalnym  �etonem w ilo�ci {player.resources.gems[keyValue.Key]}");
                    RemoveGemsOneColor(keyValue.Key, player.resources.gems[keyValue.Key]);
                    
                    continue;
                }
            }
            if(keyValue.Key != GemColor.NONE)
            {
                RemoveGemsOneColor(keyValue.Key, keyValue.Value);
            }
        }
        bankController.AddGems();


        var copiedCard = CloneCard();
        player.hand.Add(copiedCard);

        switch (cardLevel)
        {
            case 1:
                mainGameController.boardController.level1VisibleCardControllers.Remove(mainGameController.selectedToBuyCard);
                Debug.Log("Kupiono kart� 1 poziomu");
                Destroy(mainGameController.selectedToBuyCard.gameObject);
                GameObject gameObject1 = Instantiate(mainGameController.boardController.cardPrefab, vector, Quaternion.identity, mainGameController.boardController.level1VisibleCards.transform);
                gameObject1.name = "Card_Level_" + cardLevel;
                CardController cardController1 = gameObject1.GetComponent<CardController>();
                cardController1.InitCardData(mainGameController.boardController.level1StackController.PopCardFromStack());
                AddCardClickListener(gameObject1, cardController1);
                break;

            case 2:
                mainGameController.boardController.level2VisibleCardControllers.Remove(mainGameController.selectedToBuyCard);
                Debug.Log("Kupiono kart� 2 poziomu");
                Destroy(mainGameController.selectedToBuyCard.gameObject);
                GameObject gameObject2 = Instantiate(mainGameController.boardController.cardPrefab, vector, Quaternion.identity, mainGameController.boardController.level2VisibleCards.transform);
                gameObject2.name = "Card_Level_" + cardLevel;
                CardController cardController2 = gameObject2.GetComponent<CardController>();
                cardController2.InitCardData(mainGameController.boardController.level1StackController.PopCardFromStack());
                AddCardClickListener(gameObject2, cardController2);
                break;

            case 3:
                mainGameController.boardController.level3VisibleCardControllers.Remove(mainGameController.selectedToBuyCard);
                Debug.Log("Kupiono kart� 3 poziomu");
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

    public void TakeGoldenGem()
    {
        if (this.resources.gems.ContainsKey(GemColor.GOLDEN))
        {
            this.resources.gems[GemColor.GOLDEN] += 1;
        }
        else
        {
            this.resources.gems.Add(GemColor.GOLDEN, 1);
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

        this.SetGemInfo(this.resources);
    }

    public List<CardController> GetPlayerHand()
    {
        return this.hand;
    }

    public void SetPlayerId(int index)
    {
        this.playerId = index;
    }

    private void InitGemDictionary()
    {
        this.gemColorToResourceGameObject.Add(GemColor.WHITE, whiteGems);
        this.gemColorToResourceGameObject.Add(GemColor.RED, redGems);
        this.gemColorToResourceGameObject.Add(GemColor.GREEN, greenGems);
        this.gemColorToResourceGameObject.Add(GemColor.BLACK, blackGems);
        this.gemColorToResourceGameObject.Add(GemColor.BLUE, blueGems);
        this.gemColorToResourceGameObject.Add(GemColor.GOLDEN, goldGems);
    }

    private void SetGemInfo(ResourcesController resources)
    {
        int currentPlayerId = this.mainGameController.currentPlayerId;
        if(currentPlayerId != this.playerId)
        {
            return;
        }

        foreach(KeyValuePair<GemColor, int> val in resources.gems)
        {
            if(val.Key == GemColor.NONE)
            {
                continue;
            }

            GameObject targetedContainer = this.gemColorToResourceGameObject[val.Key];
            targetedContainer.GetComponent<PlayerGemInfoController>().SetAmountOfGems(val.Value);
        }
    }

    public void AddBonusResource(GemColor bonusColor)
    {
        if (bonusColor != GemColor.NONE)
        {
            BonusResources.AddResource(bonusColor);
            Debug.Log($"Dodano bonusowy zas�b: {bonusColor}");
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

    private void RemoveGemsOneColor(GemColor color, int amount)
    {
        if (amount == 0)
        {
            return;
        }
        for (int i = 0; i < amount; i++)
        {
            this.resources.gems[color] -= 1;
            bankController.gemsBeingReturned.Add(color);
        }
    }
}
