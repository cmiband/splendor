using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using SplendorConsole;

namespace SplendorTests
{
    [TestFixture]
    internal class GameTests
    {
        [Test]
      
        public void SetNumberOfPlayers_ShouldReturnListOfFourPlayers()
        {
            var game = new Game();

            var players = game.SetNumberOfPlayers();

            Assert.That(players.Count, Is.EqualTo(4), "Number of players should be 4");
          
        }
        [Test]
        public void AddResourcesToBank_ShouldInitializeCorrectResourceCounts()
        {
            var game = new Game();
            var bank = new Bank();
            var numberOfPlayers = 4;

            game.AddResourcesToBank(bank, numberOfPlayers);  
            foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
            {
                if (color == GemColor.GOLDEN)
                {
                    Assert.That(bank.resources.gems[color], Is.EqualTo(5), $"Golden gems should be 5 for color {color}");
                }
                else if (color != GemColor.NONE)
                {
                    Assert.That(bank.resources.gems[color], Is.EqualTo(7), $"Gems for color {color} should be 7");
                }
            }
        }
        [Test]
        public void GameLoop_ShouldIdentifyWinner_WhenSinglePlayerWins()
        {

            var game = new Game();
            var players = game.SetNumberOfPlayers();
            players[0].Points = 15; 
            players[1].Points = 10;
            players[2].Points = 5;
            players[3].Points = 7;
            game.listOfPlayers = players;

            var winner = players.FirstOrDefault(p => p.Points >= 15);

            Assert.That(winner, Is.Not.Null, "There should be a winner");
            Assert.That(winner.Points, Is.EqualTo(15), "Winner should have at least 15 points");
        }

        [Test]
        public void GameLoop_ShouldDetectDraw_WhenMultiplePlayersHaveEqualPoints()
        {
      
            var game = new Game();
            var players = game.SetNumberOfPlayers();
            players[0].Points = 10;
            players[1].Points = 10;
            players[2].Points = 5;
            players[3].Points = 7;
            game.listOfPlayers = players;
       
            var winners = players.Where(p => p.Points == 10).ToList();


            Assert.That(winners.Count, Is.GreaterThan(1), "There should be more than one winner in a draw");
            Assert.That(winners.All(p => p.Points == 10), Is.True, "All winners should have the same number of points");
        }
        [Test]
        public void MoreThan1Winner_ShouldReturnPlayerWithFewestCards_WhenSinglePlayerHasFewest()
        {
            var card1 = new Card(1, GemColor.RED, 1, "red_card.png", new Resources());
            var card2 = new Card(1, GemColor.GREEN, 1, "green_card.png", new Resources());
            var card3 = new Card(1, GemColor.BLUE, 1, "blue_card.png", new Resources());

            var player1 = new Player { hand = new List<Card> { card1, card2 }, Points = 15 };
            var player2 = new Player { hand = new List<Card> { card1 }, Points = 15 };
            var player3 = new Player { hand = new List<Card> { card1, card2, card3 }, Points = 15 };

            var winners = new List<Player> { player1, player2, player3 };
            var game = new Game();

            var result = game.MoreThan1Winner(winners);


            Assert.That(result, Is.EqualTo(player2), "Player with the fewest cards should be the winner");
        }

        [Test]
        public void MoreThan1Winner_ShouldReturnNull_WhenMultiplePlayersHaveSameFewestCards()
        {
            var card1 = new Card(1, GemColor.RED, 1, "red_card.png", new Resources());
            var card2 = new Card(1, GemColor.GREEN, 1, "green_card.png", new Resources());

            var player1 = new Player { hand = new List<Card> { card1 }, Points = 15 };
            var player2 = new Player { hand = new List<Card> { card2 }, Points = 15 };
            var player3 = new Player { hand = new List<Card> { card1, card2 }, Points = 15 };

            var winners = new List<Player> { player1, player2, player3 };
            var game = new Game();

            var result = game.MoreThan1Winner(winners);

            Assert.That(result, Is.Null, "Should return null when there is no clear winner");
        }

        [Test]
        public void MoreThan1Winner_ShouldHandleEmptyWinnersList()
        {

            var winners = new List<Player>();
            var game = new Game();

            var result = game.MoreThan1Winner(winners);


            Assert.That(result, Is.Null, "Should return null for an empty winners list");
        }
        [Test]
        public void CheckIfWinner_ShouldReturnTrue_WhenPlayerPointsAre15OrMore()
        {
            var player = new Player();

          
            var card1 = new Card(1, GemColor.RED, 5, "card1.png", new Resources());
            var card2 = new Card(1, GemColor.GREEN, 5, "card2.png", new Resources());
            var card3 = new Card(1, GemColor.GREEN, 5, "card2.png", new Resources());
          
            player.hand = new List<Card> { card1, card2,card3}; 

            player.PointsCounter();

            var game = new Game();

            var result = game.CheckIfWinner(player);

            Assert.That(result, Is.True, "Player with more than 15 points should  be a winner");
        }

        [Test]
        public void CheckIfWinner_ShouldReturnFalse_WhenPlayerPointsAreLessThan15()
        {
        
            var player = new Player { Points = 10 };
            var game = new Game();

            var result = game.CheckIfWinner(player);

            Assert.That(result, Is.False, "Player with less than 15 points should not be a winner");
        }
      

        [Test]
        public void ShowAvaiableTokens_ShouldReturnEmptyList_WhenNoTokensAvailable()
        {

            var game = new Game();
            var bank = game.bank;
            bank.resources.gems[GemColor.WHITE] = 0;
            bank.resources.gems[GemColor.BLUE] = 0;
            bank.resources.gems[GemColor.GREEN] = 0;
            bank.resources.gems[GemColor.RED] = 0;
            bank.resources.gems[GemColor.BLACK] = 0;


            List<GemColor> result = game.ShowAvaiableTokens();

            Assert.That(result, Is.Empty, "Metoda powinna zwrócić pustą listę, gdy nie ma dostępnych klejnotów.");
        }
       
        [Test]
        public void ShowPlayerTokens_ShouldReturnEmptyList_WhenNoTokens()
        {
        
            var player = new Player();
            var game = new Game();

         
            player.Resources.gems.Clear();

   
            List<GemColor> result = game.ShowAvaiableTokens();

         
            Assert.That(result, Is.Empty, "Metoda powinna zwrócić pustą listę, gdy gracz nie ma żadnych klejnotów.");
        }
      
        [Test]
        public void Price_ShouldReturnCorrectPriceString()
        {
            var game = new Game();
            var resources = new Resources();
            resources.AddResource(GemColor.RED);  
            resources.AddResource(GemColor.BLUE);  
            resources.AddResource(GemColor.GREEN); 

            var card = new Card(1, GemColor.RED, 3, "card1.png", resources);

           
            string expectedPrice = "RED 1 BLUE 1 GREEN 1 ";

           
            string actualPrice = game.Price(card);

       
            Assert.That(actualPrice, Is.EqualTo(expectedPrice), "Cena karty została skonstruowana niepoprawnie.");
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
    

   
