using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplendorConsole
{
    internal class Board
    {
        private Noble[] nobles;
        private Noble[] visibleNobles;
        public Noble[] VisibleNobles
        {
            get=>visibleNobles;
            set=>visibleNobles = value;
        }
        private Stack<Card> level1Deck = new Stack<Card>();
        private Stack<Card> level2Deck = new Stack<Card>();
        private Stack<Card> level3Deck = new Stack<Card>();
        private Card[] level1VisibleCards;
        private Card[] level2VisibleCards;
        private Card[] level3VisibleCards;
        public void ReplaceMissingCard(int level)
        {
            throw new NotImplementedException();
        }
    }
}
