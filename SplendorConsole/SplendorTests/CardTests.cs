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
    internal class CardTests
    {
       
        [Test]
        public void Constructor_ShouldInitializeProperties()
        {
            
            var price = new Resources();
            price.gems[GemColor.WHITE] = 1;
            price.gems[GemColor.BLUE] = 2;
            var level = 1;
            var bonusColor = GemColor.RED;
            var points = 3;
            var illustration = "image.png";

            var card = new Card(level, bonusColor, points, illustration, price);

           
            Assert.That(card.Level, Is.EqualTo(level));
            Assert.That(card.BonusColor, Is.EqualTo(bonusColor));
            Assert.That(card.Points, Is.EqualTo(points));
            Assert.That(card.DetailedPrice, Is.EqualTo(price));
        }
        [Test]
        public void Equals_ShouldReturnTrueForIdenticalCards()
        {
            
            var price = new Resources();
            price.gems[GemColor.WHITE] = 1;
            price.gems[GemColor.BLUE] = 2;

            var card1 = new Card(1, GemColor.RED, 3, "image.png", price);
            var card2 = new Card(1, GemColor.RED, 3, "image.png", price);

            Assert.That(card1.Equals(card2), Is.True);
        }

        [Test]
        public void Equals_ShouldReturnFalseForDifferentCards()
        {
            
            var price1 = new Resources();
            price1.gems[GemColor.WHITE] = 1;
            var card1 = new Card(1, GemColor.RED, 3, "image.png", price1);

            var price2 = new Resources();
            price2.gems[GemColor.WHITE] = 2;
            var card2 = new Card(1, GemColor.RED, 3, "image.png", price2);

            Assert.That(card1.Equals(card2), Is.False);
            
        }

        [Test]
        public void GetHashCode_ShouldReturnSameHashForIdenticalCards()
        {
 
            var price = new Resources();
            price.gems[GemColor.WHITE] = 1;
            price.gems[GemColor.BLUE] = 2;

            var card1 = new Card(1, GemColor.RED, 3, "image.png", price);
            var card2 = new Card(1, GemColor.RED, 3, "image.png", price);

            Assert.That(card1.GetHashCode(), Is.EqualTo(card2.GetHashCode()));
        }

        [Test]
        public void ToString_ShouldReturnFormattedString()
        {
            var price = new Resources();
            price.gems[GemColor.WHITE] = 2;
            price.gems[GemColor.BLUE] = 1;
            var card = new Card(1, GemColor.RED, 5, "image.png", price);

            var result = card.ToString();

            Assert.That(result, Is.EqualTo("Karta koloru: RED,\t cena: " + price.ToString() + ",\t dodająca 5 punktów."));
        }
    }
}
