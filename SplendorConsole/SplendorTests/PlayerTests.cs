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
            var _player = new Player();

            var _detailedPrice = new Resources();
            var _card = new Card(1, GemColor.WHITE, 2, "123", _detailedPrice);

            _player.AddCardToPlayer(_card);

            Assert.That(_player.hand.Count, Is.EqualTo(1)); 
        }

        [Test]
        public void CanAffordCard_ShouldFalse()
        {
            var _player = new Player();
            var _detailedPrice = new Resources();
            _detailedPrice.gems.Add(GemColor.BLACK, 2);
            var _card = new Card(1, GemColor.WHITE, 2, "123", _detailedPrice);

            bool test = false;

            Assert.That(test,Is.EqualTo(_player.CanAffordCard(_card)));
        }

        [Test]
        public void CanAffordCard_ShouldTrue()
        {
            var _player = new Player();
            var _detailedPrice = new Resources();
            _detailedPrice.gems.Add(GemColor.BLACK, 2);
            var _card = new Card(1, GemColor.WHITE, 2, "123", _detailedPrice);

            _player.Resources.gems.Add(GemColor.BLACK, 2);

            bool test = true;

            Assert.That(test, Is.EqualTo(_player.CanAffordCard(_card)));
        }
        [Test]
        public void CanAffordCardWithGolden_ShouldFalse()
        {
            var _player = new Player();
            var _detailedPrice = new Resources();
            _detailedPrice.gems.Add(GemColor.BLACK, 2);
            var _card = new Card(1, GemColor.WHITE, 2, "123", _detailedPrice);

            bool test = false;

            Assert.That(test, Is.EqualTo(_player.CanAffordCardWithGolden(_card)));
        }
        [Test]
        public void CanAffordCardWithGolden_ShouldTrue()
        {
            var _player = new Player();
            var _detailedPrice = new Resources();
            _detailedPrice.gems.Add(GemColor.BLACK, 2);
            var _card = new Card(1, GemColor.WHITE, 2, "123", _detailedPrice);

            _player.Resources.gems.Add(GemColor.BLACK, 1);
            _player.Resources.gems.Add(GemColor.GOLDEN, 1);

            bool test = true;

            Assert.That(test, Is.EqualTo(_player.CanAffordCardWithGolden(_card)));
        }
        [Test]
        public void GetIndex()
        {
            var _player = new Player();
            var _detailedPrice = new Resources();
            _detailedPrice.gems.Add(GemColor.BLACK, 2);
            var _card = new Card(1, GemColor.WHITE, 2, "123", _detailedPrice);
            var _card2 = new Card(2, GemColor.BLACK, 2, "321", _detailedPrice);

            Card[] test = new Card[] { _card, _card2, };

            Assert.That(1, Is.EqualTo(_player.GetIndex(_card2, test)));
        }
        [Test]
        public void PassTurn()
        {
            var _player = new Player();

            var exception = Assert.Throws<NotImplementedException>(() => _player.PassTurn());

            var test = new NotImplementedException();

            Assert.That(exception.Message, Is.EqualTo("The method or operation is not implemented."));
        }
        [Test]
        public void GetNoble()
        {
            var _player = new Player();
            var _detailedPrice = new Resources();
            var _noble = new Noble(3, _detailedPrice);

            _player.GetNoble(_noble);

            Assert.That(1, Is.EqualTo(_player.Nobles.Count));
        }
        [Test]
        public void ReserveCard()
        {
            var _player = new Player();
            var _detailedPrice = new Resources();
            var _card = new Card(1, GemColor.WHITE, 2, "123", _detailedPrice);

            _player.ReserveCard(_card); 

            Assert.That(1, Is.EqualTo(_player.ReservedCards.Count));   
        }
        [Test]
        public void TakeTwoTokens()
        {
            var _player = new Player();
            var _resources = new Resources();
            _player.TakeTwoTokens(_resources, GemColor.WHITE);

            Assert.That(2, Is.EqualTo(_player.Resources.gems[GemColor.WHITE]));    
        }
        [Test]
        public void TakeThreeTokens()
        {
            var _player = new Player();
            var _resources = new Resources();
            GemColor[] gemColor = new GemColor[] { GemColor.BLUE, GemColor.WHITE, GemColor.RED };
            _player.TakeThreeTokens(_resources, gemColor);

            Assert.That(1, Is.EqualTo(_player.Resources.gems[GemColor.WHITE]));
            Assert.That(1, Is.EqualTo(_player.Resources.gems[GemColor.BLUE]));
            Assert.That(1, Is.EqualTo(_player.Resources.gems[GemColor.RED]));
        }
        [Test]
        public void RemoveOneToken_ShouldRemove()
        {
            var _player = new Player();
            _player.Resources.gems[GemColor.WHITE] = 2;

            _player.RemoveOneToken(_player.Resources, GemColor.WHITE);

            Assert.That(1, Is.EqualTo(_player.Resources.gems[GemColor.WHITE]));
        }
        [Test]
        public void PointsCounter()
        {
            var _player = new Player();
            var _detailedPrice = new Resources();
            _detailedPrice.gems.Add(GemColor.BLACK, 2);
            var _card = new Card(1, GemColor.WHITE, 2, "123", _detailedPrice);
            var _noble = new Noble(3, _detailedPrice);
            _player.hand.Add(_card);
            _player.Nobles.Add(_noble); 

            _player.PointsCounter();

            Assert.That(5, Is.EqualTo(_player.Points));
        }
    }
}
