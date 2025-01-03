using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplendorConsole
{
    public class ResponseValidator
    {
        private int passPlace = 0;
        public int CheckMoves(int[] arrayOfMoves, Player currentPlayer, Game game, Bank bank, Board board)
        {
            if (arrayOfMoves == null)
            {
                throw new Exception("arrayOfMoves was null");
            }
            for(int i = 0; i < arrayOfMoves.Length; i++)
            {
                if (arrayOfMoves[i]==0)
                {
                    passPlace = i;
                }
                else if (IsValid(arrayOfMoves[i], currentPlayer, game, bank, board))
                {
                    return i;
                }
            }
            return passPlace;
        }
        public bool IsValid(int move,  Player currentPlayer, Game game, Bank bank, Board board) 
        {
            try
            {
                switch (move)
                {
                    case 0:
                        game.Pass();
                        return true;
                    case 1:
                        if (currentPlayer.CanAffordCard(board.level1VisibleCards[0]) || currentPlayer.CanAffordCardWithGolden(board.level1VisibleCards[0]))
                        {
                            var simulatedResourcesUsed = new Dictionary<GemColor, int>();
                            foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
                            {
                                simulatedResourcesUsed[color] = 0;
                            }

                            if (board.level1VisibleCards[0] == null)
                            {
                                throw new Exception("Nie działa try, catch :/");
                            }

                            var cardPrice = board.level1VisibleCards[0].DetailedPrice.gems;

                            foreach (var colorOnCard in cardPrice)
                            {
                                GemColor color = colorOnCard.Key;
                                int requiredAmount = colorOnCard.Value;

                                int bonusAmount = currentPlayer.BonusResources.gems.TryGetValue(color, out var bonus) ? bonus : 0;
                                requiredAmount -= Math.Min(bonusAmount, requiredAmount);

                                if (requiredAmount > 0)
                                {
                                    int playerAmount = currentPlayer.Resources.gems.TryGetValue(color, out var playerR) ? playerR : 0;

                                    if (playerAmount >= requiredAmount)
                                    {
                                        simulatedResourcesUsed[color] += requiredAmount;
                                    }
                                    else
                                    {
                                        int deficit = requiredAmount - playerAmount;
                                        simulatedResourcesUsed[color] += playerAmount;
                                        simulatedResourcesUsed[GemColor.GOLDEN] += deficit;
                                    }
                                }
                            }
                            foreach (var resource in simulatedResourcesUsed)
                            {
                                if (resource.Value > 0 && resource.Key != GemColor.GOLDEN)
                                {
                                    currentPlayer.Resources.gems[resource.Key] -= resource.Value;
                                    if (currentPlayer.Resources.gems[resource.Key] == 0)
                                        currentPlayer.Resources.gems.Remove(resource.Key);
                                }
                            }

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                currentPlayer.Resources.gems[GemColor.GOLDEN] -= simulatedResourcesUsed[GemColor.GOLDEN];
                                if (currentPlayer.Resources.gems[GemColor.GOLDEN] == 0)
                                {
                                    currentPlayer.Resources.gems.Remove(GemColor.GOLDEN);
                                }
                            }

                            game.RefillBankResources(bank, board.level1VisibleCards[0], simulatedResourcesUsed);

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                bank.AddGoldenGem(simulatedResourcesUsed[GemColor.GOLDEN]);
                            }

                            currentPlayer.AddCardToPlayer(board.level1VisibleCards[0]);
                            currentPlayer.BonusResources.AddResource(board.level1VisibleCards[0].BonusColor);
                            currentPlayer.Points += board.level1VisibleCards[0].Points;

                            board.ReplaceMissingCard(1, board.level1VisibleCards[0]);
                            return true;
                        }
                        return false;
                    case 2:
                        if (currentPlayer.CanAffordCard(board.level1VisibleCards[1]) || currentPlayer.CanAffordCardWithGolden(board.level1VisibleCards[1]))
                        {
                            var simulatedResourcesUsed = new Dictionary<GemColor, int>();
                            foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
                            {
                                simulatedResourcesUsed[color] = 0;
                            }
                            if (board.level1VisibleCards[1] == null)
                            {
                                throw new Exception("Nie działa try, catch :/");
                            }
                            var cardPrice = board.level1VisibleCards[1].DetailedPrice.gems;

                            foreach (var colorOnCard in cardPrice)
                            {
                                GemColor color = colorOnCard.Key;
                                int requiredAmount = colorOnCard.Value;

                                int bonusAmount = currentPlayer.BonusResources.gems.TryGetValue(color, out var bonus) ? bonus : 0;
                                requiredAmount -= Math.Min(bonusAmount, requiredAmount);

                                if (requiredAmount > 0)
                                {
                                    int playerAmount = currentPlayer.Resources.gems.TryGetValue(color, out var playerR) ? playerR : 0;

                                    if (playerAmount >= requiredAmount)
                                    {
                                        simulatedResourcesUsed[color] += requiredAmount;
                                    }
                                    else
                                    {
                                        int deficit = requiredAmount - playerAmount;
                                        simulatedResourcesUsed[color] += playerAmount;
                                        simulatedResourcesUsed[GemColor.GOLDEN] += deficit;
                                    }
                                }
                            }
                            foreach (var resource in simulatedResourcesUsed)
                            {
                                if (resource.Value > 0 && resource.Key != GemColor.GOLDEN)
                                {
                                    currentPlayer.Resources.gems[resource.Key] -= resource.Value;
                                    if (currentPlayer.Resources.gems[resource.Key] == 0)
                                        currentPlayer.Resources.gems.Remove(resource.Key);
                                }
                            }

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                currentPlayer.Resources.gems[GemColor.GOLDEN] -= simulatedResourcesUsed[GemColor.GOLDEN];
                                if (currentPlayer.Resources.gems[GemColor.GOLDEN] == 0)
                                {
                                    currentPlayer.Resources.gems.Remove(GemColor.GOLDEN);
                                }
                            }

                            game.RefillBankResources(bank, board.level1VisibleCards[1], simulatedResourcesUsed);

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                bank.AddGoldenGem(simulatedResourcesUsed[GemColor.GOLDEN]);
                            }

                            currentPlayer.AddCardToPlayer(board.level1VisibleCards[1]);
                            currentPlayer.BonusResources.AddResource(board.level1VisibleCards[1].BonusColor);
                            currentPlayer.Points += board.level1VisibleCards[1].Points;

                            board.ReplaceMissingCard(1, board.level1VisibleCards[1]);
                            return true;
                        }
                        return false;
                    case 3:
                        if (currentPlayer.CanAffordCard(board.level1VisibleCards[2]) || currentPlayer.CanAffordCardWithGolden(board.level1VisibleCards[2]))
                        {
                            var simulatedResourcesUsed = new Dictionary<GemColor, int>();
                            foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
                            {
                                simulatedResourcesUsed[color] = 0;
                            }
                            if (board.level1VisibleCards[2] == null)
                            {
                                throw new Exception("Nie działa try, catch :/");
                            }
                            var cardPrice = board.level1VisibleCards[2].DetailedPrice.gems;

                            foreach (var colorOnCard in cardPrice)
                            {
                                GemColor color = colorOnCard.Key;
                                int requiredAmount = colorOnCard.Value;

                                int bonusAmount = currentPlayer.BonusResources.gems.TryGetValue(color, out var bonus) ? bonus : 0;
                                requiredAmount -= Math.Min(bonusAmount, requiredAmount);

                                if (requiredAmount > 0)
                                {
                                    int playerAmount = currentPlayer.Resources.gems.TryGetValue(color, out var playerR) ? playerR : 0;

                                    if (playerAmount >= requiredAmount)
                                    {
                                        simulatedResourcesUsed[color] += requiredAmount;
                                    }
                                    else
                                    {
                                        int deficit = requiredAmount - playerAmount;
                                        simulatedResourcesUsed[color] += playerAmount;
                                        simulatedResourcesUsed[GemColor.GOLDEN] += deficit;
                                    }
                                }
                            }
                            foreach (var resource in simulatedResourcesUsed)
                            {
                                if (resource.Value > 0 && resource.Key != GemColor.GOLDEN)
                                {
                                    currentPlayer.Resources.gems[resource.Key] -= resource.Value;
                                    if (currentPlayer.Resources.gems[resource.Key] == 0)
                                        currentPlayer.Resources.gems.Remove(resource.Key);
                                }
                            }

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                currentPlayer.Resources.gems[GemColor.GOLDEN] -= simulatedResourcesUsed[GemColor.GOLDEN];
                                if (currentPlayer.Resources.gems[GemColor.GOLDEN] == 0)
                                {
                                    currentPlayer.Resources.gems.Remove(GemColor.GOLDEN);
                                }
                            }

                            game.RefillBankResources(bank, board.level1VisibleCards[2], simulatedResourcesUsed);

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                bank.AddGoldenGem(simulatedResourcesUsed[GemColor.GOLDEN]);
                            }

                            currentPlayer.AddCardToPlayer(board.level1VisibleCards[2]);
                            currentPlayer.BonusResources.AddResource(board.level1VisibleCards[2].BonusColor);
                            currentPlayer.Points += board.level1VisibleCards[2].Points;

                            board.ReplaceMissingCard(1, board.level1VisibleCards[2]);
                            return true;
                        }
                        return false;
                    case 4:
                        if (currentPlayer.CanAffordCard(board.level1VisibleCards[3]) || currentPlayer.CanAffordCardWithGolden(board.level1VisibleCards[3]))
                        {
                            var simulatedResourcesUsed = new Dictionary<GemColor, int>();
                            foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
                            {
                                simulatedResourcesUsed[color] = 0;
                            }
                            if (board.level1VisibleCards[3] == null)
                            {
                                throw new Exception("Nie działa try, catch :/");
                            }
                            var cardPrice = board.level1VisibleCards[3].DetailedPrice.gems;

                            foreach (var colorOnCard in cardPrice)
                            {
                                GemColor color = colorOnCard.Key;
                                int requiredAmount = colorOnCard.Value;

                                int bonusAmount = currentPlayer.BonusResources.gems.TryGetValue(color, out var bonus) ? bonus : 0;
                                requiredAmount -= Math.Min(bonusAmount, requiredAmount);

                                if (requiredAmount > 0)
                                {
                                    int playerAmount = currentPlayer.Resources.gems.TryGetValue(color, out var playerR) ? playerR : 0;

                                    if (playerAmount >= requiredAmount)
                                    {
                                        simulatedResourcesUsed[color] += requiredAmount;
                                    }
                                    else
                                    {
                                        int deficit = requiredAmount - playerAmount;
                                        simulatedResourcesUsed[color] += playerAmount;
                                        simulatedResourcesUsed[GemColor.GOLDEN] += deficit;
                                    }
                                }
                            }
                            foreach (var resource in simulatedResourcesUsed)
                            {
                                if (resource.Value > 0 && resource.Key != GemColor.GOLDEN)
                                {
                                    currentPlayer.Resources.gems[resource.Key] -= resource.Value;
                                    if (currentPlayer.Resources.gems[resource.Key] == 0)
                                        currentPlayer.Resources.gems.Remove(resource.Key);
                                }
                            }

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                currentPlayer.Resources.gems[GemColor.GOLDEN] -= simulatedResourcesUsed[GemColor.GOLDEN];
                                if (currentPlayer.Resources.gems[GemColor.GOLDEN] == 0)
                                {
                                    currentPlayer.Resources.gems.Remove(GemColor.GOLDEN);
                                }
                            }

                            game.RefillBankResources(bank, board.level1VisibleCards[3], simulatedResourcesUsed);

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                bank.AddGoldenGem(simulatedResourcesUsed[GemColor.GOLDEN]);
                            }

                            currentPlayer.AddCardToPlayer(board.level1VisibleCards[3]);
                            if (board.level1VisibleCards[3] == null || board.level1VisibleCards[3].BonusColor == null)
                            {
                                throw new Exception("Nie działa try, catch :/");
                            }
                            currentPlayer.BonusResources.AddResource(board.level1VisibleCards[3].BonusColor);
                            currentPlayer.Points += board.level1VisibleCards[3].Points;

                            board.ReplaceMissingCard(1, board.level1VisibleCards[3]);
                            return true;
                        }
                        return false;
                    case 5:
                        if (currentPlayer.CanAffordCard(board.level2VisibleCards[0]) || currentPlayer.CanAffordCardWithGolden(board.level2VisibleCards[0]))
                        {
                            var simulatedResourcesUsed = new Dictionary<GemColor, int>();
                            foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
                            {
                                simulatedResourcesUsed[color] = 0;
                            }
                            if (board.level2VisibleCards[0] == null)
                            {
                                throw new Exception("Nie działa try, catch :/");
                            }
                            var cardPrice = board.level2VisibleCards[0].DetailedPrice.gems;

                            foreach (var colorOnCard in cardPrice)
                            {
                                GemColor color = colorOnCard.Key;
                                int requiredAmount = colorOnCard.Value;

                                int bonusAmount = currentPlayer.BonusResources.gems.TryGetValue(color, out var bonus) ? bonus : 0;
                                requiredAmount -= Math.Min(bonusAmount, requiredAmount);

                                if (requiredAmount > 0)
                                {
                                    int playerAmount = currentPlayer.Resources.gems.TryGetValue(color, out var playerR) ? playerR : 0;

                                    if (playerAmount >= requiredAmount)
                                    {
                                        simulatedResourcesUsed[color] += requiredAmount;
                                    }
                                    else
                                    {
                                        int deficit = requiredAmount - playerAmount;
                                        simulatedResourcesUsed[color] += playerAmount;
                                        simulatedResourcesUsed[GemColor.GOLDEN] += deficit;
                                    }
                                }
                            }
                            foreach (var resource in simulatedResourcesUsed)
                            {
                                if (resource.Value > 0 && resource.Key != GemColor.GOLDEN)
                                {
                                    currentPlayer.Resources.gems[resource.Key] -= resource.Value;
                                    if (currentPlayer.Resources.gems[resource.Key] == 0)
                                        currentPlayer.Resources.gems.Remove(resource.Key);
                                }
                            }

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                currentPlayer.Resources.gems[GemColor.GOLDEN] -= simulatedResourcesUsed[GemColor.GOLDEN];
                                if (currentPlayer.Resources.gems[GemColor.GOLDEN] == 0)
                                {
                                    currentPlayer.Resources.gems.Remove(GemColor.GOLDEN);
                                }
                            }

                            game.RefillBankResources(bank, board.level2VisibleCards[0], simulatedResourcesUsed);

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                bank.AddGoldenGem(simulatedResourcesUsed[GemColor.GOLDEN]);
                            }

                            currentPlayer.AddCardToPlayer(board.level2VisibleCards[0]);
                            currentPlayer.BonusResources.AddResource(board.level2VisibleCards[0].BonusColor);
                            currentPlayer.Points += board.level2VisibleCards[0].Points;

                            board.ReplaceMissingCard(2, board.level2VisibleCards[0]);
                            return true;
                        }
                        return false;
                    case 6:
                        if (currentPlayer.CanAffordCard(board.level2VisibleCards[1]) || currentPlayer.CanAffordCardWithGolden(board.level2VisibleCards[1]))
                        {
                            var simulatedResourcesUsed = new Dictionary<GemColor, int>();
                            foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
                            {
                                simulatedResourcesUsed[color] = 0;
                            }
                            if (board.level2VisibleCards[1] == null)
                            {
                                throw new Exception("Nie działa try, catch :/");
                            }
                            var cardPrice = board.level2VisibleCards[1].DetailedPrice.gems;

                            foreach (var colorOnCard in cardPrice)
                            {
                                GemColor color = colorOnCard.Key;
                                int requiredAmount = colorOnCard.Value;

                                int bonusAmount = currentPlayer.BonusResources.gems.TryGetValue(color, out var bonus) ? bonus : 0;
                                requiredAmount -= Math.Min(bonusAmount, requiredAmount);

                                if (requiredAmount > 0)
                                {
                                    int playerAmount = currentPlayer.Resources.gems.TryGetValue(color, out var playerR) ? playerR : 0;

                                    if (playerAmount >= requiredAmount)
                                    {
                                        simulatedResourcesUsed[color] += requiredAmount;
                                    }
                                    else
                                    {
                                        int deficit = requiredAmount - playerAmount;
                                        simulatedResourcesUsed[color] += playerAmount;
                                        simulatedResourcesUsed[GemColor.GOLDEN] += deficit;
                                    }
                                }
                            }
                            foreach (var resource in simulatedResourcesUsed)
                            {
                                if (resource.Value > 0 && resource.Key != GemColor.GOLDEN)
                                {
                                    currentPlayer.Resources.gems[resource.Key] -= resource.Value;
                                    if (currentPlayer.Resources.gems[resource.Key] == 0)
                                        currentPlayer.Resources.gems.Remove(resource.Key);
                                }
                            }

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                currentPlayer.Resources.gems[GemColor.GOLDEN] -= simulatedResourcesUsed[GemColor.GOLDEN];
                                if (currentPlayer.Resources.gems[GemColor.GOLDEN] == 0)
                                {
                                    currentPlayer.Resources.gems.Remove(GemColor.GOLDEN);
                                }
                            }

                            game.RefillBankResources(bank, board.level2VisibleCards[1], simulatedResourcesUsed);

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                bank.AddGoldenGem(simulatedResourcesUsed[GemColor.GOLDEN]);
                            }

                            currentPlayer.AddCardToPlayer(board.level2VisibleCards[1]);
                            currentPlayer.BonusResources.AddResource(board.level2VisibleCards[1].BonusColor);
                            currentPlayer.Points += board.level2VisibleCards[1].Points;

                            board.ReplaceMissingCard(2, board.level2VisibleCards[1]);
                            return true;
                        }
                        return false;
                    case 7:
                        if (currentPlayer.CanAffordCard(board.level2VisibleCards[2]) || currentPlayer.CanAffordCardWithGolden(board.level2VisibleCards[2]))
                        {
                            var simulatedResourcesUsed = new Dictionary<GemColor, int>();
                            foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
                            {
                                simulatedResourcesUsed[color] = 0;
                            }
                            if (board.level2VisibleCards[2] == null)
                            {
                                throw new Exception("Nie działa try, catch :/");
                            }
                            var cardPrice = board.level2VisibleCards[2].DetailedPrice.gems;

                            foreach (var colorOnCard in cardPrice)
                            {
                                GemColor color = colorOnCard.Key;
                                int requiredAmount = colorOnCard.Value;

                                int bonusAmount = currentPlayer.BonusResources.gems.TryGetValue(color, out var bonus) ? bonus : 0;
                                requiredAmount -= Math.Min(bonusAmount, requiredAmount);

                                if (requiredAmount > 0)
                                {
                                    int playerAmount = currentPlayer.Resources.gems.TryGetValue(color, out var playerR) ? playerR : 0;

                                    if (playerAmount >= requiredAmount)
                                    {
                                        simulatedResourcesUsed[color] += requiredAmount;
                                    }
                                    else
                                    {
                                        int deficit = requiredAmount - playerAmount;
                                        simulatedResourcesUsed[color] += playerAmount;
                                        simulatedResourcesUsed[GemColor.GOLDEN] += deficit;
                                    }
                                }
                            }
                            foreach (var resource in simulatedResourcesUsed)
                            {
                                if (resource.Value > 0 && resource.Key != GemColor.GOLDEN)
                                {
                                    currentPlayer.Resources.gems[resource.Key] -= resource.Value;
                                    if (currentPlayer.Resources.gems[resource.Key] == 0)
                                        currentPlayer.Resources.gems.Remove(resource.Key);
                                }
                            }

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                currentPlayer.Resources.gems[GemColor.GOLDEN] -= simulatedResourcesUsed[GemColor.GOLDEN];
                                if (currentPlayer.Resources.gems[GemColor.GOLDEN] == 0)
                                {
                                    currentPlayer.Resources.gems.Remove(GemColor.GOLDEN);
                                }
                            }

                            game.RefillBankResources(bank, board.level2VisibleCards[2], simulatedResourcesUsed);

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                bank.AddGoldenGem(simulatedResourcesUsed[GemColor.GOLDEN]);
                            }

                            currentPlayer.AddCardToPlayer(board.level2VisibleCards[2]);
                            currentPlayer.BonusResources.AddResource(board.level2VisibleCards[2].BonusColor);
                            currentPlayer.Points += board.level2VisibleCards[2].Points;

                            board.ReplaceMissingCard(2, board.level2VisibleCards[2]);
                            return true;
                        }
                        return false;
                    case 8:
                        if (currentPlayer.CanAffordCard(board.level2VisibleCards[3]) || currentPlayer.CanAffordCardWithGolden(board.level2VisibleCards[3]))
                        {
                            var simulatedResourcesUsed = new Dictionary<GemColor, int>();
                            foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
                            {
                                simulatedResourcesUsed[color] = 0;
                            }
                            if (board.level2VisibleCards[3] == null)
                            {
                                throw new Exception("Nie działa try, catch :/");
                            }
                            var cardPrice = board.level2VisibleCards[3].DetailedPrice.gems;

                            foreach (var colorOnCard in cardPrice)
                            {
                                GemColor color = colorOnCard.Key;
                                int requiredAmount = colorOnCard.Value;

                                int bonusAmount = currentPlayer.BonusResources.gems.TryGetValue(color, out var bonus) ? bonus : 0;
                                requiredAmount -= Math.Min(bonusAmount, requiredAmount);

                                if (requiredAmount > 0)
                                {
                                    int playerAmount = currentPlayer.Resources.gems.TryGetValue(color, out var playerR) ? playerR : 0;

                                    if (playerAmount >= requiredAmount)
                                    {
                                        simulatedResourcesUsed[color] += requiredAmount;
                                    }
                                    else
                                    {
                                        int deficit = requiredAmount - playerAmount;
                                        simulatedResourcesUsed[color] += playerAmount;
                                        simulatedResourcesUsed[GemColor.GOLDEN] += deficit;
                                    }
                                }
                            }
                            foreach (var resource in simulatedResourcesUsed)
                            {
                                if (resource.Value > 0 && resource.Key != GemColor.GOLDEN)
                                {
                                    currentPlayer.Resources.gems[resource.Key] -= resource.Value;
                                    if (currentPlayer.Resources.gems[resource.Key] == 0)
                                        currentPlayer.Resources.gems.Remove(resource.Key);
                                }
                            }

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                currentPlayer.Resources.gems[GemColor.GOLDEN] -= simulatedResourcesUsed[GemColor.GOLDEN];
                                if (currentPlayer.Resources.gems[GemColor.GOLDEN] == 0)
                                {
                                    currentPlayer.Resources.gems.Remove(GemColor.GOLDEN);
                                }
                            }

                            game.RefillBankResources(bank, board.level2VisibleCards[3], simulatedResourcesUsed);

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                bank.AddGoldenGem(simulatedResourcesUsed[GemColor.GOLDEN]);
                            }

                            currentPlayer.AddCardToPlayer(board.level2VisibleCards[3]);
                            currentPlayer.BonusResources.AddResource(board.level2VisibleCards[3].BonusColor);
                            currentPlayer.Points += board.level2VisibleCards[3].Points;

                            board.ReplaceMissingCard(2, board.level2VisibleCards[3]);
                            return true;
                        }
                        return false;
                    case 9:
                        if (currentPlayer.CanAffordCard(board.level3VisibleCards[0]) || currentPlayer.CanAffordCardWithGolden(board.level3VisibleCards[0]))
                        {
                            var simulatedResourcesUsed = new Dictionary<GemColor, int>();
                            foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
                            {
                                simulatedResourcesUsed[color] = 0;
                            }
                            if (board.level3VisibleCards[0] == null)
                            {
                                throw new Exception("Nie działa try, catch :/");
                            }
                            var cardPrice = board.level3VisibleCards[0].DetailedPrice.gems;

                            foreach (var colorOnCard in cardPrice)
                            {
                                GemColor color = colorOnCard.Key;
                                int requiredAmount = colorOnCard.Value;

                                int bonusAmount = currentPlayer.BonusResources.gems.TryGetValue(color, out var bonus) ? bonus : 0;
                                requiredAmount -= Math.Min(bonusAmount, requiredAmount);

                                if (requiredAmount > 0)
                                {
                                    int playerAmount = currentPlayer.Resources.gems.TryGetValue(color, out var playerR) ? playerR : 0;

                                    if (playerAmount >= requiredAmount)
                                    {
                                        simulatedResourcesUsed[color] += requiredAmount;
                                    }
                                    else
                                    {
                                        int deficit = requiredAmount - playerAmount;
                                        simulatedResourcesUsed[color] += playerAmount;
                                        simulatedResourcesUsed[GemColor.GOLDEN] += deficit;
                                    }
                                }
                            }
                            foreach (var resource in simulatedResourcesUsed)
                            {
                                if (resource.Value > 0 && resource.Key != GemColor.GOLDEN)
                                {
                                    currentPlayer.Resources.gems[resource.Key] -= resource.Value;
                                    if (currentPlayer.Resources.gems[resource.Key] == 0)
                                        currentPlayer.Resources.gems.Remove(resource.Key);
                                }
                            }

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                currentPlayer.Resources.gems[GemColor.GOLDEN] -= simulatedResourcesUsed[GemColor.GOLDEN];
                                if (currentPlayer.Resources.gems[GemColor.GOLDEN] == 0)
                                {
                                    currentPlayer.Resources.gems.Remove(GemColor.GOLDEN);
                                }
                            }

                            game.RefillBankResources(bank, board.level3VisibleCards[0], simulatedResourcesUsed);

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                bank.AddGoldenGem(simulatedResourcesUsed[GemColor.GOLDEN]);
                            }

                            currentPlayer.AddCardToPlayer(board.level3VisibleCards[0]);
                            currentPlayer.BonusResources.AddResource(board.level3VisibleCards[0].BonusColor);
                            currentPlayer.Points += board.level3VisibleCards[0].Points;

                            board.ReplaceMissingCard(3, board.level3VisibleCards[0]);
                            return true;
                        }
                        return false;
                    case 10:
                        if (currentPlayer.CanAffordCard(board.level3VisibleCards[1]) || currentPlayer.CanAffordCardWithGolden(board.level3VisibleCards[1]))
                        {
                            var simulatedResourcesUsed = new Dictionary<GemColor, int>();
                            foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
                            {
                                simulatedResourcesUsed[color] = 0;
                            }
                            if (board.level3VisibleCards[1] == null)
                            {
                                throw new Exception("Nie działa try, catch :/");
                            }
                            var cardPrice = board.level3VisibleCards[1].DetailedPrice.gems;

                            foreach (var colorOnCard in cardPrice)
                            {
                                GemColor color = colorOnCard.Key;
                                int requiredAmount = colorOnCard.Value;

                                int bonusAmount = currentPlayer.BonusResources.gems.TryGetValue(color, out var bonus) ? bonus : 0;
                                requiredAmount -= Math.Min(bonusAmount, requiredAmount);

                                if (requiredAmount > 0)
                                {
                                    int playerAmount = currentPlayer.Resources.gems.TryGetValue(color, out var playerR) ? playerR : 0;

                                    if (playerAmount >= requiredAmount)
                                    {
                                        simulatedResourcesUsed[color] += requiredAmount;
                                    }
                                    else
                                    {
                                        int deficit = requiredAmount - playerAmount;
                                        simulatedResourcesUsed[color] += playerAmount;
                                        simulatedResourcesUsed[GemColor.GOLDEN] += deficit;
                                    }
                                }
                            }
                            foreach (var resource in simulatedResourcesUsed)
                            {
                                if (resource.Value > 0 && resource.Key != GemColor.GOLDEN)
                                {
                                    currentPlayer.Resources.gems[resource.Key] -= resource.Value;
                                    if (currentPlayer.Resources.gems[resource.Key] == 0)
                                        currentPlayer.Resources.gems.Remove(resource.Key);
                                }
                            }

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                currentPlayer.Resources.gems[GemColor.GOLDEN] -= simulatedResourcesUsed[GemColor.GOLDEN];
                                if (currentPlayer.Resources.gems[GemColor.GOLDEN] == 0)
                                {
                                    currentPlayer.Resources.gems.Remove(GemColor.GOLDEN);
                                }
                            }

                            game.RefillBankResources(bank, board.level3VisibleCards[1], simulatedResourcesUsed);

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                bank.AddGoldenGem(simulatedResourcesUsed[GemColor.GOLDEN]);
                            }

                            currentPlayer.AddCardToPlayer(board.level3VisibleCards[1]);
                            currentPlayer.BonusResources.AddResource(board.level3VisibleCards[1].BonusColor);
                            currentPlayer.Points += board.level3VisibleCards[1].Points;

                            board.ReplaceMissingCard(3, board.level3VisibleCards[1]);
                            return true;
                        }
                        return false;
                    case 11:
                        if (currentPlayer.CanAffordCard(board.level3VisibleCards[2]) || currentPlayer.CanAffordCardWithGolden(board.level3VisibleCards[2]))
                        {
                            var simulatedResourcesUsed = new Dictionary<GemColor, int>();
                            foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
                            {
                                simulatedResourcesUsed[color] = 0;
                            }
                            if (board.level3VisibleCards[2] == null)
                            {
                                throw new Exception("Nie działa try, catch :/");
                            }
                            var cardPrice = board.level3VisibleCards[2].DetailedPrice.gems;

                            foreach (var colorOnCard in cardPrice)
                            {
                                GemColor color = colorOnCard.Key;
                                int requiredAmount = colorOnCard.Value;

                                int bonusAmount = currentPlayer.BonusResources.gems.TryGetValue(color, out var bonus) ? bonus : 0;
                                requiredAmount -= Math.Min(bonusAmount, requiredAmount);

                                if (requiredAmount > 0)
                                {
                                    int playerAmount = currentPlayer.Resources.gems.TryGetValue(color, out var playerR) ? playerR : 0;

                                    if (playerAmount >= requiredAmount)
                                    {
                                        simulatedResourcesUsed[color] += requiredAmount;
                                    }
                                    else
                                    {
                                        int deficit = requiredAmount - playerAmount;
                                        simulatedResourcesUsed[color] += playerAmount;
                                        simulatedResourcesUsed[GemColor.GOLDEN] += deficit;
                                    }
                                }
                            }
                            foreach (var resource in simulatedResourcesUsed)
                            {
                                if (resource.Value > 0 && resource.Key != GemColor.GOLDEN)
                                {
                                    currentPlayer.Resources.gems[resource.Key] -= resource.Value;
                                    if (currentPlayer.Resources.gems[resource.Key] == 0)
                                        currentPlayer.Resources.gems.Remove(resource.Key);
                                }
                            }

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                currentPlayer.Resources.gems[GemColor.GOLDEN] -= simulatedResourcesUsed[GemColor.GOLDEN];
                                if (currentPlayer.Resources.gems[GemColor.GOLDEN] == 0)
                                {
                                    currentPlayer.Resources.gems.Remove(GemColor.GOLDEN);
                                }
                            }

                            game.RefillBankResources(bank, board.level3VisibleCards[2], simulatedResourcesUsed);

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                bank.AddGoldenGem(simulatedResourcesUsed[GemColor.GOLDEN]);
                            }

                            currentPlayer.AddCardToPlayer(board.level3VisibleCards[2]);
                            currentPlayer.BonusResources.AddResource(board.level3VisibleCards[2].BonusColor);
                            currentPlayer.Points += board.level3VisibleCards[2].Points;

                            board.ReplaceMissingCard(3, board.level3VisibleCards[2]);
                            return true;
                        }
                        return false;
                    case 12:
                        if (currentPlayer.CanAffordCard(board.level3VisibleCards[3]) || currentPlayer.CanAffordCardWithGolden(board.level3VisibleCards[3]))
                        {
                            var simulatedResourcesUsed = new Dictionary<GemColor, int>();
                            foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
                            {
                                simulatedResourcesUsed[color] = 0;
                            }
                            if (board.level3VisibleCards[3] == null)
                            {
                                throw new Exception("Nie działa try, catch :/");
                            }
                            var cardPrice = board.level3VisibleCards[3].DetailedPrice.gems;

                            foreach (var colorOnCard in cardPrice)
                            {
                                GemColor color = colorOnCard.Key;
                                int requiredAmount = colorOnCard.Value;

                                int bonusAmount = currentPlayer.BonusResources.gems.TryGetValue(color, out var bonus) ? bonus : 0;
                                requiredAmount -= Math.Min(bonusAmount, requiredAmount);

                                if (requiredAmount > 0)
                                {
                                    int playerAmount = currentPlayer.Resources.gems.TryGetValue(color, out var playerR) ? playerR : 0;

                                    if (playerAmount >= requiredAmount)
                                    {
                                        simulatedResourcesUsed[color] += requiredAmount;
                                    }
                                    else
                                    {
                                        int deficit = requiredAmount - playerAmount;
                                        simulatedResourcesUsed[color] += playerAmount;
                                        simulatedResourcesUsed[GemColor.GOLDEN] += deficit;
                                    }
                                }
                            }
                            foreach (var resource in simulatedResourcesUsed)
                            {
                                if (resource.Value > 0 && resource.Key != GemColor.GOLDEN)
                                {
                                    currentPlayer.Resources.gems[resource.Key] -= resource.Value;
                                    if (currentPlayer.Resources.gems[resource.Key] == 0)
                                        currentPlayer.Resources.gems.Remove(resource.Key);
                                }
                            }

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                currentPlayer.Resources.gems[GemColor.GOLDEN] -= simulatedResourcesUsed[GemColor.GOLDEN];
                                if (currentPlayer.Resources.gems[GemColor.GOLDEN] == 0)
                                {
                                    currentPlayer.Resources.gems.Remove(GemColor.GOLDEN);
                                }
                            }

                            game.RefillBankResources(bank, board.level3VisibleCards[3], simulatedResourcesUsed);

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                bank.AddGoldenGem(simulatedResourcesUsed[GemColor.GOLDEN]);
                            }

                            currentPlayer.AddCardToPlayer(board.level3VisibleCards[3]);
                            currentPlayer.BonusResources.AddResource(board.level3VisibleCards[3].BonusColor);
                            currentPlayer.Points += board.level3VisibleCards[3].Points;

                            board.ReplaceMissingCard(3, board.level3VisibleCards[3]);
                            return true;
                        }
                        return false;
                    case 13:
                        if (game.NumberOfPlayerTokens() <= 8 && bank.resources.gems[GemColor.WHITE] >= 4)
                        {
                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.WHITE))
                            {
                                currentPlayer.Resources.gems[GemColor.WHITE] += 2;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.WHITE, 2);
                            }
                            bank.TakeOutResources(2, GemColor.WHITE);
                            return true;
                        }
                        return false;
                    case 14:
                        if (game.NumberOfPlayerTokens() <= 8 && bank.resources.gems[GemColor.BLUE] >= 4)
                        {
                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.BLUE))
                            {
                                currentPlayer.Resources.gems[GemColor.BLUE] += 2;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.BLUE, 2);
                            }
                            bank.TakeOutResources(2, GemColor.BLUE);
                            return true;
                        }
                        return false;
                    case 15:
                        if (game.NumberOfPlayerTokens() <= 8 && bank.resources.gems[GemColor.GREEN] >= 4)
                        {
                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.GREEN))
                            {
                                currentPlayer.Resources.gems[GemColor.GREEN] += 2;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.GREEN, 2);
                            }
                            bank.TakeOutResources(2, GemColor.GREEN);
                            return true;
                        }
                        return false;
                    case 16:
                        if (game.NumberOfPlayerTokens() <= 8 && bank.resources.gems[GemColor.RED] >= 4)
                        {
                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.RED))
                            {
                                currentPlayer.Resources.gems[GemColor.RED] += 2;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.RED, 2);
                            }
                            bank.TakeOutResources(2, GemColor.RED);
                            return true;
                        }
                        return false;
                    case 17:
                        if (game.NumberOfPlayerTokens() <= 8 && bank.resources.gems[GemColor.BLACK] >= 4)
                        {
                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.BLACK))
                            {
                                currentPlayer.Resources.gems[GemColor.BLACK] += 2;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.BLACK, 2);
                            }
                            bank.TakeOutResources(2, GemColor.BLACK);
                            return true;
                        }
                        return false;
                    case 18:
                        if (game.NumberOfPlayerTokens() <= 7 && bank.resources.gems[GemColor.GREEN] >= 1 && bank.resources.gems[GemColor.RED] >= 1 && bank.resources.gems[GemColor.BLACK] >= 1)
                        {
                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.GREEN))
                            {
                                currentPlayer.Resources.gems[GemColor.GREEN] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.GREEN, 1);
                            }

                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.RED))
                            {
                                currentPlayer.Resources.gems[GemColor.RED] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.RED, 1);
                            }

                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.BLACK))
                            {
                                currentPlayer.Resources.gems[GemColor.BLACK] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.BLACK, 1);
                            }

                            bank.TakeOutResources(1, GemColor.GREEN);
                            bank.TakeOutResources(1, GemColor.RED);
                            bank.TakeOutResources(1, GemColor.BLACK);
                            return true;
                        }
                        return false;
                    case 19:
                        if (game.NumberOfPlayerTokens() <= 7 && bank.resources.gems[GemColor.BLUE] >= 1 && bank.resources.gems[GemColor.RED] >= 1 && bank.resources.gems[GemColor.BLACK] >= 1)
                        {
                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.BLUE))
                            {
                                currentPlayer.Resources.gems[GemColor.BLUE] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.BLUE, 1);
                            }

                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.RED))
                            {
                                currentPlayer.Resources.gems[GemColor.RED] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.RED, 1);
                            }

                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.BLACK))
                            {
                                currentPlayer.Resources.gems[GemColor.BLACK] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.BLACK, 1);
                            }

                            bank.TakeOutResources(1, GemColor.BLUE);
                            bank.TakeOutResources(1, GemColor.RED);
                            bank.TakeOutResources(1, GemColor.BLACK);
                            return true;
                        }
                        return false;
                    case 20:
                        if (game.NumberOfPlayerTokens() <= 7 && bank.resources.gems[GemColor.BLUE] >= 1 && bank.resources.gems[GemColor.GREEN] >= 1 && bank.resources.gems[GemColor.BLACK] >= 1)
                        {
                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.BLUE))
                            {
                                currentPlayer.Resources.gems[GemColor.BLUE] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.BLUE, 1);
                            }

                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.GREEN))
                            {
                                currentPlayer.Resources.gems[GemColor.GREEN] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.GREEN, 1);
                            }

                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.BLACK))
                            {
                                currentPlayer.Resources.gems[GemColor.BLACK] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.BLACK, 1);
                            }

                            bank.TakeOutResources(1, GemColor.BLUE);
                            bank.TakeOutResources(1, GemColor.GREEN);
                            bank.TakeOutResources(1, GemColor.BLACK);
                            return true;
                        }
                        return false;
                    case 21:
                        if (game.NumberOfPlayerTokens() <= 7 && bank.resources.gems[GemColor.BLUE] >= 1 && bank.resources.gems[GemColor.GREEN] >= 1 && bank.resources.gems[GemColor.RED] >= 1)
                        {
                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.BLUE))
                            {
                                currentPlayer.Resources.gems[GemColor.BLUE] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.BLUE, 1);
                            }

                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.GREEN))
                            {
                                currentPlayer.Resources.gems[GemColor.GREEN] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.GREEN, 1);
                            }

                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.RED))
                            {
                                currentPlayer.Resources.gems[GemColor.RED] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.RED, 1);
                            }

                            bank.TakeOutResources(1, GemColor.BLUE);
                            bank.TakeOutResources(1, GemColor.GREEN);
                            bank.TakeOutResources(1, GemColor.RED);
                            return true;
                        }
                        return false;
                    case 22:
                        if (game.NumberOfPlayerTokens() <= 7 && bank.resources.gems[GemColor.WHITE] >= 1 && bank.resources.gems[GemColor.RED] >= 1 && bank.resources.gems[GemColor.BLACK] >= 1)
                        {
                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.WHITE))
                            {
                                currentPlayer.Resources.gems[GemColor.WHITE] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.WHITE, 1);
                            }

                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.RED))
                            {
                                currentPlayer.Resources.gems[GemColor.RED] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.RED, 1);
                            }

                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.BLACK))
                            {
                                currentPlayer.Resources.gems[GemColor.BLACK] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.BLACK, 1);
                            }

                            bank.TakeOutResources(1, GemColor.WHITE);
                            bank.TakeOutResources(1, GemColor.RED);
                            bank.TakeOutResources(1, GemColor.BLACK);
                            return true;
                        }
                        return false;
                    case 23:
                        if (game.NumberOfPlayerTokens() <= 7 && bank.resources.gems[GemColor.WHITE] >= 1 && bank.resources.gems[GemColor.GREEN] >= 1 && bank.resources.gems[GemColor.BLACK] >= 1)
                        {
                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.WHITE))
                            {
                                currentPlayer.Resources.gems[GemColor.WHITE] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.WHITE, 1);
                            }

                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.GREEN))
                            {
                                currentPlayer.Resources.gems[GemColor.GREEN] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.GREEN, 1);
                            }

                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.BLACK))
                            {
                                currentPlayer.Resources.gems[GemColor.BLACK] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.BLACK, 1);
                            }

                            bank.TakeOutResources(1, GemColor.WHITE);
                            bank.TakeOutResources(1, GemColor.GREEN);
                            bank.TakeOutResources(1, GemColor.BLACK);
                            return true;
                        }
                        return false;
                    case 24:
                        if (game.NumberOfPlayerTokens() <= 7 && bank.resources.gems[GemColor.WHITE] >= 1 && bank.resources.gems[GemColor.GREEN] >= 1 && bank.resources.gems[GemColor.RED] >= 1)
                        {
                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.WHITE))
                            {
                                currentPlayer.Resources.gems[GemColor.WHITE] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.WHITE, 1);
                            }

                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.GREEN))
                            {
                                currentPlayer.Resources.gems[GemColor.GREEN] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.GREEN, 1);
                            }

                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.RED))
                            {
                                currentPlayer.Resources.gems[GemColor.RED] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.RED, 1);
                            }

                            bank.TakeOutResources(1, GemColor.WHITE);
                            bank.TakeOutResources(1, GemColor.GREEN);
                            bank.TakeOutResources(1, GemColor.RED);
                            return true;
                        }
                        return false;
                    case 25:
                        if (game.NumberOfPlayerTokens() <= 7 && bank.resources.gems[GemColor.WHITE] >= 1 && bank.resources.gems[GemColor.BLUE] >= 1 && bank.resources.gems[GemColor.BLACK] >= 1)
                        {
                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.WHITE))
                            {
                                currentPlayer.Resources.gems[GemColor.WHITE] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.WHITE, 1);
                            }

                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.BLUE))
                            {
                                currentPlayer.Resources.gems[GemColor.BLUE] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.BLUE, 1);
                            }

                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.BLACK))
                            {
                                currentPlayer.Resources.gems[GemColor.BLACK] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.BLACK, 1);
                            }

                            bank.TakeOutResources(1, GemColor.WHITE);
                            bank.TakeOutResources(1, GemColor.BLUE);
                            bank.TakeOutResources(1, GemColor.BLACK);
                            return true;
                        }
                        return false;
                    case 26:
                        if (game.NumberOfPlayerTokens() <= 7 && bank.resources.gems[GemColor.WHITE] >= 1 && bank.resources.gems[GemColor.BLUE] >= 1 && bank.resources.gems[GemColor.RED] >= 1)
                        {
                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.WHITE))
                            {
                                currentPlayer.Resources.gems[GemColor.WHITE] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.WHITE, 1);
                            }

                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.BLUE))
                            {
                                currentPlayer.Resources.gems[GemColor.BLUE] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.BLUE, 1);
                            }

                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.RED))
                            {
                                currentPlayer.Resources.gems[GemColor.RED] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.RED, 1);
                            }

                            bank.TakeOutResources(1, GemColor.WHITE);
                            bank.TakeOutResources(1, GemColor.BLUE);
                            bank.TakeOutResources(1, GemColor.RED);
                            return true;
                        }
                        return false;
                    case 27:
                        if (game.NumberOfPlayerTokens() <= 7 && bank.resources.gems[GemColor.WHITE] >= 1 && bank.resources.gems[GemColor.BLUE] >= 1 && bank.resources.gems[GemColor.GREEN] >= 1)
                        {
                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.WHITE))
                            {
                                currentPlayer.Resources.gems[GemColor.WHITE] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.WHITE, 1);
                            }

                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.BLUE))
                            {
                                currentPlayer.Resources.gems[GemColor.BLUE] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.BLUE, 1);
                            }

                            if (currentPlayer.Resources.gems.ContainsKey(GemColor.GREEN))
                            {
                                currentPlayer.Resources.gems[GemColor.GREEN] += 1;
                            }
                            else
                            {
                                currentPlayer.Resources.gems.Add(GemColor.GREEN, 1);
                            }

                            bank.TakeOutResources(1, GemColor.WHITE);
                            bank.TakeOutResources(1, GemColor.BLUE);
                            bank.TakeOutResources(1, GemColor.GREEN);
                            return true;
                        }
                        return false;
                    case 28:
                        if (currentPlayer.ReservedCardsCounter < 3)
                        {
                            if (bank.resources.gems[GemColor.GOLDEN] > 0)
                            {

                                if (currentPlayer.Resources.gems.ContainsKey(GemColor.GOLDEN))
                                {
                                    currentPlayer.Resources.gems[GemColor.GOLDEN] += 1;
                                }
                                else
                                {
                                    currentPlayer.Resources.gems.Add(GemColor.GOLDEN, 1);
                                }
                                bank.TakeOutResources(1, GemColor.GOLDEN);
                            }
                            currentPlayer.ReserveCard(board.level1VisibleCards[0]);
                            board.ReplaceMissingCard(1, board.level1VisibleCards[0]);
                            return true;
                        }
                        return false;
                    case 29:
                        if (currentPlayer.ReservedCardsCounter < 3)
                        {
                            if (bank.resources.gems[GemColor.GOLDEN] > 0)
                            {

                                if (currentPlayer.Resources.gems.ContainsKey(GemColor.GOLDEN))
                                {
                                    currentPlayer.Resources.gems[GemColor.GOLDEN] += 1;
                                }
                                else
                                {
                                    currentPlayer.Resources.gems.Add(GemColor.GOLDEN, 1);
                                }
                                bank.TakeOutResources(1, GemColor.GOLDEN);
                            }
                            currentPlayer.ReserveCard(board.level1VisibleCards[1]);
                            board.ReplaceMissingCard(1, board.level1VisibleCards[1]);
                            return true;
                        }
                        return false;
                    case 30:
                        if (currentPlayer.ReservedCardsCounter < 3)
                        {
                            if (bank.resources.gems[GemColor.GOLDEN] > 0)
                            {

                                if (currentPlayer.Resources.gems.ContainsKey(GemColor.GOLDEN))
                                {
                                    currentPlayer.Resources.gems[GemColor.GOLDEN] += 1;
                                }
                                else
                                {
                                    currentPlayer.Resources.gems.Add(GemColor.GOLDEN, 1);
                                }
                                bank.TakeOutResources(1, GemColor.GOLDEN);
                            }
                            currentPlayer.ReserveCard(board.level1VisibleCards[2]);
                            board.ReplaceMissingCard(1, board.level1VisibleCards[2]);
                            return true;
                        }
                        return false;
                    case 31:
                        if (currentPlayer.ReservedCardsCounter < 3)
                        {
                            if (bank.resources.gems[GemColor.GOLDEN] > 0)
                            {

                                if (currentPlayer.Resources.gems.ContainsKey(GemColor.GOLDEN))
                                {
                                    currentPlayer.Resources.gems[GemColor.GOLDEN] += 1;
                                }
                                else
                                {
                                    currentPlayer.Resources.gems.Add(GemColor.GOLDEN, 1);
                                }
                                bank.TakeOutResources(1, GemColor.GOLDEN);
                            }
                            currentPlayer.ReserveCard(board.level1VisibleCards[3]);
                            board.ReplaceMissingCard(1, board.level1VisibleCards[3]);
                            return true;
                        }
                        return false;
                    case 32:
                        if (currentPlayer.ReservedCardsCounter < 3)
                        {
                            if (bank.resources.gems[GemColor.GOLDEN] > 0)
                            {

                                if (currentPlayer.Resources.gems.ContainsKey(GemColor.GOLDEN))
                                {
                                    currentPlayer.Resources.gems[GemColor.GOLDEN] += 1;
                                }
                                else
                                {
                                    currentPlayer.Resources.gems.Add(GemColor.GOLDEN, 1);
                                }
                                bank.TakeOutResources(1, GemColor.GOLDEN);
                            }
                            currentPlayer.ReserveCard(board.level2VisibleCards[0]);
                            board.ReplaceMissingCard(2, board.level2VisibleCards[0]);
                            return true;
                        }
                        return false;
                    case 33:
                        if (currentPlayer.ReservedCardsCounter < 3)
                        {
                            if (bank.resources.gems[GemColor.GOLDEN] > 0)
                            {

                                if (currentPlayer.Resources.gems.ContainsKey(GemColor.GOLDEN))
                                {
                                    currentPlayer.Resources.gems[GemColor.GOLDEN] += 1;
                                }
                                else
                                {
                                    currentPlayer.Resources.gems.Add(GemColor.GOLDEN, 1);
                                }
                                bank.TakeOutResources(1, GemColor.GOLDEN);
                            }
                            currentPlayer.ReserveCard(board.level2VisibleCards[1]);
                            board.ReplaceMissingCard(2, board.level2VisibleCards[1]);
                            return true;
                        }
                        return false;
                    case 34:
                        if (currentPlayer.ReservedCardsCounter < 3)
                        {
                            if (bank.resources.gems[GemColor.GOLDEN] > 0)
                            {

                                if (currentPlayer.Resources.gems.ContainsKey(GemColor.GOLDEN))
                                {
                                    currentPlayer.Resources.gems[GemColor.GOLDEN] += 1;
                                }
                                else
                                {
                                    currentPlayer.Resources.gems.Add(GemColor.GOLDEN, 1);
                                }
                                bank.TakeOutResources(1, GemColor.GOLDEN);
                            }
                            currentPlayer.ReserveCard(board.level2VisibleCards[2]);
                            board.ReplaceMissingCard(2, board.level2VisibleCards[2]);
                            return true;
                        }
                        return false;
                    case 35:
                        if (currentPlayer.ReservedCardsCounter < 3)
                        {
                            if (bank.resources.gems[GemColor.GOLDEN] > 0)
                            {

                                if (currentPlayer.Resources.gems.ContainsKey(GemColor.GOLDEN))
                                {
                                    currentPlayer.Resources.gems[GemColor.GOLDEN] += 1;
                                }
                                else
                                {
                                    currentPlayer.Resources.gems.Add(GemColor.GOLDEN, 1);
                                }
                                bank.TakeOutResources(1, GemColor.GOLDEN);
                            }
                            currentPlayer.ReserveCard(board.level2VisibleCards[3]);
                            board.ReplaceMissingCard(2, board.level2VisibleCards[3]);
                            return true;
                        }
                        return false;
                    case 36:
                        if (currentPlayer.ReservedCardsCounter < 3)
                        {
                            if (bank.resources.gems[GemColor.GOLDEN] > 0)
                            {

                                if (currentPlayer.Resources.gems.ContainsKey(GemColor.GOLDEN))
                                {
                                    currentPlayer.Resources.gems[GemColor.GOLDEN] += 1;
                                }
                                else
                                {
                                    currentPlayer.Resources.gems.Add(GemColor.GOLDEN, 1);
                                }
                                bank.TakeOutResources(1, GemColor.GOLDEN);
                            }
                            currentPlayer.ReserveCard(board.level3VisibleCards[0]);
                            board.ReplaceMissingCard(3, board.level3VisibleCards[0]);
                            return true;
                        }
                        return false;
                    case 37:
                        if (currentPlayer.ReservedCardsCounter < 3)
                        {
                            if (bank.resources.gems[GemColor.GOLDEN] > 0)
                            {

                                if (currentPlayer.Resources.gems.ContainsKey(GemColor.GOLDEN))
                                {
                                    currentPlayer.Resources.gems[GemColor.GOLDEN] += 1;
                                }
                                else
                                {
                                    currentPlayer.Resources.gems.Add(GemColor.GOLDEN, 1);
                                }
                                bank.TakeOutResources(1, GemColor.GOLDEN);
                            }
                            currentPlayer.ReserveCard(board.level3VisibleCards[1]);
                            board.ReplaceMissingCard(3, board.level3VisibleCards[1]);
                            return true;
                        }
                        return false;
                    case 38:
                        if (currentPlayer.ReservedCardsCounter < 3)
                        {
                            if (bank.resources.gems[GemColor.GOLDEN] > 0)
                            {

                                if (currentPlayer.Resources.gems.ContainsKey(GemColor.GOLDEN))
                                {
                                    currentPlayer.Resources.gems[GemColor.GOLDEN] += 1;
                                }
                                else
                                {
                                    currentPlayer.Resources.gems.Add(GemColor.GOLDEN, 1);
                                }
                                bank.TakeOutResources(1, GemColor.GOLDEN);
                            }
                            currentPlayer.ReserveCard(board.level3VisibleCards[2]);
                            board.ReplaceMissingCard(3, board.level3VisibleCards[2]);
                            return true;
                        }
                        return false;
                    case 39:
                        if (currentPlayer.ReservedCardsCounter < 3)
                        {
                            if (bank.resources.gems[GemColor.GOLDEN] > 0)
                            {

                                if (currentPlayer.Resources.gems.ContainsKey(GemColor.GOLDEN))
                                {
                                    currentPlayer.Resources.gems[GemColor.GOLDEN] += 1;
                                }
                                else
                                {
                                    currentPlayer.Resources.gems.Add(GemColor.GOLDEN, 1);
                                }
                                bank.TakeOutResources(1, GemColor.GOLDEN);
                            }
                            currentPlayer.ReserveCard(board.level3VisibleCards[3]);
                            board.ReplaceMissingCard(3, board.level3VisibleCards[3]);
                            return true;
                        }
                        return false;
                    case 40:
                        if (currentPlayer.ReservedCardsCounter > 0 && (currentPlayer.CanAffordCard(currentPlayer.ReservedCards[0]) || currentPlayer.CanAffordCardWithGolden(currentPlayer.ReservedCards[0])))
                        {
                            var simulatedResourcesUsed = new Dictionary<GemColor, int>();
                            foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
                            {
                                simulatedResourcesUsed[color] = 0;
                            }
                            var cardPrice = currentPlayer.ReservedCards[0].DetailedPrice.gems;

                            foreach (var colorOnCard in cardPrice)
                            {
                                GemColor color = colorOnCard.Key;
                                int requiredAmount = colorOnCard.Value;

                                int bonusAmount = currentPlayer.BonusResources.gems.TryGetValue(color, out var bonus) ? bonus : 0;
                                requiredAmount -= Math.Min(bonusAmount, requiredAmount);

                                if (requiredAmount > 0)
                                {
                                    int playerAmount = currentPlayer.Resources.gems.TryGetValue(color, out var playerR) ? playerR : 0;

                                    if (playerAmount >= requiredAmount)
                                    {
                                        simulatedResourcesUsed[color] += requiredAmount;
                                    }
                                    else
                                    {
                                        int deficit = requiredAmount - playerAmount;
                                        simulatedResourcesUsed[color] += playerAmount;
                                        simulatedResourcesUsed[GemColor.GOLDEN] += deficit;
                                    }
                                }
                            }
                            foreach (var resource in simulatedResourcesUsed)
                            {
                                if (resource.Value > 0 && resource.Key != GemColor.GOLDEN)
                                {
                                    currentPlayer.Resources.gems[resource.Key] -= resource.Value;
                                    if (currentPlayer.Resources.gems[resource.Key] == 0)
                                        currentPlayer.Resources.gems.Remove(resource.Key);
                                }
                            }

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                currentPlayer.Resources.gems[GemColor.GOLDEN] -= simulatedResourcesUsed[GemColor.GOLDEN];
                                if (currentPlayer.Resources.gems[GemColor.GOLDEN] == 0)
                                {
                                    currentPlayer.Resources.gems.Remove(GemColor.GOLDEN);
                                }
                            }

                            game.RefillBankResources(bank, currentPlayer.ReservedCards[0], simulatedResourcesUsed);

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                bank.AddGoldenGem(simulatedResourcesUsed[GemColor.GOLDEN]);
                            }

                            currentPlayer.AddCardToPlayer(currentPlayer.ReservedCards[0]);
                            currentPlayer.BonusResources.AddResource(currentPlayer.ReservedCards[0].BonusColor);
                            currentPlayer.Points += currentPlayer.ReservedCards[0].Points;


                            Card selectedCard = currentPlayer.ReservedCards[0];

                            currentPlayer.ReservedCards.Remove(selectedCard);
                            currentPlayer.ReservedCardsCounter--;

                            return true;
                        }
                        return false;
                    case 41:
                        if (currentPlayer.ReservedCardsCounter > 1 && (currentPlayer.CanAffordCard(currentPlayer.ReservedCards[1]) || currentPlayer.CanAffordCardWithGolden(currentPlayer.ReservedCards[1])))
                        {
                            var simulatedResourcesUsed = new Dictionary<GemColor, int>();
                            foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
                            {
                                simulatedResourcesUsed[color] = 0;
                            }
                            var cardPrice = currentPlayer.ReservedCards[1].DetailedPrice.gems;

                            foreach (var colorOnCard in cardPrice)
                            {
                                GemColor color = colorOnCard.Key;
                                int requiredAmount = colorOnCard.Value;

                                int bonusAmount = currentPlayer.BonusResources.gems.TryGetValue(color, out var bonus) ? bonus : 0;
                                requiredAmount -= Math.Min(bonusAmount, requiredAmount);

                                if (requiredAmount > 0)
                                {
                                    int playerAmount = currentPlayer.Resources.gems.TryGetValue(color, out var playerR) ? playerR : 0;

                                    if (playerAmount >= requiredAmount)
                                    {
                                        simulatedResourcesUsed[color] += requiredAmount;
                                    }
                                    else
                                    {
                                        int deficit = requiredAmount - playerAmount;
                                        simulatedResourcesUsed[color] += playerAmount;
                                        simulatedResourcesUsed[GemColor.GOLDEN] += deficit;
                                    }
                                }
                            }
                            foreach (var resource in simulatedResourcesUsed)
                            {
                                if (resource.Value > 0 && resource.Key != GemColor.GOLDEN)
                                {
                                    currentPlayer.Resources.gems[resource.Key] -= resource.Value;
                                    if (currentPlayer.Resources.gems[resource.Key] == 0)
                                        currentPlayer.Resources.gems.Remove(resource.Key);
                                }
                            }

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                currentPlayer.Resources.gems[GemColor.GOLDEN] -= simulatedResourcesUsed[GemColor.GOLDEN];
                                if (currentPlayer.Resources.gems[GemColor.GOLDEN] == 0)
                                {
                                    currentPlayer.Resources.gems.Remove(GemColor.GOLDEN);
                                }
                            }

                            game.RefillBankResources(bank, currentPlayer.ReservedCards[1], simulatedResourcesUsed);

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                bank.AddGoldenGem(simulatedResourcesUsed[GemColor.GOLDEN]);
                            }

                            currentPlayer.AddCardToPlayer(currentPlayer.ReservedCards[1]);
                            currentPlayer.BonusResources.AddResource(currentPlayer.ReservedCards[1].BonusColor);
                            currentPlayer.Points += currentPlayer.ReservedCards[1].Points;


                            Card selectedCard = currentPlayer.ReservedCards[1];

                            currentPlayer.ReservedCards.Remove(selectedCard);
                            currentPlayer.ReservedCardsCounter--;

                            return true;
                        }
                        return false;
                    case 42:
                        if (currentPlayer.ReservedCardsCounter == 3 && (currentPlayer.CanAffordCard(currentPlayer.ReservedCards[2]) || currentPlayer.CanAffordCardWithGolden(currentPlayer.ReservedCards[2])))
                        {
                            var simulatedResourcesUsed = new Dictionary<GemColor, int>();
                            foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
                            {
                                simulatedResourcesUsed[color] = 0;
                            }
                            var cardPrice = currentPlayer.ReservedCards[2].DetailedPrice.gems;

                            foreach (var colorOnCard in cardPrice)
                            {
                                GemColor color = colorOnCard.Key;
                                int requiredAmount = colorOnCard.Value;

                                int bonusAmount = currentPlayer.BonusResources.gems.TryGetValue(color, out var bonus) ? bonus : 0;
                                requiredAmount -= Math.Min(bonusAmount, requiredAmount);

                                if (requiredAmount > 0)
                                {
                                    int playerAmount = currentPlayer.Resources.gems.TryGetValue(color, out var playerR) ? playerR : 0;

                                    if (playerAmount >= requiredAmount)
                                    {
                                        simulatedResourcesUsed[color] += requiredAmount;
                                    }
                                    else
                                    {
                                        int deficit = requiredAmount - playerAmount;
                                        simulatedResourcesUsed[color] += playerAmount;
                                        simulatedResourcesUsed[GemColor.GOLDEN] += deficit;
                                    }
                                }
                            }
                            foreach (var resource in simulatedResourcesUsed)
                            {
                                if (resource.Value > 0 && resource.Key != GemColor.GOLDEN)
                                {
                                    currentPlayer.Resources.gems[resource.Key] -= resource.Value;
                                    if (currentPlayer.Resources.gems[resource.Key] == 0)
                                        currentPlayer.Resources.gems.Remove(resource.Key);
                                }
                            }

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                currentPlayer.Resources.gems[GemColor.GOLDEN] -= simulatedResourcesUsed[GemColor.GOLDEN];
                                if (currentPlayer.Resources.gems[GemColor.GOLDEN] == 0)
                                {
                                    currentPlayer.Resources.gems.Remove(GemColor.GOLDEN);
                                }
                            }

                            game.RefillBankResources(bank, currentPlayer.ReservedCards[2], simulatedResourcesUsed);

                            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
                            {
                                bank.AddGoldenGem(simulatedResourcesUsed[GemColor.GOLDEN]);
                            }

                            currentPlayer.AddCardToPlayer(currentPlayer.ReservedCards[2]);
                            currentPlayer.BonusResources.AddResource(currentPlayer.ReservedCards[2].BonusColor);
                            currentPlayer.Points += currentPlayer.ReservedCards[2].Points;


                            Card selectedCard = currentPlayer.ReservedCards[2];

                            currentPlayer.ReservedCards.Remove(selectedCard);
                            currentPlayer.ReservedCardsCounter--;

                            return true;
                        }
                        return false;
                }
                return false;
            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
}
