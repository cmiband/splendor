using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResponseValidatorController : MonoBehaviour
{
    public GameController gameController;
    public PlayerController currentPlayerController;
    public BoardController boardController;
    public BankController bankController;
    public GemStashController goldenGemStash;

    private List<List<int>> threeGemsConfigurations = new List<List<int>>
        {
            new List<int> { 0, 0, 1, 1, 1 },
            new List<int> { 0, 1, 0, 1, 1 },
            new List<int> { 0, 1, 1, 0, 1 },
            new List<int> { 0, 1, 1, 1, 0 },
            new List<int> { 1, 0, 0, 1, 1 },
            new List<int> { 1, 0, 1, 0, 1 },
            new List<int> { 1, 0, 1, 1, 0 },
            new List<int> { 1, 1, 0, 0, 1 },
            new List<int> { 1, 1, 0, 1, 0 },
            new List<int> { 1, 1, 1, 0, 0 }
        };

    public int PerformAgentMoveAndReturnAmountOfInvalidMoves(int[] arrayOfMoves)
    {
        if(arrayOfMoves == null)
        {
            throw new System.Exception("Array of moves was null");
        }
        int passPlace = 0;

        for (int i = 0; i < arrayOfMoves.Length; i++)
        {
            if (arrayOfMoves[i] == 0)
            {
                passPlace = i;
            }
            else if (this.IsValidMove(arrayOfMoves[i]))
            {
                this.currentPlayerController.UpdatePlayersResources();
                this.currentPlayerController.UpdatePlayersPoints();
                return i;
            }
        }
        return passPlace;
    }

    private bool IsValidMove(int move) 
    {
        try
        {
            switch(move)
            {
                case 0:
                    this.gameController.HandlePass();
                    return true;

                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                    CardController targetedCard = this.GetSelectedCard(move - 1);

                    bool buyResult = this.HandleCardBuy(targetedCard);
                    return buyResult;

                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    bool takeTwoResult = this.HandleTakeTwoGems(move);
                    return takeTwoResult;

                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                    bool takeThreeResult = this.HandleTakeThreeGems(move);
                    return takeThreeResult;

                case 29:
                case 30:
                case 31:
                case 32:
                case 33:
                case 34:
                case 35:
                case 36:
                case 37:
                case 38:
                case 39:
                case 40:
                    bool reserveResult = this.HandleReserveCard(move - 29);
                    return reserveResult;

                case 41:
                case 42:
                case 43:
                    CardController targetedReservedCard = this.currentPlayerController.handReserved[move - 41];

                    bool buyReservedResult = this.HandleBuyReservedCard(targetedReservedCard);
                    return buyReservedResult;

                default:
                    return false;
            }
        } catch(System.Exception e)
        {
            return false;
        }
    }

    private bool HandleBuyReservedCard(CardController selectedCard)
    {
        return this.HandleCardBuy(selectedCard);
    }

    private bool HandleReserveCard(int cardId)
    {
        if(this.currentPlayerController.handReserved.Count == 3)
        {
            return false;
        }

        this.goldenGemStash.TakeGolden();

        CardController selectedCard = this.GetSelectedCard(cardId);

        int cardLevel = selectedCard.level;
        Vector3 vector = selectedCard.transform.position;
        
        selectedCard.isReserved = true;


        var copiedCard = CloneCard(selectedCard);
        this.currentPlayerController.handReserved.Add(copiedCard);

        switch (cardLevel)
        {
            case 1:
                this.gameController.boardController.level1VisibleCardControllers.Remove(selectedCard);
                Debug.Log("Zarezerwowano kart� 1 poziomu");
                Destroy(selectedCard.gameObject);
                GameObject gameObject1 = Instantiate(this.gameController.boardController.cardPrefab, vector, Quaternion.identity, this.gameController.boardController.level1VisibleCards.transform);
                gameObject1.name = "Card_Level_" + cardLevel;
                CardController cardController1 = gameObject1.GetComponent<CardController>();
                cardController1.InitCardData(this.gameController.boardController.level1StackController.PopCardFromStack());
                cardController1.gameController = this.gameController;
                this.currentPlayerController.AddCardClickListener(gameObject1, cardController1);
                break;

            case 2:
                this.gameController.boardController.level2VisibleCardControllers.Remove(selectedCard);
                Debug.Log("Zarezerwowano kart� 2 poziomu");
                Destroy(selectedCard.gameObject);
                GameObject gameObject2 = Instantiate(this.gameController.boardController.cardPrefab, vector, Quaternion.identity, this.gameController.boardController.level2VisibleCards.transform);
                gameObject2.name = "Card_Level_" + cardLevel;
                CardController cardController2 = gameObject2.GetComponent<CardController>();
                cardController2.InitCardData(this.gameController.boardController.level2StackController.PopCardFromStack());
                cardController2.gameController = this.gameController;
                this.currentPlayerController.AddCardClickListener(gameObject2, cardController2);
                break;

            case 3:
                this.gameController.boardController.level3VisibleCardControllers.Remove(selectedCard);
                Debug.Log("Zarezerwowano kart� 3 poziomu");
                Destroy(selectedCard.gameObject);
                GameObject gameObject3 = Instantiate(this.gameController.boardController.cardPrefab, vector, Quaternion.identity, this.gameController.boardController.level3VisibleCards.transform);
                gameObject3.name = "Card_Level_" + cardLevel;
                CardController cardController3 = gameObject3.GetComponent<CardController>();
                cardController3.InitCardData(this.gameController.boardController.level3StackController.PopCardFromStack());
                cardController3.gameController = this.gameController;
                this.currentPlayerController.AddCardClickListener(gameObject3, cardController3);
                break;
        }

        return true;
    }

    private bool HandleTakeThreeGems(int moveId)
    {
        int targetedCombinationIndex = moveId - 19;

        List<int> targetedCombination = this.threeGemsConfigurations[targetedCombinationIndex];

        List<GemColor> colors = Enum.GetValues(typeof(GemColor)).Cast<GemColor>().ToList();

        int playerAmount = this.currentPlayerController.GetAmountOfGems();
        List<int> bankAmounts = this.GetBankAmounts(targetedCombination, colors);

        if(!this.CanTakeThreeGems(playerAmount, bankAmounts))
        {
            return false;
        }

        List<GemColor> gemsToTake = this.CreateListOfGemsToTake(targetedCombination, colors);
        this.bankController.gemsBeingChosen = gemsToTake;
        this.bankController.TakeGems();
        return true;
    }

    private List<GemColor> CreateListOfGemsToTake(List<int> combination, List<GemColor> colors)
    {
        List<GemColor> result = new List<GemColor>();
        for (int i = 0; i < combination.Count; i++)
        {
            if (combination[i] == 1)
            {
                GemColor targetedColor = colors[i];

                result.Add(targetedColor);
            }
        }
        return result;
    }

    private bool CanTakeThreeGems(int playerAmount, List<int> bankAmounts)
    {
        if(playerAmount > 7)
        {
            return false;
        }

        foreach(int amount in bankAmounts)
        {
            if(amount == 0)
            {
                return false;
            }
        }

        return true;
    }

    private List<int> GetBankAmounts(List<int> combination, List<GemColor> colors) {
        List<int> result = new List<int>();
        for(int i = 0; i<combination.Count; i++)
        {
            if (combination[i] == 1)
            {
                GemColor targetedColor = colors[i];

                int bankAmount = this.bankController.resourcesController.gems[targetedColor];
                result.Add(bankAmount);
            }
        }
        return result;
    }

    private bool HandleTakeTwoGems(int moveId)
    {
        int targetedGems = moveId - 14;

        List<GemColor> colors = Enum.GetValues(typeof(GemColor)).Cast<GemColor>().ToList();

        GemColor targetedColor = colors[targetedGems];

        int amountInBank = this.bankController.resourcesController.gems[targetedColor];
        int playerAmount = this.currentPlayerController.GetAmountOfGems();

        if(!this.CanTakeTwoGems(amountInBank, playerAmount))
        {
            return false;
        }

        List<GemColor> gemsToTake = new List<GemColor> { targetedColor, targetedColor};
        
        this.bankController.gemsBeingChosen = gemsToTake;
        this.bankController.TakeGems();
        return true;
    }

    private bool CanTakeTwoGems(int amountInBank, int playerAmount)
    {
        return amountInBank >= 4 && playerAmount <= 8;
    }
    
    private bool HandleCardBuy(CardController selectedCard)
    {
        bool canPlayerBuyCard = this.CheckIfPlayerCanBuyCard(selectedCard);
        if(!canPlayerBuyCard)
        {
            return false;
        }

        Dictionary<GemColor, int> price = selectedCard.detailedPrice.gems;
        Vector3 vector = selectedCard.transform.position;

        var resourcesUsed = new Dictionary<GemColor, int>();
        foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
        {
            resourcesUsed[color] = 0;
        }

        foreach (var gemCost in price)
        {
            int requiredAmount = gemCost.Value;

            int bonusAmount = this.currentPlayerController.BonusResources.gems.TryGetValue(gemCost.Key, out var bonus) ? bonus : 0;
            requiredAmount -= Math.Min(bonusAmount, requiredAmount);

            if (requiredAmount > 0)
            {
                int availableAmount = this.currentPlayerController.Resources.gems.TryGetValue(gemCost.Key, out var playerAmount) ? playerAmount : 0;

                if (availableAmount >= requiredAmount)
                {
                    resourcesUsed[gemCost.Key] = requiredAmount;
                }
                else
                {
                    int deficit = requiredAmount - availableAmount;

                    if (this.currentPlayerController.Resources.gems.TryGetValue(GemColor.GOLDEN, out var goldenAmount) && goldenAmount >= deficit)
                    {
                        resourcesUsed[gemCost.Key] = availableAmount;
                        resourcesUsed[GemColor.GOLDEN] += deficit;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        foreach (var resource in resourcesUsed)
        {
            if (resource.Value > 0)
            {
                this.currentPlayerController.Resources.RemoveResource(resource.Key, resource.Value);
            }
        }

        this.bankController.AddGems(resourcesUsed);

        this.currentPlayerController.hand.Add(this.CloneCard(selectedCard));
        this.currentPlayerController.AddBonusResource(selectedCard.bonusColor);
        this.currentPlayerController.Points += selectedCard.points;
        int cardLevel = selectedCard.Level;

        if (selectedCard.isReserved)
        {
            this.currentPlayerController.handReserved.Remove(selectedCard);
            this.gameController.reservedCardController.reservedCards.Remove(selectedCard.gameObject);
            Debug.Log("Kupiono zarezerwowaną kartę");
            Destroy(selectedCard.gameObject);

        }
        else
        {
            switch (cardLevel)
            {
                case 1:
                    this.gameController.boardController.level1VisibleCardControllers.Remove(selectedCard);
                    Debug.Log("Kupiono kart� 1 poziomu");
                    Destroy(selectedCard.gameObject);

                    if (this.gameController.boardController.level1StackController.CheckCardsCount() == 0)
                    {
                        break;
                    }

                    GameObject gameObject1 = Instantiate(this.gameController.boardController.cardPrefab, vector, Quaternion.identity, this.gameController.boardController.level1VisibleCards.transform);
                    gameObject1.name = "Card_Level_" + cardLevel;
                    CardController cardController1 = gameObject1.GetComponent<CardController>();
                    cardController1.InitCardData(this.gameController.boardController.level1StackController.PopCardFromStack());
                    cardController1.gameController = this.gameController;
                    currentPlayerController.AddCardClickListener(gameObject1, cardController1);
                    break;

                case 2:
                    this.gameController.boardController.level2VisibleCardControllers.Remove(selectedCard);
                    Debug.Log("Kupiono kart� 2 poziomu");
                    Destroy(selectedCard.gameObject);

                    if (this.gameController.boardController.level2StackController.CheckCardsCount() == 0)
                    {
                        break;
                    }

                    GameObject gameObject2 = Instantiate(this.gameController.boardController.cardPrefab, vector, Quaternion.identity, this.gameController.boardController.level2VisibleCards.transform);
                    gameObject2.name = "Card_Level_" + cardLevel;
                    CardController cardController2 = gameObject2.GetComponent<CardController>();
                    cardController2.InitCardData(this.gameController.boardController.level2StackController.PopCardFromStack());
                    cardController2.gameController = this.gameController;
                    this.currentPlayerController.AddCardClickListener(gameObject2, cardController2);
                    break;

                case 3:
                    this.gameController.boardController.level3VisibleCardControllers.Remove(selectedCard);
                    Debug.Log("Kupiono kart� 3 poziomu");
                    Destroy(selectedCard.gameObject);

                    if (this.gameController.boardController.level3StackController.CheckCardsCount() == 0)
                    {
                        break;
                    }

                    GameObject gameObject3 = Instantiate(this.gameController.boardController.cardPrefab, vector, Quaternion.identity, this.gameController.boardController.level3VisibleCards.transform);
                    gameObject3.name = "Card_Level_" + cardLevel;
                    CardController cardController3 = gameObject3.GetComponent<CardController>();
                    cardController3.InitCardData(this.gameController.boardController.level3StackController.PopCardFromStack());
                    cardController3.gameController = this.gameController;
                    this.currentPlayerController.AddCardClickListener(gameObject3, cardController3);
                    break;
            }
        }
        return true;
    }

    private bool CheckIfPlayerCanBuyCard(CardController selectedCard)
    {
        return !this.currentPlayerController.CanAffordCardWithGolden(selectedCard) && !this.currentPlayerController.CanAffordCard(selectedCard);
    }

    private CardController GetSelectedCard(int cardId)
    {
        int targetedStack = this.GetTargetedStack(cardId);
        int targetedCard = cardId - targetedStack * 4;

        if(targetedStack == 0)
        {
            return this.boardController.level1VisibleCardControllers[targetedCard];
        }
        if(targetedCard == 1)
        {
            return this.boardController.level2VisibleCardControllers[targetedCard];
        } 
        else
        {
            return this.boardController.level3VisibleCardControllers[targetedCard];
        }
    }

    private int GetTargetedStack(int cardId)
    {
        if (cardId >= 1 && cardId <= 4) return 0;
        if (cardId > 4 && cardId <= 8) return 1;
        else return 2;
    }

    private CardController CloneCard(CardController card)
    {
        GameObject cardObject = Instantiate(card.gameObject);

        CardController clonedCard = cardObject.GetComponent<CardController>();

        clonedCard.InitCardData(card);

        return clonedCard;
    }
} 
