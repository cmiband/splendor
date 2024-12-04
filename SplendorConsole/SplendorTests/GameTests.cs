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
        public void ShowAvaiableTokens_ShouldReturnTrue_WhenNoTokensAvailable()
        {
            var game = new Game();
            var bank = new Bank();

            game.AddResourcesToBank(bank, 4);

            List<GemColor> result = game.ShowAvaiableTokens();

            Assert.That(0.Equals(result.Count), Is.True);
           

        }
        [Test]
        public void NumberOfPlayerTokens_ShouldReturnTrue()
        {
            var game = new Game();
            var player = new Player();

            player.Resources.gems.Add(GemColor.WHITE, 3);
            player.Resources.gems.Add(GemColor.BLUE, 2);
            player.Resources.gems.Add(GemColor.RED, 1);

            game.listOfPlayers.Add(player);

            var result = game.NumberOfPlayerTokens();
            var sum = player.Resources.gems.Values.Sum();

            Assert.That(result.Equals(sum),Is.True);
        }
        [Test]
        public void VisibleCardsOnTable_ShouldReturnCorrectCardsForLevel1()
        {
            var game = new Game();

            var res1 = new Resources();
            res1.gems[GemColor.RED] = 3;

            var res2 = new Resources();
            res2.gems[GemColor.WHITE] = 3;


            List<Card> level1Cards = new List<Card>
            {
              new Card(1, GemColor.RED, 2, "Zaba", res1),
              new Card(1, GemColor.WHITE, 2, "Dzik", res2),
              new Card(1, GemColor.WHITE, 3, "Wito", res2),
              new Card(1, GemColor.WHITE, 1, "Bobo", res1),
            };

            Game.level1VisibleCards = level1Cards;

            var result = game.VisibleCardsOnTable(1);

            Assert.That(4.Equals(result.Length), Is.True); 
            Assert.That(level1Cards[0].Equals(result[0]),Is.True); 
            Assert.That(level1Cards[1].Equals(result[1]), Is.True);

        }


    }
}
