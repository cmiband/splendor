using DocumentFormat.OpenXml.Wordprocessing;
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
    private GoldenGemStashController goldenGemStashController = new GoldenGemStashController();

    public ResourcesController BonusResources
    {
        get { return bonusResources; }
        set { bonusResources = value; }
    }
    public ResourcesController Resources { get => resources; set => resources = value; }
    public List<CardController> hand;
    public List<CardController> handReserved;
    public List<NobleController> nobleController;
    public int points;
    public int Points { get => points; set => points = value; }

    private void Start()
    {
        this.hand = new List<CardController>();
        this.mainGameController = this.game.GetComponent<GameController>();

        this.InitGemDictionary();
        this.bankController = FindObjectOfType<BankController>();
        this.goldenGemStashController = FindObjectOfType<GoldenGemStashController>();
    }


    public void HandleBuyCard()
    {
        if (mainGameController.selectedCard == null)
        {
            Debug.Log("Nie wybrano żadnej karty do zakupu.");
            return;
        }

        PlayerController player = mainGameController.currentPlayer.GetComponent<PlayerController>();
        CardController selectedCard = mainGameController.selectedCard;
        Vector3 vector = mainGameController.selectedCard.transform.position;

        if (!player.CanAffordCardWithGolden(selectedCard) && !player.CanAffordCard(selectedCard))
        {
            Debug.Log("Nie stać cię na tę kartę.");
            return;
        }

        Dictionary<GemColor, int> price = selectedCard.detailedPrice.gems;

        var resourcesUsed = new Dictionary<GemColor, int>();
        foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
        {
            resourcesUsed[color] = 0;
        }

        foreach (var gemCost in price)
        {
            int requiredAmount = gemCost.Value;

            int bonusAmount = player.BonusResources.gems.TryGetValue(gemCost.Key, out var bonus) ? bonus : 0;
            requiredAmount -= Math.Min(bonusAmount, requiredAmount);

            if (requiredAmount > 0)
            {
                int availableAmount = player.Resources.gems.TryGetValue(gemCost.Key, out var playerAmount) ? playerAmount : 0;

                if (availableAmount >= requiredAmount)
                {
                    resourcesUsed[gemCost.Key] = requiredAmount;
                }
                else
                {
                    int deficit = requiredAmount - availableAmount;

                    if (player.Resources.gems.TryGetValue(GemColor.GOLDEN, out var goldenAmount) && goldenAmount >= deficit)
                    {
                        resourcesUsed[gemCost.Key] = availableAmount;
                        resourcesUsed[GemColor.GOLDEN] += deficit;
                    }
                    else
                    {
                        Debug.Log("Nie masz wystarczających zasobów.");
                        return;
                    }
                }
            }
        }

        foreach (var resource in resourcesUsed)
        {
            if (resource.Value > 0)
            {
                player.Resources.RemoveResource(resource.Key, resource.Value);
            }
        }

        bankController.AddGems(resourcesUsed);

        player.hand.Add(CloneCard());
        player.BonusResources.AddResource(selectedCard.bonusColor);
        player.Points += selectedCard.points;
        int cardLevel = selectedCard.Level;

        switch (cardLevel)
        {
            case 1:
                mainGameController.boardController.level1VisibleCardControllers.Remove(mainGameController.selectedCard);
                Debug.Log("Kupiono kart� 1 poziomu");
                Destroy(mainGameController.selectedCard.gameObject);
                GameObject gameObject1 = Instantiate(mainGameController.boardController.cardPrefab, vector, Quaternion.identity, mainGameController.boardController.level1VisibleCards.transform);
                gameObject1.name = "Card_Level_" + cardLevel;
                CardController cardController1 = gameObject1.GetComponent<CardController>();
                cardController1.InitCardData(mainGameController.boardController.level1StackController.PopCardFromStack());
                AddCardClickListener(gameObject1, cardController1);
                break;

            case 2:
                mainGameController.boardController.level2VisibleCardControllers.Remove(mainGameController.selectedCard);
                Debug.Log("Kupiono kart� 2 poziomu");
                Destroy(mainGameController.selectedCard.gameObject);
                GameObject gameObject2 = Instantiate(mainGameController.boardController.cardPrefab, vector, Quaternion.identity, mainGameController.boardController.level2VisibleCards.transform);
                gameObject2.name = "Card_Level_" + cardLevel;
                CardController cardController2 = gameObject2.GetComponent<CardController>();
                cardController2.InitCardData(mainGameController.boardController.level1StackController.PopCardFromStack());
                AddCardClickListener(gameObject2, cardController2);
                break;

            case 3:
                mainGameController.boardController.level3VisibleCardControllers.Remove(mainGameController.selectedCard);
                Debug.Log("Kupiono kart� 3 poziomu");
                Destroy(mainGameController.selectedCard.gameObject);
                GameObject gameObject3 = Instantiate(mainGameController.boardController.cardPrefab, vector, Quaternion.identity, mainGameController.boardController.level3VisibleCards.transform);
                gameObject3.name = "Card_Level_" + cardLevel;
                CardController cardController3 = gameObject3.GetComponent<CardController>();
                cardController3.InitCardData(mainGameController.boardController.level1StackController.PopCardFromStack());
                AddCardClickListener(gameObject3, cardController3);
                break;
        }

        mainGameController.ChangeTurn();
    }


    public void HandleReserveCard()
    {
        if(handReserved.Count < 3)
        {
            if (goldenGemStashController.TakeOne())
            {
                TakeGoldenGem();
                Debug.Log("Pobrano złoty żeton");
            }
            else Debug.Log("Nie ma już złotych żetonów");

            if(mainGameController.selectedCard != null)
            {
                int cardLevel = mainGameController.selectedCard.level;
                PlayerController player = mainGameController.currentPlayer.GetComponent<PlayerController>();
                Vector3 vector = mainGameController.selectedCard.transform.position;

                mainGameController.selectedCard.isReserved = true;


                var copiedCard = CloneCard();
                player.handReserved.Add(copiedCard);

                switch (cardLevel)
                {
                    case 1:
                        mainGameController.boardController.level1VisibleCardControllers.Remove(mainGameController.selectedCard);
                        Debug.Log("Zarezerwowano kart� 1 poziomu");
                        Destroy(mainGameController.selectedCard.gameObject);
                        GameObject gameObject1 = Instantiate(mainGameController.boardController.cardPrefab, vector, Quaternion.identity, mainGameController.boardController.level1VisibleCards.transform);
                        gameObject1.name = "Card_Level_" + cardLevel;
                        CardController cardController1 = gameObject1.GetComponent<CardController>();
                        cardController1.InitCardData(mainGameController.boardController.level1StackController.PopCardFromStack());
                        AddCardClickListener(gameObject1, cardController1);
                        break;

                    case 2:
                        mainGameController.boardController.level2VisibleCardControllers.Remove(mainGameController.selectedCard);
                        Debug.Log("Zarezerwowano kart� 2 poziomu");
                        Destroy(mainGameController.selectedCard.gameObject);
                        GameObject gameObject2 = Instantiate(mainGameController.boardController.cardPrefab, vector, Quaternion.identity, mainGameController.boardController.level2VisibleCards.transform);
                        gameObject2.name = "Card_Level_" + cardLevel;
                        CardController cardController2 = gameObject2.GetComponent<CardController>();
                        cardController2.InitCardData(mainGameController.boardController.level1StackController.PopCardFromStack());
                        AddCardClickListener(gameObject2, cardController2);
                        break;

                    case 3:
                        mainGameController.boardController.level3VisibleCardControllers.Remove(mainGameController.selectedCard);
                        Debug.Log("Zarezerwowano kart� 3 poziomu");
                        Destroy(mainGameController.selectedCard.gameObject);
                        GameObject gameObject3 = Instantiate(mainGameController.boardController.cardPrefab, vector, Quaternion.identity, mainGameController.boardController.level3VisibleCards.transform);
                        gameObject3.name = "Card_Level_" + cardLevel;
                        CardController cardController3 = gameObject3.GetComponent<CardController>();
                        cardController3.InitCardData(mainGameController.boardController.level1StackController.PopCardFromStack());
                        AddCardClickListener(gameObject3, cardController3);
                        break;
                }
            }
            else if(mainGameController.selectedStack != null)
            {

                PlayerController player = mainGameController.currentPlayer.GetComponent<PlayerController>();

                CardController reservedCard = mainGameController.selectedStack.PopCardFromStack();
                reservedCard.isReserved = true;

                Vector3 vector = mainGameController.selectedStack.transform.position;

                Debug.Log($"Zarezerwowano kartę ze stosu poziomu {reservedCard.level}");

                player.handReserved.Add(reservedCard);

                GameObject gameObject = Instantiate(mainGameController.boardController.cardPrefab, vector, Quaternion.identity, player.transform);
                CardController cardController = gameObject.GetComponent<CardController>();
                cardController.InitCardData(reservedCard);

                AddCardClickListener(gameObject, cardController);
            }
            else
            {
                Debug.Log("Nie wybrano żadnej karty do zarezerwowania");
            }

            mainGameController.ChangeTurn();
        }
        else
        {
            Debug.Log("Za dużo zarezerwowałeś kart");
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

    public void SetPlayerReserveHand(List<CardController> cards)
    {
        this.handReserved = cards;
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

        foreach(KeyValuePair<GemColor, int> val in resources.gems)
        {
            if(val.Key == GemColor.NONE)
            {
                continue;
            }
            if(!this.gemColorToResourceGameObject.ContainsKey(val.Key))
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
        GameObject cardObject = Instantiate(mainGameController.selectedCard.gameObject);

        CardController clonedCard = cardObject.GetComponent<CardController>();

        clonedCard.InitCardData(mainGameController.selectedCard);

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
    public NobleController GetNoble(GameController game, PlayerController player)
    {
        foreach (var noble in game.boardController.visibleNoblesCoppied)
        {
            int canGetThisNoble = 0;

            if (player.BonusResources.gems.Count == 0)
                return null;
            else
            {
                foreach (var bonus in player.BonusResources.gems)
                {
                    var color = bonus.Key;
                    var amount = bonus.Value;
                    var nobleAmountForColor = noble.detailedPrice.gems[color];

                    if (nobleAmountForColor!=0 && amount >= nobleAmountForColor)
                        canGetThisNoble++;
                }
            }
            if (canGetThisNoble == NumberOfRequiredBonuses(noble))
                return noble;
        }
        return null;
    }
    public void SetPlayerNoble(List<NobleController> nobles)
    {
        this.nobleController = nobles;
    }
    private int NumberOfRequiredBonuses(NobleController noble)
    {
        int amount = 0;
        foreach(var kpv in noble.detailedPrice.gems)
        {
            var value = kpv.Value;
            if (value!=0)
                amount++;
        }
        return amount;
    }
}
