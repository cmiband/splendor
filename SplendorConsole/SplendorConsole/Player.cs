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
        public void TakeTwoTokens(Resources resources)
        {
            throw new NotImplementedException();
        }
        public void TakeThreeTokens(Resources resources)
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
