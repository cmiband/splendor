using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SplendorConsole;


namespace SplendorTests
{
    internal class PlayerTests
    {
        [Test]
        public void AddCardToPlayer()
        {
            var player = new Player();

            var detailedPrice = new Resources();
            var card = new Card(1, GemColor.WHITE, 2, "123", detailedPrice);

            player.AddCardToPlayer(card);

            Assert.That(player.hand.Count, Is.EqualTo(1)); 
        }

        [Test]
        public void CanAffordCard_ShouldFalse()
        {
            var player = new Player();
            var detailedPrice = new Resources();
            detailedPrice.gems.Add(GemColor.BLACK, 2);
            var card = new Card(1, GemColor.WHITE, 2, "123", detailedPrice);

            bool test = false;

            Assert.That(test,Is.EqualTo(player.CanAffordCard(card)));
        }

        [Test]
        public void CanAffordCard_ShouldTrue()
        {
            var player = new Player();
            var detailedPrice = new Resources();
            detailedPrice.gems.Add(GemColor.BLACK, 2);
            var card = new Card(1, GemColor.WHITE, 2, "123", detailedPrice);

            player.Resources.gems.Add(GemColor.BLACK, 2);

            bool test = true;

            Assert.That(test, Is.EqualTo(player.CanAffordCard(card)));
        }
        [Test]
        public void CanAffordCardWithGolden_ShouldFalse()
        {
            var player = new Player();
            var detailedPrice = new Resources();
            detailedPrice.gems.Add(GemColor.BLACK, 2);
            var card = new Card(1, GemColor.WHITE, 2, "123", detailedPrice);

            bool test = false;

            Assert.That(test, Is.EqualTo(player.CanAffordCardWithGolden(card)));
        }
        [Test]
        public void CanAffordCardWithGolden_ShouldTrue()
        {
            var player = new Player();
            var detailedPrice = new Resources();
            detailedPrice.gems.Add(GemColor.BLACK, 2);
            var card = new Card(1, GemColor.WHITE, 2, "123", detailedPrice);

            player.Resources.gems.Add(GemColor.BLACK, 1);
            player.Resources.gems.Add(GemColor.GOLDEN, 1);

            bool test = true;

            Assert.That(test, Is.EqualTo(player.CanAffordCardWithGolden(card)));
        }
        [Test]
        public void GetIndex()
        {
            var player = new Player();
            var detailedPrice = new Resources();
            detailedPrice.gems.Add(GemColor.BLACK, 2);
            var card = new Card(1, GemColor.WHITE, 2, "123", detailedPrice);
            var card2 = new Card(2, GemColor.BLACK, 2, "321", detailedPrice);

            Card[] test = new Card[] { card, card2, };

            Assert.That(1, Is.EqualTo(player.GetIndex(card2, test)));
        }
        [Test]
        public void PassTurnTest()
        {
            var player = new Player();

            Assert.DoesNotThrow(() => player.PassTurn());
        }
        [Test]
        public void GetNoble()
        {
            var player = new Player();
            var detailedPrice = new Resources();
            var noble = new Noble(3, detailedPrice);

            player.GetNoble(noble);

            Assert.That(1, Is.EqualTo(player.Nobles.Count));
        }
        [Test]
        public void ReserveCard()
        {
            var player = new Player();
            var detailedPrice = new Resources();
            var card = new Card(1, GemColor.WHITE, 2, "123", detailedPrice);

            player.ReserveCard(card); 

            Assert.That(1, Is.EqualTo(player.ReservedCards.Count));   
        }
        [Test]
        public void TakeTwoTokens()
        {
            var player = new Player();
            var resources = new Resources();
            player.TakeTwoTokens(resources, GemColor.WHITE);

            Assert.That(2, Is.EqualTo(player.Resources.gems[GemColor.WHITE]));    
        }
        [Test]
        public void TakeThreeTokens()
        {
            var player = new Player();
            var resources = new Resources();
            GemColor[] gemColor = new GemColor[] { GemColor.BLUE, GemColor.WHITE, GemColor.RED };
            player.TakeThreeTokens(resources, gemColor);

            Assert.That(1, Is.EqualTo(player.Resources.gems[GemColor.WHITE]));
            Assert.That(1, Is.EqualTo(player.Resources.gems[GemColor.BLUE]));
            Assert.That(1, Is.EqualTo(player.Resources.gems[GemColor.RED]));
        }
        [Test]
        public void RemoveOneToken_ShouldRemove()
        {
            var player = new Player();
            player.Resources.gems[GemColor.WHITE] = 2;

            player.RemoveOneToken(player.Resources, GemColor.WHITE);

            Assert.That(1, Is.EqualTo(player.Resources.gems[GemColor.WHITE]));
        }
        [Test]
        public void PointsCounter()
        {
            var player = new Player();
            var detailedPrice = new Resources();
            detailedPrice.gems.Add(GemColor.BLACK, 2);
            var card = new Card(1, GemColor.WHITE, 2, "123", detailedPrice);
            var noble = new Noble(3, detailedPrice);
            player.hand.Add(card);
            player.Nobles.Add(noble); 

            player.PointsCounter();

            Assert.That(5, Is.EqualTo(player.Points));
        }
    }
}
