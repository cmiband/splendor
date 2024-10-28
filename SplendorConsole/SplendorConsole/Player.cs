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
        private Card[] hand;
        private Card[] reservedCard;
        private Noble[] nobles;
        private int points;
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
            this.resources.gems.Add(color,2);
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

    }
}
