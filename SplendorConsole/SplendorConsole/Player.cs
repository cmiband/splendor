using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SplendorConsole
{
    public class Player
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
            return;
        }
        public void GetNoble(Noble noble)
        {
            nobles.Add(noble);                 
            points += noble.Points;
        }     
        public void ReserveCard(Card card)
        {
            this.reservedCards.Add(card);
            reservedCardsCounter++;
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
        
        public void RemoveOneToken(Resources resources, GemColor color)
        {
            if (this.resources.gems.ContainsKey(color) && resources.gems[color] > 1)
            {
                this.resources.gems[color] -= 1;
            }
            else
            {
                this.resources.gems.Remove(color);
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
        public int[] ToArray()
        {
            int[] output = new int[45];
            output[0] = points;
            output[1] = bonusResources.gems.ContainsKey(GemColor.WHITE) ? bonusResources.gems[GemColor.WHITE] : 0;
            output[2] = bonusResources.gems.ContainsKey(GemColor.BLUE) ? bonusResources.gems[GemColor.BLUE] : 0;
            output[3] = bonusResources.gems.ContainsKey(GemColor.GREEN) ? bonusResources.gems[GemColor.GREEN] : 0;
            output[4] = bonusResources.gems.ContainsKey(GemColor.RED) ? bonusResources.gems[GemColor.RED] : 0;
            output[5] = bonusResources.gems.ContainsKey(GemColor.BLACK) ? bonusResources.gems[GemColor.BLACK] : 0;
            output[6] = resources.gems.ContainsKey(GemColor.WHITE) ?  resources.gems[GemColor.WHITE] : 0;
            output[7] = resources.gems.ContainsKey(GemColor.BLUE) ? resources.gems[GemColor.BLUE] : 0;
            output[8] = resources.gems.ContainsKey(GemColor.GREEN) ? resources.gems[GemColor.GREEN] : 0;
            output[9] = resources.gems.ContainsKey(GemColor.RED) ? resources.gems[GemColor.RED] : 0;
            output[10] = resources.gems.ContainsKey(GemColor.BLACK) ? resources.gems[GemColor.BLACK] : 0;
            int pointer = 11;
            foreach (var item in reservedCards)
            {
                foreach(var parameter in item.ToArray())
                {
                    output[pointer++] = parameter;
                }
            }
            for(int i=0; i<3-reservedCardsCounter; i++)
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
    }
}
