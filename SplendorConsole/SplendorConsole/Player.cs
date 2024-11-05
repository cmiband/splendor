using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplendorConsole
{
    internal class Player
    {
        private Resources resources = new Resources();
        private Resources bonusResources;
        public List<Card> hand = new List<Card>();
        private Card[] reservedCard;
        private List<Noble> nobles = new List<Noble>();
        private int points;
        public int Points
        {
            get => points;
        }
        public void BuyCard(Card card)
        {
            throw new NotImplementedException();
        }
        public void ReserveCard(Card card)
        {
            throw new NotImplementedException();
        }
        public void TakeTwoTokens(Resources resources, GemColor color)
        {

            if (this.resources.gems.ContainsKey(color))
            {
                this.resources.gems[color] += 2;

            }
            else this.resources.gems.Add(color,2);
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
        public Noble GetNoble(Noble noble)
        {
            throw new NotImplementedException();
        }
        public bool CanGetNoble(Noble noble)
        {
            throw new NotImplementedException();
        }
        public bool CanAffordCard(Card card)
        {
            throw new NotImplementedException();
        }
        public void PassTurn()
        {
            throw new NotImplementedException();
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
    }
}
