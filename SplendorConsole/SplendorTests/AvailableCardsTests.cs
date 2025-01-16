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
    class AvailableCardsTests
    {
        [Test]
        public void AddCard_Level1Card_ShouldAdd()
        {
            
            var availableCards = new AvailableCards();
            var detailedPrice = new Resources();
            detailedPrice.gems[GemColor.WHITE] = 1;
            var card = new Card(1, GemColor.WHITE, 2, "1", detailedPrice);

            
            availableCards.AddCard(card);

            
            Assert.That(availableCards.level1Cards.Count(), Is.EqualTo(1));
        }
        [Test]
        public void AddCard_Level2Card_ShouldAdd()
        {
            
            var availableCards = new AvailableCards();
            var detailedPrice = new Resources();
            detailedPrice.gems[GemColor.WHITE] = 1;
            var card = new Card(2, GemColor.WHITE, 2, "1", detailedPrice);

            
            availableCards.AddCard(card);

            
            Assert.That(availableCards.level2Cards.Count(), Is.EqualTo(1));
        }
        [Test]
        public void AddCard_Level3Card_ShouldAdd()
        {
            
            var availableCards = new AvailableCards();
            var detailedPrice = new Resources();
            detailedPrice.gems[GemColor.WHITE] = 1;
            var card = new Card(3, GemColor.WHITE, 2, "1", detailedPrice);

            
            availableCards.AddCard(card);

            
            Assert.That(availableCards.level3Cards.Count(), Is.EqualTo(1));
        }
        [Test]
        public void AddCard_ShouldNotAdd()
        {
            
            var availableCards = new AvailableCards();
            var detailedPrice = new Resources();
            detailedPrice.gems[GemColor.WHITE] = 1;
            var card = new Card(4, GemColor.WHITE, 2, "1", detailedPrice);

            
            var exception = Assert.Throws<ArgumentException>(() => availableCards.AddCard(card));

            
            Assert.That(exception.Message, Is.EqualTo("Wrong card level!"));
        }
        [Test]
        public void LoadCardsFromExcel_Test()
        {
            
            var availableCards = new AvailableCards();

            
            availableCards.LoadCardsFromExcel("LoadCardsFromExcelTest.xlsx");
            var card = availableCards.level3Cards[0].ToString();
            string message = "Karta koloru: WHITE,\t cena: WHITE: 1, BLUE: 2, GREEN: 3, RED: 4, BLACK: 5,\t dodająca 1 punktów.";

            
            Assert.That(card, Is.EqualTo(message));
        }
    }
}
