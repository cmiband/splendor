using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplendorConsole
{
    internal class Player
    {
        private Resources resources;
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
            if (resources.gems[color] >= 4)
            {
                this.resources.gems.Add(color, 2);
            }
            Console.WriteLine("Byku wybierz inny kolor");
        }
        public void TakeThreeTokens(Resources resources, GemColor[] colors)
        {
            for (int i = 0; i < 3; i++)
            {
                if (resources.gems[colors[i]] > 0) this.resources.gems.Add(colors[i], 1);
            }
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
