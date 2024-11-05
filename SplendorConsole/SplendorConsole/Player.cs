using DocumentFormat.OpenXml.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SplendorConsole
{
    internal class Player
    {
        private Resources resources = new Resources();
        private Resources bonusResources = new Resources();
        private List<Card> hand;
        private Card[] reservedCard;
        private Noble[] nobles;
        private int points;
        public Player()
        {
            hand = new List<Card>();
        }
        public void BuyCardAction(Board board, Bank bank)
        {
            Console.WriteLine("Wybierz poziom karty do zakupu (1, 2 lub 3):");
            int level;
            while (!int.TryParse(Console.ReadLine(), out level) || level < 1 || level > 3)
            {
                Console.WriteLine("Niepoprawny poziom. Wprowadź 1, 2 lub 3:");
            }

            List<Card> visibleCards = board.GetVisibleCards(level);
            Console.WriteLine("Wybierz numer karty do zakupu:");
            for (int i = 0; i < visibleCards.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {visibleCards[i]}");
            }

            int cardIndex;
            while (!int.TryParse(Console.ReadLine(), out cardIndex) || cardIndex < 1 || cardIndex > visibleCards.Count)
            {
                Console.WriteLine("Niepoprawny wybór. Wprowadź numer odpowiadający wybranej karcie:");
            }

            Card selectedCard = visibleCards[cardIndex - 1];

            var success = BuyCard(board, selectedCard, bank);
            if (success)
            {
                Console.WriteLine("Karta została pomyślnie zakupiona!");
                board.ReplaceMissingCard(level, cardIndex - 1);
            }
            else
                Console.WriteLine("Operacja kupowania karty nie powiodła się.");
            
        }

        public bool BuyCard(Board board, Card card, Bank bank)
        {
            Console.WriteLine("Czy chcesz użyć złotego żetonu aby zapłacić za kartę?");
            Console.WriteLine("1 - Tak");
            Console.WriteLine("2 - Nie");
            var choiceForGolden = WantToSpendGoldCoin();
            GemColor colorOfChoice = GemColor.NONE;
            if (choiceForGolden)
            {
                colorOfChoice = ColorToBePaidWithGolden();
            }

            if (!CanAffordCardWithGolden(card) && !CanAffordCard(card))
            {
                Console.WriteLine("Nie stać cię na tę kartę.");
                return false;
            }

            int cardToBuyLevel = card.Level;
            int indexInVisibleCards = board.GetCardIndexInVisibleCards(card, cardToBuyLevel);

            if (indexInVisibleCards == -1)
            {
                Console.WriteLine("Karta nie jest widoczna na stole.");
                return false;
            }

            var cardPrice = card.DetailedPrice.gems;
            foreach (var colorOnCard in cardPrice)
            {
                GemColor color = colorOnCard.Key;
                int requiredAmount = colorOnCard.Value;

                int bonusAmount = this.bonusResources.gems.TryGetValue(color, out var bonus) ? bonus : 0;
                if (bonusAmount > 0)
                {
                    int amountToUse = Math.Min(bonusAmount, requiredAmount);
                    this.bonusResources.gems[color] -= amountToUse;
                    requiredAmount -= amountToUse;
                }
                if (requiredAmount > 0)
                {
                    int playerAmount = this.resources.gems.TryGetValue(color, out var player) ? player : 0;
                    if (playerAmount >= requiredAmount)
                    {
                        this.resources.gems[color] -= requiredAmount;
                    }
                    else
                    {
                        int deficit = requiredAmount - playerAmount;
                        if (choiceForGolden && colorOfChoice == color)
                        {
                            int goldenTokens = this.resources.gems.TryGetValue(GemColor.GOLDEN, out var golden) ? golden : 0;
                            if (goldenTokens >= deficit)
                            {
                                this.resources.gems[GemColor.GOLDEN] -= deficit;
                            }
                            else
                            {
                                Console.WriteLine("Nie masz wystarczającej liczby złotych żetonów.");
                                return false;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Nie masz wystarczającej ilości żetonów koloru {color}.");
                            return false;
                        }
                    }
                }
            }

            AddCardToPlayer(card);
            this.bonusResources.AddResource(card.BonusColor);
            this.points += card.Points;

            board.ReplaceMissingCard(cardToBuyLevel, indexInVisibleCards);
            Console.WriteLine($"Karta {card.ToString()} została zakupiona!");
            return true;
        }

        private void AddCardToPlayer(Card card)
        {
            hand.Add(card);
            Console.WriteLine($"Dodano kartę z następującą liczbą punktów: {card.Points}");
        }

        public bool CanAffordCard(Card card)
        {
            var cardPrice = card.DetailedPrice.gems;
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
        public bool CanAffordCardWithGolden(Card card)
        {
            var cardPrice = card.DetailedPrice.gems;
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
        public int GetIndex(Card card, Card[] cardsOnTable)
        {
            for (int i = 0; i < cardsOnTable.Length; i++)
            {
                if (card.Equals(cardsOnTable[i]))
                    return i;
            }
            return -1;
        }
        public bool WantToSpendGoldCoin()
        {
            int wantTo;
            while (true)
            {
                wantTo = Convert.ToInt32(Console.ReadLine());
                if (wantTo == 2)
                    return false;
                else if (wantTo == 1)
                    return true;
                else
                    Console.WriteLine("Podano zły klawisz. Podaj 1 lub 2");
            }
        }
        public GemColor ColorToBePaidWithGolden()
        {
            Console.WriteLine("Podaj kolor żetonu, zamiast którego użyjesz złotego żetonu:");
            Console.WriteLine("Opcje do wyboru: ");
            foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
            {
                if (color != GemColor.GOLDEN)
                {
                    Console.WriteLine($"- {color}");
                }
            }
            int input;
            while (true)
            {
                Console.Write("Wprowadź numer koloru: ");
                if (int.TryParse(Console.ReadLine(), out input) && input > 0 && input <= Enum.GetValues(typeof(GemColor)).Length - 1)
                    return (GemColor)(input - 1);
                else
                    Console.WriteLine("Niepoprawny wybór. Wprowadź numer odpowiadający dostępnym kolorom.");
            }
        }
        public void PassTurn()
        {
            throw new NotImplementedException();
        }
        public Noble GetNoble(Noble noble)
        {
            throw new NotImplementedException();
        }
        public bool CanGetNoble(Noble noble)
        {
            throw new NotImplementedException();
        }
        public void ReserveCard(Card card)
        {
            throw new NotImplementedException();
        }
        public void TakeThreeTokens(Resources resources, GemColor[] colors)
        {
            throw new NotImplementedException();
        }
        public void TakeTwoTokens(Resources resources, GemColor color)
        {
            throw new NotImplementedException();
        }
    }
}
