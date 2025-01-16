using System;
using System.Collections.Generic;
using System.Linq;
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

            Assert.That(updatedNobles,Is.EqualTo(board.VisibleNobles2)); 
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

    }
}
