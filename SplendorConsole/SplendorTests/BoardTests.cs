using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SplendorConsole;
namespace SplendorTests
{
    [TestFixture]
    internal class BoardTests
    {

        [Test]
        public void VisibleNobles2_SetAndGet_ShouldWorkCorrectly()
        {
            // Arrange
            var initialNobles = new List<Noble> { new Noble(1, new Resources()) };
            var updatedNobles = new List<Noble> { new Noble(2, new Resources()), new Noble(3, new Resources()) };

            var board = new Board(new List<Card>(), new List<Card>(), new List<Card>(), new List<Card>(), new List<Card>(), new List<Card>(), initialNobles);

            board.VisibleNobles2 = updatedNobles;

            Assert.That(updatedNobles, Is.EqualTo(board.VisibleNobles2));
        }

        [Test]
        public void GetCardIndexInVisibleCards_CardNotFound_ShouldReturnMinusOne()
        {
            // Arrange
            var level1Cards = new List<Card>
            {
                new Card(1, GemColor.WHITE, 3, "Sigma", new Resources()),
                new Card(1, GemColor.BLUE, 4, "Orzech", new Resources()),
                new Card(1, GemColor.GREEN, 2, "Alpha", new Resources())
            };

            var level2Cards = new List<Card>
            {
                new Card(2, GemColor.RED, 5, "Beta", new Resources()),
                new Card(2, GemColor.BLUE, 4, "Orzech", new Resources()),
                new Card(2, GemColor.BLACK, 6, "Gamma", new Resources())
            };

            var level3Cards = new List<Card>
            {
                new Card(3, GemColor.GREEN, 5, "Delta", new Resources()),
                new Card(3, GemColor.RED, 6, "Alpha", new Resources()),
                new Card(3, GemColor.BLACK, 7, "Omega", new Resources())
            };

            var board = new Board(level1Cards, level2Cards, level3Cards, new List<Card>(), new List<Card>(), new List<Card>(), new List<Noble>());

            var cardNotFound = new Card(2, GemColor.RED, 5, "Beta", new Resources());

            var index = board.GetCardIndexInVisibleCards(cardNotFound, 1);

            Assert.That(index, Is.EqualTo(-1), "Card should not be found in level 1.");
        }
        [Test]
        public void GetVisibleCards_ValidLevels_ShouldReturnCorrectList()
        {

            var level1Cards = new List<Card>
            {
                new Card(1, GemColor.WHITE, 3, "Sigma", new Resources()),
                new Card(1, GemColor.BLUE, 4, "Orzech", new Resources()),
                new Card(1, GemColor.GREEN, 2, "Alpha", new Resources())
            };
            var level2Cards = new List<Card>
            {
                new Card(2, GemColor.RED, 5, "Beta", new Resources()),
                new Card(2, GemColor.BLUE, 4, "Orzech", new Resources()),
                new Card(2, GemColor.BLACK, 6, "Gamma", new Resources())
            };
            var level3Cards = new List<Card>
            {
                new Card(3, GemColor.GREEN, 5, "Delta", new Resources()),
                new Card(3, GemColor.RED, 6, "Alpha", new Resources()),
                new Card(3, GemColor.BLACK, 7, "Omega", new Resources())
            };

            var board = new Board(level1Cards, level2Cards, level3Cards, new List<Card>(), new List<Card>(), new List<Card>(), new List<Noble>());

            Assert.That(board.GetVisibleCards(1), Is.EqualTo(level1Cards), "Level 1 cards mismatch");
            Assert.That(board.GetVisibleCards(2), Is.EqualTo(level2Cards), "Level 2 cards mismatch");
            Assert.That(board.GetVisibleCards(3), Is.EqualTo(level3Cards), "Level 3 cards mismatch");
        }

