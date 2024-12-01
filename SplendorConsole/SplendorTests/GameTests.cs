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
    public class GameTests
    {
        [Test]
        public void TakeTwoSameGems_ShouldReturnFalse_WhenNoSufficientGemsInBank()
        {
            /*var game = new Game();
            var player = new Player();
            var bank = new Bank();

            game.AddResourcesToBank(bank, 4);

            var result = game.TakeTwoSameGems(player);

            Assert.That(result, Is.True);
            */
        }

        [Test]
        public void ShowAvaiableTokens_ShouldReturnTrue_WhenNoTokensAvailable()
        {
            var game = new Game();
            var bank = new Bank();
                   
            game.AddResourcesToBank(bank, 4);

            List<GemColor> result = game.ShowAvaiableTokens();

            Assert.That(0.Equals(result.Count), Is.True);

        }
    }
}
