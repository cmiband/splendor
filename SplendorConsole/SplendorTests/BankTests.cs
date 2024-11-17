using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using NUnit.Framework;
using SplendorConsole;

namespace SplendorTests
{
    [TestFixture]
    class BankTests
    {
        [Test]
        public void TakeOutResources_ShouldTakeOut()
        {
            
            var _Bank = new Bank();
            _Bank.resources.gems[GemColor.WHITE] = 3;

            
            int remove = 2;
            _Bank.TakeOutResources(remove, GemColor.WHITE);

            
            Assert.That(_Bank.resources.gems[GemColor.WHITE], Is.EqualTo(1));
        }

        [Test]
        public void AddResources_ShouldAdd()
        {
            
            var _Bank = new Bank();
            _Bank.resources.gems[GemColor.BLUE] = 3;

            
            int add = 4;
            _Bank.AddResources(add, GemColor.BLUE);

            
            Assert.That(_Bank.resources.gems[GemColor.BLUE], Is.EqualTo(3+add));     
        }

        [Test]
        public void AddGoldenGem_ShouldAdd()
        {
            
            var _Bank = new Bank();
            _Bank.resources.gems[GemColor.GOLDEN] = 1;

            
            _Bank.AddGoldenGem();

            
            Assert.That(_Bank.resources.gems[GemColor.GOLDEN], Is.EqualTo(2));
        }

        [Test]
        public void CanTakeGoldenGem_ReturnTrue()
        {
            
            var _Bank = new Bank();
            _Bank.resources.gems[GemColor.GOLDEN] = 1;

            
            bool canTake = _Bank.CanTakeGoldenGem();

            
            Assert.That(canTake, Is.EqualTo(true));
        }
        [Test]
        public void CanTakeGoldenGem_ReturnFalse()
        {
            
            var _Bank = new Bank();
            _Bank.resources.gems[GemColor.GOLDEN] = 0;

            
            bool canTake = _Bank.CanTakeGoldenGem();

            
            Assert.That(canTake, Is.EqualTo(false));
        }
    }
}
