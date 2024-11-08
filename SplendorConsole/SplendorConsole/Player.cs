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
        public List<Card> ReservedCards { get => reservedCards; set => reservedCards = value; }
        public void AddCardToPlayer(Card card)
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