        [Test]
        public void GetVisibleCards_InvalidLevel_ShouldThrowArgumentException()
        {
            var board = new Board(new List<Card>(), new List<Card>(), new List<Card>(), new List<Card>(), new List<Card>(), new List<Card>(), new List<Noble>());

            var exception = Assert.Throws<ArgumentException>(() => board.GetVisibleCards(4));
            Assert.That(exception.Message, Is.EqualTo("Niepoprawny poziom karty"));

            Assert.Throws<ArgumentException>(() => board.GetVisibleCards(null), "Expected an exception for null level");
        }
        [Test]
        public void ReplaceMissingCard_Level1VisibleCards_Test()
        {
            var level1 = new List<Card>();
            var res1 = new Resources();

            var card1 = new Card(1, GemColor.WHITE, 0, "elo1", res1);
            var card2 = new Card(1, GemColor.WHITE, 0, "elo2", res1);
            var card3 = new Card(1, GemColor.WHITE, 0, "elo3", res1);
            var card4 = new Card(1, GemColor.WHITE, 0, "elo4", res1);
            var card5 = new Card(1, GemColor.BLACK, 0, "elo5", res1);
            var card6 = new Card(1, GemColor.BLACK, 0, "elo6", res1);

            level1.Add(card1);
            level1.Add(card2);
            level1.Add(card3);
            level1.Add(card4);

            var level1Deck = new List<Card>();
            level1Deck.Add(card5);
            level1Deck.Add(card6);
            var board = new Board(level1, new List<Card>(), new List<Card>(), level1Deck, new List<Card>(), new List<Card>(), new List<Noble>());

            board.ReplaceMissingCard(1, card4);

            Assert.That(card5.ToString(), Is.EqualTo(board.Level1VisibleCards[3].ToString()));
        }
        [Test]
        public void ReplaceMissingCard_Level2VisibleCards_Test()
        {
            var level2 = new List<Card>();
            var res1 = new Resources();

            var card1 = new Card(2, GemColor.WHITE, 0, "elo1", res1);
            var card2 = new Card(2, GemColor.WHITE, 0, "elo2", res1);
            var card3 = new Card(2, GemColor.WHITE, 0, "elo3", res1);
            var card4 = new Card(2, GemColor.WHITE, 0, "elo4", res1);
            var card5 = new Card(2, GemColor.BLACK, 0, "elo5", res1);
            var card6 = new Card(2, GemColor.BLACK, 0, "elo6", res1);

            level2.Add(card1);
            level2.Add(card2);
            level2.Add(card3);
            level2.Add(card4);

            var level2Deck = new List<Card>();
            level2Deck.Add(card5);
            level2Deck.Add(card6);
            var board = new Board(new List<Card>(),level2, new List<Card>(), new List<Card>(),level2Deck, new List<Card>(), new List<Noble>());

            board.ReplaceMissingCard(2, card4);

            Assert.That(card5.ToString(), Is.EqualTo(board.Level2VisibleCards[3].ToString()));
        }
        [Test]
        public void ReplaceMissingCard_Level3VisibleCards_Test()
        {
            var level3 = new List<Card>();
            var res1 = new Resources();

            var card1 = new Card(2, GemColor.WHITE, 0, "elo1", res1);
            var card2 = new Card(2, GemColor.WHITE, 0, "elo2", res1);
            var card3 = new Card(2, GemColor.WHITE, 0, "elo3", res1);
            var card4 = new Card(2, GemColor.WHITE, 0, "elo4", res1);
            var card5 = new Card(2, GemColor.BLACK, 0, "elo5", res1);
            var card6 = new Card(2, GemColor.BLACK, 0, "elo6", res1);

            level3.Add(card1);
            level3.Add(card2);
            level3.Add(card3);
            level3.Add(card4);

            var level3Deck = new List<Card>();
            level3Deck.Add(card5);
            level3Deck.Add(card6);
            var board = new Board(new List<Card>(), new List<Card>(),level3, new List<Card>(),  new List<Card>(),level3Deck, new List<Noble>());

            board.ReplaceMissingCard(3, card4);

            Assert.That(card5.ToString(), Is.EqualTo(board.Level3VisibleCards[3].ToString()));
        }
        [Test]
        public void ReplaceMissingCard_Level1DeckRemoved_Test()
        {
            var level1 = new List<Card>();
            var res1 = new Resources();

            var card1 = new Card(1, GemColor.WHITE, 0, "elo1", res1);
            var card2 = new Card(1, GemColor.WHITE, 0, "elo2", res1);
            var card3 = new Card(1, GemColor.WHITE, 0, "elo3", res1);
            var card4 = new Card(1, GemColor.WHITE, 0, "elo4", res1);
            var card5 = new Card(1, GemColor.BLACK, 0, "elo5", res1);
            var card6 = new Card(1, GemColor.RED, 0, "elo6", res1);
            var card7 = new Card(1, GemColor.GREEN, 0, "elo7", res1);

            bool checkEqual = false; 

            level1.Add(card1);
            level1.Add(card2);
            level1.Add(card3);
            level1.Add(card4);

            var level1Deck = new List<Card>();
            level1Deck.Add(card5);
            level1Deck.Add(card6);
            level1Deck.Add(card7);
            var board = new Board(level1, new List<Card>(), new List<Card>(), level1Deck, new List<Card>(), new List<Card>(), new List<Noble>());

            board.ReplaceMissingCard(1, card4);
            foreach (Card card in board.Level1Deck)
            {
                if (card5.ToString() == card.ToString())
                {
                    checkEqual = true;
                }
            }

            Assert.That(false, Is.EqualTo(checkEqual));
        }
        [Test]
        public void ReplaceMissingCard_Level2DeckRemoved_Test()
        {
            var level1 = new List<Card>();
            var res1 = new Resources();

            var card1 = new Card(1, GemColor.WHITE, 0, "elo1", res1);
            var card2 = new Card(1, GemColor.WHITE, 0, "elo2", res1);
            var card3 = new Card(1, GemColor.WHITE, 0, "elo3", res1);
            var card4 = new Card(1, GemColor.WHITE, 0, "elo4", res1);
            var card5 = new Card(1, GemColor.BLACK, 0, "elo5", res1);
            var card6 = new Card(1, GemColor.RED, 0, "elo6", res1);
            var card7 = new Card(1, GemColor.GREEN, 0, "elo7", res1);

            bool checkEqual = false;

            level1.Add(card1);
            level1.Add(card2);
            level1.Add(card3);
            level1.Add(card4);

            var level2Deck = new List<Card>();
            level2Deck.Add(card5);
            level2Deck.Add(card6);
            level2Deck.Add(card7);
            var board = new Board(level1, new List<Card>(), new List<Card>(), new List<Card>(),level2Deck, new List<Card>(), new List<Noble>());

            board.ReplaceMissingCard(2, card4);
            foreach (Card card in board.Level2Deck)
            {
                if (card5.ToString() == card.ToString())
                {
                    checkEqual = true;
                }
            }

            Assert.That(false, Is.EqualTo(checkEqual));
        }
        [Test]
        public void ReplaceMissingCard_Level3DeckRemoved_Test()
        {
            var level1 = new List<Card>();
            var res1 = new Resources();

            var card1 = new Card(1, GemColor.WHITE, 0, "elo1", res1);
            var card2 = new Card(1, GemColor.WHITE, 0, "elo2", res1);
            var card3 = new Card(1, GemColor.WHITE, 0, "elo3", res1);
            var card4 = new Card(1, GemColor.WHITE, 0, "elo4", res1);
            var card5 = new Card(1, GemColor.BLACK, 0, "elo5", res1);
            var card6 = new Card(1, GemColor.RED, 0, "elo6", res1);
            var card7 = new Card(1, GemColor.GREEN, 0, "elo7", res1);

            bool checkEqual = false;

            level1.Add(card1);
            level1.Add(card2);
            level1.Add(card3);
            level1.Add(card4);

            var level3Deck = new List<Card>();
            level3Deck.Add(card5);
            level3Deck.Add(card6);
            level3Deck.Add(card7);
            var board = new Board(level1, new List<Card>(), new List<Card>(), new List<Card>(), new List<Card>(),level3Deck, new List<Noble>());

            board.ReplaceMissingCard(3, card4);
            foreach (Card card in board.Level3Deck)
            {
                if (card5.ToString() == card.ToString())
                {
                    checkEqual = true;
                }
            }

            Assert.That(false, Is.EqualTo(checkEqual));
        }
        
    }        
}
