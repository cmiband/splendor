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
        public bool BUYING_RESERVED_CARD = true;
        public bool NOT_BUYING_RESERVED_CARD = false;

        private Resources resources = new Resources();
        private Resources bonusResources = new Resources();
        public Resources BonusResources
        {
            get { return bonusResources; }
            set { bonusResources = value; }

        }
        public List<Card> hand;
        private List<Card> reservedCards = new List<Card>();
        private int reservedCardsCounter = 0;
        private List<Noble> nobles = new List<Noble>();
        public List<Noble> Nobles
        {
            get => nobles;
            set => nobles = value;
        }
        private int points;
        public int Points { get => points; set => points = value; }
        public Player()
        {
            hand = new List<Card>();
        }
        public int ReservedCardsCounter
        {
            set => reservedCardsCounter = value;
            get { return reservedCardsCounter; }
        }

        public Resources Resources { get => resources; }
        public bool BuyCardAction(Board board, Bank bank)
        {
            Console.WriteLine("Chcesz kupić nową kartę czy kupić zarezerwowaną?");
            Console.WriteLine("[1] Nowa");
            Console.WriteLine("[2] Zarerwowana");

            int opChoice;
            while (!int.TryParse(Console.ReadLine(), out opChoice) || opChoice < 1 || opChoice > 2)
            {
                Console.WriteLine("Niepoprawny poziom. Wprowadź 1 lub 2");
            }

            if(opChoice == 1)
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

                var success = BuyCard(board, selectedCard, bank, NOT_BUYING_RESERVED_CARD);
                if (success)
                {
                    Console.WriteLine("Karta została pomyślnie zakupiona!");
                    board.ReplaceMissingCard(level, selectedCard);
                    return true;
                }
                else
                {
                    Console.WriteLine("Operacja kupowania karty nie powiodła się.");
                    return false;
                }
            } 
            else
            {
                return this.HandleBuyReservedCard(board, bank);
            }
        }

        public bool HandleBuyReservedCard(Board board, Bank bank)
        {
            if(this.reservedCards.Count == 0)
            {
                Console.WriteLine("Nie masz zarezerwowanych kart.");
                return false;
            }

            Console.WriteLine("Wybierz zarezerwowaną kartę, którą chcesz kupić: ");
            for(int i = 0; i<this.reservedCards.Count; i++)
            {
                Console.WriteLine($"[{i+1}] {this.reservedCards[i]}");
            }
            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > this.reservedCards.Count)
            {
                Console.WriteLine("Niepoprawna karta.");
            }

            Card selectedCard = this.reservedCards[choice - 1];
            this.reservedCards.Remove(selectedCard);

            return this.BuyCard(board, selectedCard, bank, BUYING_RESERVED_CARD);
        }

        public bool BuyCard(Board board, Card card, Bank bank, bool isBuyingReservedCard)
        {
            Console.WriteLine("Czy chcesz użyć złotego żetonu aby zapłacić za kartę?");
            Console.WriteLine("1 - Tak");
            Console.WriteLine("2 - Nie");
            var choiceForGolden = WantToSpendGoldCoin();
            GemColor colorReplacedWithGolden = GemColor.NONE;
            GemColor colorOfChoice = GemColor.NONE;
            if (choiceForGolden)
            {
                colorOfChoice = ColorToBePaidWithGolden();
                colorReplacedWithGolden = colorOfChoice;
            }

            if (!CanAffordCardWithGolden(card) && !CanAffordCard(card))
            {
                Console.WriteLine("Nie stać cię na tę kartę.");
                return false;
            }

            int cardToBuyLevel = card.Level;
            int indexInVisibleCards = board.GetCardIndexInVisibleCards(card, cardToBuyLevel);

            if (indexInVisibleCards == -1 && !isBuyingReservedCard)
            {
                Console.WriteLine("Karta nie jest widoczna na stole.");
                return false;
            }

            var cardPrice = card.DetailedPrice.gems;
            bool goldenWasUsed = false;
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
                                goldenWasUsed = true;
                                bank.AddGoldenGem();
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

            board.ReplaceMissingCard(cardToBuyLevel, card);
            this.RefillBankResources(bank, card, colorReplacedWithGolden);
            if(isBuyingReservedCard && !goldenWasUsed)
            {
                this.resources.gems[GemColor.GOLDEN]--;
                bank.AddGoldenGem();
            }
            Console.WriteLine($"Karta {card} została zakupiona!");
            return true;
        }

        private void AddCardToPlayer(Card card)
        {
            hand.Add(card);
            Console.WriteLine($"Dodano kartę z następującą liczbą punktów: {card.Points}");
        }

        private void RefillBankResources(Bank bank, Card card, GemColor colorReplacedWithGolden)
        {
            foreach(var x in card.DetailedPrice.gems)
            {
                int amountToReplace = x.Value;
                if(x.Key == colorReplacedWithGolden)
                {
                    amountToReplace--;
                }
                
                bank.AddResources(amountToReplace, x.Key);
            }
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

        public void GetNoble(Noble noble)
        {
            for(int i=0;i<nobles.Count;i++)
            {
                if (nobles[i] == null)
                    nobles[i] = noble;
            }

            Noble[] CopiedVisibleNobles = new Noble[Board.VisibleNobles.Length - 1];
            int j = 0;
            for(int i=0;i<Board.VisibleNobles.Length;i++)
            {
                if (Board.VisibleNobles[i] != noble)
                {
                    CopiedVisibleNobles[j] = Board.VisibleNobles[i];
                    j++;
                }
            }

            Array.Copy(CopiedVisibleNobles, Board.VisibleNobles, CopiedVisibleNobles.Length);


            points += noble.Points;
        }

        public bool CanGetMultipleNobles()
        {
            int counter = 0;
            foreach (Noble noble in Board.VisibleNobles)
            {
                if (CanGetNoble(noble))
                    counter++;
            }

            if (counter > 1)
                return true;
            else
                return false;
        }

        public bool CanGetNoble(Noble noble)
        {
            foreach (var requiredBonus in noble.RequiredBonuses)
            {
                GemColor color = requiredBonus.Key;
                int requiredAmount = requiredBonus.Value;

                if (noble.RequiredBonuses.gems[color] <= bonusResources.gems[color])
                {
                    continue;
                }
                else
                    return false;
            }
            return true;
        }
        public void ReserveCard(Card card)
        {
            this.reservedCards.Add(card);
        }
        public void TakeTwoTokens(Resources resources, GemColor color)
        {

            if (this.resources.gems.ContainsKey(color))
            {
                this.resources.gems[color] += 2;

            }
            else this.resources.gems.Add(color, 2);
        }
        public void TakeThreeTokens(Resources resources, GemColor[] colors)
        {
            for (int i = 0; i < 3; i++)
            {
                if (this.resources.gems.ContainsKey(colors[i]))
                {
                    this.resources.gems[colors[i]] += 1;

                }
                else this.resources.gems.Add(colors[i], 1);
            }
        }
        public void PointsCounter()
        {
            this.points = 0;
            foreach(Card card in this.hand)
            {
                points += card.Points;
            }
            foreach(Noble noble in this.nobles)
            {
                points += noble.Points;
            }
        }

        public string handToString()
        {
            return string.Join("  ", hand);
        }

        public string nobleToString()
        {
            return string.Join(" ", nobles);
        }
    }
}
