using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    public const int MAXIMUM_AMOUNT_OF_GEMS = 10;
    public Text pointsText;

    public int playerId;
    public Image avatar;
    public GameObject game;
    public GameController mainGameController;
    public BankController bankController;
    
    public string resourcesInfo = "";
    public string bonusResourcesInfo = "";

    public GameObject TooManyGemsAlert;
    public Text TooManyGemsAlertText;
    AlertController alertController = new AlertController();

    public Dictionary<GemColor, GameObject> gemColorToResourceGameObject = new Dictionary<GemColor, GameObject>();
    public GameObject whiteGems;
    public GameObject redGems;
    public GameObject greenGems;
    public GameObject blackGems;
    public GameObject blueGems;
    public GameObject goldGems;
    public GameObject goldenGemStash;

    public Text bonusWhiteGems;
    public Text bonusRedGems;
    public Text bonusGreenGems;
    public Text bonusBlackGems;
    public Text bonusBlueGems;

    public ResourcesController resources = new ResourcesController();
    private ResourcesController bonusResources = new ResourcesController();

    public ResourcesController BonusResources
    {
        get { return bonusResources; }
        set { bonusResources = value; }
    }
    public ResourcesController Resources { get => resources; set => resources = value; }
    public List<CardController> hand;
    public List<CardController> handReserved;
    public List<NobleController> nobleController;
    public GemStashController goldenGemStashController = new GemStashController();
    public int points;
    public int Points { get => points; set => points = value; }

    private void Start()
    {
        this.hand = new List<CardController>();
        this.mainGameController = this.game.GetComponent<GameController>();
        this.goldenGemStashController = this.goldenGemStash.GetComponent<GemStashController>();

        this.InitGemDictionary();
        this.bankController = FindObjectOfType<BankController>();
    }
    public void Update()
    {
        pointsText.text = this.points.ToString();
    }
    public void PointsCounter(PlayerController player)
    {
        player.bonusWhiteGems.text = Convert.ToString(player.BonusResources.gems[GemColor.WHITE]);
        player.bonusRedGems.text = Convert.ToString(player.BonusResources.gems[GemColor.RED]);
        player.bonusGreenGems.text = Convert.ToString(player.BonusResources.gems[GemColor.GREEN]);
        player.bonusBlackGems.text = Convert.ToString(player.BonusResources.gems[GemColor.BLACK]);
        player.bonusBlueGems.text = Convert.ToString(player.BonusResources.gems[GemColor.BLUE]);
    }

    public void HandleBuyCard()
    {
        if (this.mainGameController.actionIsTaken || this.mainGameController.blockAction)
        {
            return;
        }

        if (mainGameController.selectedCard == null)
        {
            return;
        }

        PlayerController player = mainGameController.clientPlayer.GetComponent<PlayerController>();
        CardController selectedCard = mainGameController.selectedCard;
        Vector3 vector = mainGameController.selectedCard.transform.position;

        if (!player.CanAffordCardWithGolden(selectedCard) && !player.CanAffordCard(selectedCard))
        {
            alertController = FindObjectOfType<AlertController>();
            alertController.ShowNotEnoughGems();
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
        player.AddBonusResource(selectedCard.bonusColor);
        player.Points += selectedCard.points;
        int cardLevel = selectedCard.Level;

        if(selectedCard.isReserved)
        {
            player.handReserved.Remove(selectedCard);
            mainGameController.reservedCardController.reservedCards.Remove(mainGameController.selectedCard.gameObject);
            Debug.Log("Kupiono zarezerwowaną kartę");
            Destroy(mainGameController.selectedCard.gameObject);

        }
        else
        {
            switch (cardLevel)
            {
                case 1:
                    mainGameController.boardController.level1CardGameObjects.Remove(mainGameController.selectedCard.gameObject);
                    mainGameController.boardController.level1VisibleCardControllers.Remove(mainGameController.selectedCard);
                    Debug.Log("Kupiono kart� 1 poziomu");
                    Destroy(mainGameController.selectedCard.gameObject);

                    if(mainGameController.boardController.level1StackController.CheckCardsCount() == 0 )
                    {
                        break;
                    }

                    GameObject gameObject1 = Instantiate(mainGameController.boardController.cardPrefab, vector, Quaternion.identity, mainGameController.boardController.level1VisibleCards.transform);
                    gameObject1.name = "Card_Level_" + cardLevel;
                    CardController cardController1 = gameObject1.GetComponent<CardController>();
                    cardController1.InitCardData(mainGameController.boardController.level1StackController.PopCardFromStack());
                    cardController1.gameController = mainGameController;
                    AddCardClickListener(gameObject1, cardController1);

                    this.mainGameController.boardController.level1CardGameObjects.Add(gameObject1);
                    this.mainGameController.boardController.level1VisibleCardControllers.Add(cardController1);
                    break;

                case 2:
                    mainGameController.boardController.level2CardGameObjects.Remove(mainGameController.selectedCard.gameObject);
                    mainGameController.boardController.level2VisibleCardControllers.Remove(mainGameController.selectedCard);
                    Debug.Log("Kupiono kart� 2 poziomu");
                    Destroy(mainGameController.selectedCard.gameObject);

                    if (mainGameController.boardController.level2StackController.CheckCardsCount() == 0)
                    {
                        break;
                    }

                    GameObject gameObject2 = Instantiate(mainGameController.boardController.cardPrefab, vector, Quaternion.identity, mainGameController.boardController.level2VisibleCards.transform);
                    gameObject2.name = "Card_Level_" + cardLevel;
                    CardController cardController2 = gameObject2.GetComponent<CardController>();
                    cardController2.InitCardData(mainGameController.boardController.level2StackController.PopCardFromStack());
                    cardController2.gameController = mainGameController;
                    AddCardClickListener(gameObject2, cardController2);
                   
                    this.mainGameController.boardController.level2CardGameObjects.Add(gameObject2);
                    this.mainGameController.boardController.level2VisibleCardControllers.Add(cardController2);

                    break;

                case 3:
                    mainGameController.boardController.level3CardGameObjects.Remove(mainGameController.selectedCard.gameObject);
                    mainGameController.boardController.level3VisibleCardControllers.Remove(mainGameController.selectedCard);
                    Debug.Log("Kupiono kart� 3 poziomu");
                    Destroy(mainGameController.selectedCard.gameObject);

                    if (mainGameController.boardController.level3StackController.CheckCardsCount() == 0)
                    {
                        break;
                    }

                    GameObject gameObject3 = Instantiate(mainGameController.boardController.cardPrefab, vector, Quaternion.identity, mainGameController.boardController.level3VisibleCards.transform);
                    gameObject3.name = "Card_Level_" + cardLevel;
                    CardController cardController3 = gameObject3.GetComponent<CardController>();
                    cardController3.InitCardData(mainGameController.boardController.level3StackController.PopCardFromStack());
                    cardController3.gameController = mainGameController;
                    AddCardClickListener(gameObject3, cardController3);

                    this.mainGameController.boardController.level3CardGameObjects.Add(gameObject3);
                    this.mainGameController.boardController.level3VisibleCardControllers.Add(cardController3);

                    break;
            }
        }
        this.ConfirmPlayerMove();
    }

    public void HandleReserveCard()
    {
        if(this.mainGameController.actionIsTaken || this.mainGameController.blockAction || this.mainGameController.boughtCards.activeInHierarchy == true)
        {
            return;
        }
        if (this.mainGameController.selectedStack != null && mainGameController.selectedStack.CheckCardsCount() <= 0) return;

        if (handReserved.Count < 3)
        {
            if(mainGameController.selectedCard != null)
            {
                mainGameController.selectedCard.ownerId = mainGameController.currentPlayerId;
                int cardLevel = mainGameController.selectedCard.level;
                PlayerController player = this;
                Vector3 vector = mainGameController.selectedCard.transform.position;

                mainGameController.selectedCard.isReserved = true;


                var copiedCard = CloneCard();
                player.handReserved.Add(copiedCard);

                switch (cardLevel)
                {
                    case 1:
                        mainGameController.boardController.level1CardGameObjects.Remove(mainGameController.selectedCard.gameObject);
                        mainGameController.boardController.level1VisibleCardControllers.Remove(mainGameController.selectedCard);
                        Destroy(mainGameController.selectedCard.gameObject);
                        if (this.mainGameController.boardController.level1StackController.CheckCardsCount() == 0)
                        {
                            break;
                        }

                        GameObject gameObject1 = Instantiate(mainGameController.boardController.cardPrefab, vector, Quaternion.identity, mainGameController.boardController.level1VisibleCards.transform);
                        gameObject1.name = "Card_Level_" + cardLevel;
                        CardController cardController1 = gameObject1.GetComponent<CardController>();
                        cardController1.InitCardData(mainGameController.boardController.level1StackController.PopCardFromStack());
                        cardController1.gameController = this.mainGameController;
                        AddCardClickListener(gameObject1, cardController1);

                        this.mainGameController.boardController.level1CardGameObjects.Add(gameObject1);
                        this.mainGameController.boardController.level1VisibleCardControllers.Add(cardController1);

                        break;

                    case 2:
                        mainGameController.boardController.level2CardGameObjects.Remove(mainGameController.selectedCard.gameObject);
                        mainGameController.boardController.level2VisibleCardControllers.Remove(mainGameController.selectedCard);
                        Destroy(mainGameController.selectedCard.gameObject);
                        if (mainGameController.boardController.level2StackController.CheckCardsCount() == 0)
                        {
                            break;
                        }

                        GameObject gameObject2 = Instantiate(mainGameController.boardController.cardPrefab, vector, Quaternion.identity, mainGameController.boardController.level2VisibleCards.transform);
                        gameObject2.name = "Card_Level_" + cardLevel;
                        CardController cardController2 = gameObject2.GetComponent<CardController>();
                        cardController2.InitCardData(mainGameController.boardController.level2StackController.PopCardFromStack());
                        cardController2.gameController = this.mainGameController;
                        AddCardClickListener(gameObject2, cardController2);

                        this.mainGameController.boardController.level2CardGameObjects.Add(gameObject2);
                        this.mainGameController.boardController.level2VisibleCardControllers.Add(cardController2);

                        break;

                    case 3:
                        mainGameController.boardController.level3CardGameObjects.Remove(mainGameController.selectedCard.gameObject);
                        mainGameController.boardController.level3VisibleCardControllers.Remove(mainGameController.selectedCard);
                        Destroy(mainGameController.selectedCard.gameObject);
                        if (mainGameController.boardController.level3StackController.CheckCardsCount() == 0)
                        {
                            break;
                        }

                        GameObject gameObject3 = Instantiate(mainGameController.boardController.cardPrefab, vector, Quaternion.identity, mainGameController.boardController.level3VisibleCards.transform);
                        gameObject3.name = "Card_Level_" + cardLevel;
                        CardController cardController3 = gameObject3.GetComponent<CardController>();
                        cardController3.InitCardData(mainGameController.boardController.level3StackController.PopCardFromStack());
                        cardController3.gameController = this.mainGameController;
                        AddCardClickListener(gameObject3, cardController3);

                        this.mainGameController.boardController.level3CardGameObjects.Add(gameObject3);
                        this.mainGameController.boardController.level3VisibleCardControllers.Add(cardController3);
                        break;
                }
                if (goldenGemStashController.TakeGolden())
                {
                    Debug.Log("Pobrano złoty żeton");
                }
                else Debug.Log("Nie ma już złotych żetonów");
            }
            else if(mainGameController.selectedStack != null && mainGameController.selectedStack.CheckCardsCount() > 0)
            {

                PlayerController player = this;

                if(mainGameController.selectedStack.CheckCardsCount() > 0)
                {
                    CardController reservedCard = mainGameController.selectedStack.PopCardFromStack();
                    reservedCard.isReserved = true;
                    reservedCard.ownerId = mainGameController.currentPlayerId;

                    player.handReserved.Add(reservedCard);

                    if (goldenGemStashController.TakeGolden())
                    {
                        Debug.Log("Pobrano złoty żeton");
                    }
                    else Debug.Log("Nie ma już złotych żetonów");
                }
                else
                {
                    Debug.Log("Nie ma juz kart na stosie!");
                }
            }
            else
            {
                Debug.Log("Nie wybrano żadnej karty do zarezerwowania");
            }

            this.SetGemInfo(this.resources);
            this.PerformEndOfTurnDecision();
        }
        else
        {
            alertController.ShowTooManyReservedCards();
            if (mainGameController.selectedStack.CheckCardsCount() <= 0) Debug.Log("Nie ma juz kart na stosie!");
            else Debug.Log("Za dużo zarezerwowałeś kart");
        }
    }

    public void SetPlayerAvatar(string avatarPath)
    {
        Sprite playerAvatar = UnityEngine.Resources.Load<Sprite>(avatarPath);

        this.avatar.sprite = playerAvatar;
    }

    public void AddCardClickListener(GameObject cardGameObject, CardController cardController)
    {
        Button button = cardGameObject.GetComponent<Button>();
        if (button == null)
        {
            button = cardGameObject.AddComponent<Button>();
            button.targetGraphic = cardGameObject.GetComponent<Image>();
        }
        button.onClick.AddListener(() => mainGameController.SelectCard(cardController));
    }

    public void TakeGems(List<GemColor> colors)
    {
        for (int i = 0; i < colors.Count; i++)
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

        this.PerformEndOfTurnDecision();
    }

    private void PerformEndOfTurnDecision()
    {
        int amountOfGems = this.GetAmountOfGems();
        if (amountOfGems > 10)
        {
            this.SetGemInfo(this.resources);
            Debug.Log("Masz za dużo żetonów musisz oddać " + (amountOfGems - MAXIMUM_AMOUNT_OF_GEMS));
            TooManyGemsInformation(amountOfGems - MAXIMUM_AMOUNT_OF_GEMS);

            mainGameController.actionIsTaken = true;
            bankController.SetModeToGive(amountOfGems - MAXIMUM_AMOUNT_OF_GEMS);
        }
        else
        {
            this.ConfirmPlayerMove();
        }
    }

    public void RemoveResource(GemColor color)
    {
        this.resources.RemoveResource(color, 1);
        this.SetGemInfo(this.resources);
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

    public void UpdatePlayersResources()
    {
        this.mainGameController.UpdateTargetedPlayerResources(this.playerId, this.resources);
        this.mainGameController.UpdateTargetedPlayerBonusResources(this.playerId, this.bonusResources);
    }

    public void UpdatePlayersPoints()
    {
        this.mainGameController.UpdateTargetedPlayerPoints(this.playerId, this.points);
    }

    public void ConfirmPlayerMove()
    {
        this.UpdatePlayersResources();
        this.UpdatePlayersPoints();

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

    public void SetPlayerResources(ResourcesController resources, ResourcesController bonusResources)
    {
        this.resources = resources;
        this.bonusResources = bonusResources;

        this.resourcesInfo = this.resources.ToString();
        this.bonusResourcesInfo = this.bonusResources.ToString();

        this.SetGemInfo(this.resources);
    }

    public void SetPlayerPoints(int points)
    {
        this.points = points;
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
            this.bonusResources.AddResource(bonusColor);
            Debug.Log($"Dodano bonusowy zas�b: {bonusColor}");
        }
    }
    public bool CanAffordCard(CardController cardToBuy)
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
    public bool CanAffordCardWithGolden(CardController cardToBuy)
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

    public int GetAmountOfGems()
    {
        int result = 0;

        foreach(GemColor color in this.resources.gems.Keys)
        {
            if(color == GemColor.NONE)
            {
                continue;
            }

            result += this.resources.gems[color];
        }

        return result;
    }

    public List<GameObject> GetNoble(GameController game, PlayerController player)
    {
        List<GameObject> result = new List<GameObject>();
        foreach (var nobleGameObject in game.boardController.createdAvailableNoblesGameObjects)
        {
            NobleController noble = nobleGameObject.GetComponent<NobleController>();
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
                result.Add(nobleGameObject);
        }

        return result;
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
    public void TooManyGemsInformation(int amount)
    {
        TooManyGemsAlert.SetActive(true);
        TooManyGemsAlertText.text = $"{amount}";
    }
    public void HideTooManyGemsInformation()
    {
        TooManyGemsAlert.SetActive(false);
    }
}
