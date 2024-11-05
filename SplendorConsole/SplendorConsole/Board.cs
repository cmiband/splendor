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
        private static Noble[] visibleNobles;
        public static Noble[] VisibleNobles
        {
            get=>visibleNobles;
            set=>visibleNobles = value;
        }
        private Stack<Card> level1Deck = new Stack<Card>();
        private Stack<Card> level2Deck = new Stack<Card>();
        private Stack<Card> level3Deck = new Stack<Card>();
        private List<Card> level1VisibleCards;
        private List<Card> level2VisibleCards;
        private List<Card> level3VisibleCards;

        public Board(List<Card> level1VisibleCards, List<Card> level2VisibleCards, List<Card> level3VisibleCards)
        {
            this.level1VisibleCards = level1VisibleCards;
            this.level2VisibleCards = level2VisibleCards;
            this.level3VisibleCards = level3VisibleCards;
        }

        public List<Card> Level1VisibleCards { get => level1VisibleCards; }
        public List<Card> Level2VisibleCards { get => level2VisibleCards;}
        public List<Card> Level3VisibleCards { get => level3VisibleCards;}
        public int GetCardIndexInVisibleCards(Card card, int level)
        {
            List<Card> visibleCards = GetVisibleCards(level);
            for (int i = 0; i < visibleCards.Count; i++)
            {
                if (card.Equals(visibleCards[i]))
                    return i;
            }
            return -1;
        }
        public List<Card> GetVisibleCards(int? level)
        {
            return level switch
            {
                1 => Level1VisibleCards,
                2 => Level2VisibleCards,
                3 => Level3VisibleCards,
                _ => throw new ArgumentException("Niepoprawny poziom karty")
            };
        }
        public void ReplaceMissingCard(int level)
        {
            throw new NotImplementedException();
        }
    }
}
