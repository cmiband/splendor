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
            
            var Bank = new Bank();
            Bank.resources.gems[GemColor.WHITE] = 3;

            
            int remove = 2;
            Bank.TakeOutResources(remove, GemColor.WHITE);

            
            Assert.That(Bank.resources.gems[GemColor.WHITE], Is.EqualTo(1));
        }

        [Test]
        public void AddResources_ShouldAdd()
        {
            
            var bank = new Bank();
            bank.resources.gems[GemColor.BLUE] = 3;

            
            int add = 4;
            bank.AddResources(add, GemColor.BLUE);

            
            Assert.That(bank.resources.gems[GemColor.BLUE], Is.EqualTo(3+add));     
        }

        [Test]
        public void AddGoldenGem_ShouldAdd()
        {
            
            var Bank = new Bank();
            Bank.resources.gems[GemColor.GOLDEN] = 1;

            
            Bank.AddGoldenGem(1);

            
            Assert.That(Bank.resources.gems[GemColor.GOLDEN], Is.EqualTo(2));
        }

        [Test]
        public void CanTakeGoldenGem_ReturnTrue()
        {
            
            var Bank = new Bank();
            Bank.resources.gems[GemColor.GOLDEN] = 1;

            
            bool canTake = Bank.CanTakeGoldenGem();

            
            Assert.That(canTake, Is.EqualTo(true));
        }
        [Test]
        public void CanTakeGoldenGem_ReturnFalse()
        {
            
            var Bank = new Bank();
            Bank.resources.gems[GemColor.GOLDEN] = 0;

            
            bool canTake = Bank.CanTakeGoldenGem();

            
            Assert.That(canTake, Is.EqualTo(false));
        }
    }
}
