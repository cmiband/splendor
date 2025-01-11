using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelConnectionController : MonoBehaviour
{
    public GameController gameController;
    public BoardController boardController;
    public BankController bankController;

    public float[] Standartize(int[] array)
    {
        int n = array.Length;
        float sum = 0;

        for (int i = 0; i < n; i++)
        {
            sum += array[i];
        }

        float mean = sum / n;

        float q = 0;
        for (int i = 0; i < n; i++)
        {
            float xiu = (float)Math.Pow((array[i] - mean), 2);
            q += xiu;
        }

        float standardDeviation = (float)Math.Sqrt(q / n);

        float[] finalZScore = new float[348];

        for (int i = 0; i < n; i++)
        {
            float zScore = (array[i] - mean) / standardDeviation;
            finalZScore[i] = zScore;
        }

        return finalZScore;
    }

    public int[] GameToArray()
    {
        int[] output = new int[348];
        int pointer = 0;

        foreach (var item in this.BoardToArray())
        {
            output[pointer++] = item;
        }

        foreach (var item in this.BankToArray())
        {
            output[pointer++] = item;
        }

        foreach (var noble in this.boardController.createdAvailableNoblesGameObjects)
        {
            if(noble == null)
            {
                throw new Exception("Available noble gameobject is null");
            }
            NobleController availableNobleController = noble.GetComponent<NobleController>();   

            foreach (var parameter in this.NobleToArray(availableNobleController))
            {
                output[pointer++] = parameter;
            }
        }

        while (pointer < 167)
        {
            output[pointer++] = 0;
            output[pointer++] = 16;
            output[pointer++] = 16;
            output[pointer++] = 16;
            output[pointer++] = 16;
            output[pointer++] = 16;
        }

        for (int i = 0; i < 4; i++)
        {
            foreach (var item in this.PlayerToArray(gameController.currentPlayerId+i))
            {
                output[pointer++] = item;
            }
        }
        return output;
    }

    public int[] PlayerToArray(int playerIndex)
    {
        int playerPoints = this.gameController.playerIdToPoints[playerIndex];
        ResourcesController playerBonusResources = this.gameController.playerIdToBonusResources[playerIndex];
        ResourcesController playerResources = this.gameController.playerIdToResources[playerIndex];
        List<CardController> playerReservedCards = this.gameController.playerIdToReserveHand[playerIndex]; 

        int[] output = new int[45];
        output[0] = playerPoints;
        output[1] = playerBonusResources.gems.ContainsKey(GemColor.WHITE) ? playerBonusResources.gems[GemColor.WHITE] : 0;
        output[2] = playerBonusResources.gems.ContainsKey(GemColor.BLUE) ? playerBonusResources.gems[GemColor.BLUE] : 0;
        output[3] = playerBonusResources.gems.ContainsKey(GemColor.GREEN) ? playerBonusResources.gems[GemColor.GREEN] : 0;
        output[4] = playerBonusResources.gems.ContainsKey(GemColor.RED) ? playerBonusResources.gems[GemColor.RED] : 0;
        output[5] = playerBonusResources.gems.ContainsKey(GemColor.BLACK) ? playerBonusResources.gems[GemColor.BLACK] : 0;
        output[6] = playerResources.gems.ContainsKey(GemColor.WHITE) ? playerResources.gems[GemColor.WHITE] : 0;
        output[7] = playerResources.gems.ContainsKey(GemColor.BLUE) ? playerResources.gems[GemColor.BLUE] : 0;
        output[8] = playerResources.gems.ContainsKey(GemColor.GREEN) ? playerResources.gems[GemColor.GREEN] : 0;
        output[9] = playerResources.gems.ContainsKey(GemColor.RED) ? playerResources.gems[GemColor.RED] : 0;
        output[10] = playerResources.gems.ContainsKey(GemColor.BLACK) ? playerResources.gems[GemColor.BLACK] : 0;
        int pointer = 11;

        foreach (var item in playerReservedCards)
        {
            if (item == null)
            {
                throw new Exception("Reserved card by player is null");
            }
            foreach (var parameter in CardToArray(item))
            {
                output[pointer++] = parameter;
            }
        }
        for (int i = 0; i < 3 - playerReservedCards.Count; i++)
        {
            output[pointer++] = 0;
            output[pointer++] = 0;
            output[pointer++] = 0;
            output[pointer++] = 0;
            output[pointer++] = 0;
            output[pointer++] = 0;
            output[pointer++] = 11;
            output[pointer++] = 11;
            output[pointer++] = 11;
            output[pointer++] = 11;
            output[pointer++] = 11;
        }
        return output;
    }

    private int[] NobleToArray(NobleController noble)
    {
        ResourcesController nobleRequiredResources = noble.detailedPrice;
        return new int[]   {noble.points,
                                nobleRequiredResources.gems.ContainsKey(GemColor.WHITE) ? nobleRequiredResources.gems[GemColor.WHITE] : 0,
                                nobleRequiredResources.gems.ContainsKey(GemColor.BLUE) ? nobleRequiredResources.gems[GemColor.BLUE] : 0,
                                nobleRequiredResources.gems.ContainsKey(GemColor.GREEN) ? nobleRequiredResources.gems[GemColor.GREEN] : 0,
                                nobleRequiredResources.gems.ContainsKey(GemColor.RED) ? nobleRequiredResources.gems[GemColor.RED] : 0,
                                nobleRequiredResources.gems.ContainsKey(GemColor.BLACK) ? nobleRequiredResources.gems[GemColor.BLACK] : 0
                                };
    }

    private int[] BankToArray()
    {
        ResourcesController bankResources = this.bankController.resourcesController;
        return new int[] { bankResources.gems.ContainsKey(GemColor.WHITE) ? bankResources.gems[GemColor.WHITE] : 0,
                               bankResources.gems.ContainsKey(GemColor.BLUE) ? bankResources.gems[GemColor.BLUE] : 0,
                               bankResources.gems.ContainsKey(GemColor.GREEN) ? bankResources.gems[GemColor.GREEN] : 0,
                               bankResources.gems.ContainsKey(GemColor.RED) ? bankResources.gems[GemColor.RED] : 0,
                               bankResources.gems.ContainsKey(GemColor.BLACK) ? bankResources.gems[GemColor.BLACK] : 0,
                               bankResources.gems.ContainsKey(GemColor.GOLDEN) ? bankResources.gems[GemColor.GOLDEN] : 0
                             };
    }

    private int[] BoardToArray()
    {
        int[] output = new int[132];
        int pointer = 0;

        foreach(var card in this.boardController.level1VisibleCardControllers)
        {
            if(card == null)
            {
                throw new System.Exception("Null card controller from level1");
            }

            foreach (var parameter in this.CardToArray(card))
            {
                output[pointer++] = parameter;
            }
        }

        while (pointer <= 43)
        {
            output[pointer++] = 0;
            output[pointer++] = 0;
            output[pointer++] = 0;
            output[pointer++] = 0;
            output[pointer++] = 0;
            output[pointer++] = 0;
            output[pointer++] = 11;
            output[pointer++] = 11;
            output[pointer++] = 11;
            output[pointer++] = 11;
            output[pointer++] = 11;
        }

        foreach (var card in this.boardController.level2VisibleCardControllers)
        {
            if (card == null)
            {
                throw new Exception("Null card controller from level2");
            }
            foreach (var parameter in this.CardToArray(card))
            {
                output[pointer++] = parameter;
            }
        }

        while (pointer <= 87)
        {
            output[pointer++] = 0;
            output[pointer++] = 0;
            output[pointer++] = 0;
            output[pointer++] = 0;
            output[pointer++] = 0;
            output[pointer++] = 0;
            output[pointer++] = 11;
            output[pointer++] = 11;
            output[pointer++] = 11;
            output[pointer++] = 11;
            output[pointer++] = 11;
        }

        foreach (var card in this.boardController.level3VisibleCardControllers)
        {
            if (card == null)
            {
                throw new Exception("Null card controller from level3");
            }
            foreach (var parameter in this.CardToArray(card))
            {
                output[pointer++] = parameter;
            }
        }

        while (pointer < 131)
        {
            output[pointer++] = 0;
            output[pointer++] = 0;
            output[pointer++] = 0;
            output[pointer++] = 0;
            output[pointer++] = 0;
            output[pointer++] = 0;
            output[pointer++] = 11;
            output[pointer++] = 11;
            output[pointer++] = 11;
            output[pointer++] = 11;
            output[pointer++] = 11;
        }
        return output;
    }

    private int[] CardToArray(CardController card)
    {
        return new int[] { card.points,
                               card.bonusColor == GemColor.WHITE  ?  1 : 0,
                               card.bonusColor == GemColor.BLUE   ?  1 : 0,
                               card.bonusColor == GemColor.GREEN  ?  1 : 0,
                               card.bonusColor == GemColor.RED    ?  1 : 0,
                               card.bonusColor == GemColor.BLACK  ?  1 : 0,
                               card.detailedPrice.gems.ContainsKey(GemColor.WHITE) ? card.detailedPrice.gems[GemColor.WHITE] : 0,
                               card.detailedPrice.gems.ContainsKey(GemColor.BLUE) ? card.detailedPrice.gems[GemColor.BLUE] : 0,
                               card.detailedPrice.gems.ContainsKey(GemColor.GREEN) ? card.detailedPrice.gems[GemColor.GREEN] : 0,
                               card.detailedPrice.gems.ContainsKey(GemColor.RED) ? card.detailedPrice.gems[GemColor.RED] : 0,
                               card.detailedPrice.gems.ContainsKey(GemColor.BLACK) ? card.detailedPrice.gems[GemColor.BLACK] : 0
                             };
    }
}
