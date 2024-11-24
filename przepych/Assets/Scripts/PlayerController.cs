using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
        switch (cardLevel)
        {
            case 1:
                {
                    player.hand.Add(mainGameController.selectedToBuyCard);
                    mainGameController.boardController.level1VisibleCardControllers.Remove(mainGameController.selectedToBuyCard);
                    Debug.Log("Kupiono karte 1 poziomu");
                    Destroy(mainGameController.selectedToBuyCard.gameObject);
                    GameObject gameObject = Instantiate(mainGameController.boardController.cardPrefab, vector, Quaternion.identity, mainGameController.boardController.level1VisibleCards.transform);
                    CardController cardController = gameObject.GetComponent<CardController>();
                    cardController.InitCardData(mainGameController.boardController.level1StackController.PopCardFromStack());
                    break;
                }
            case 2:
                {
                    player.hand.Add(mainGameController.selectedToBuyCard);
                    mainGameController.boardController.level2VisibleCardControllers.Remove(mainGameController.selectedToBuyCard);
                    Debug.Log("Kupiono karte 2 poziomu");
                    Destroy(mainGameController.selectedToBuyCard.gameObject);
                    GameObject gameObject = Instantiate(mainGameController.boardController.cardPrefab, vector, Quaternion.identity, mainGameController.boardController.level2VisibleCards.transform);
                    CardController cardController = gameObject.GetComponent<CardController>();
                    cardController.InitCardData(mainGameController.boardController.level2StackController.PopCardFromStack());
                    break;
                }
            case 3:
                {
                    player.hand.Add(mainGameController.selectedToBuyCard);
                    mainGameController.boardController.level3VisibleCardControllers.Remove(mainGameController.selectedToBuyCard);
                    Debug.Log("Kupiono karte 3 poziomu");
                    Destroy(mainGameController.selectedToBuyCard.gameObject);
                    GameObject gameObject = Instantiate(mainGameController.boardController.cardPrefab, vector, Quaternion.identity, mainGameController.boardController.level3VisibleCards.transform);
                    CardController cardController = gameObject.GetComponent<CardController>();
                    cardController.InitCardData(mainGameController.boardController.level3StackController.PopCardFromStack());
                    break;
                }

        }
        mainGameController.ChangeTurn();
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
}
