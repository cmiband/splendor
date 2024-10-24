using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;

namespace SplendorConsole
{
    internal class Game
    {
        AvailableCards availableCards = new AvailableCards();
        List<Card> level1Shuffled = new List<Card>();
        List<Card> level2Shuffled = new List<Card>();
        List<Card> level3Shuffled = new List<Card>();

        public void GameStart()
        {
            Random random = new Random();
            List<Player> listOfPlayers = SetNumberOfPlayers();
            List<Noble> listOfNobles = SetNumberOfNobles(listOfPlayers.Count);
            level1Shuffled = Shuffling(availableCards.level1Cards, random);
            level2Shuffled = Shuffling(availableCards.level2Cards, random);
            level3Shuffled = Shuffling(availableCards.level3Cards, random);
        }

        List<Noble> SetNumberOfNobles(int numberOfPlayers)
        {
            int numberOfNobles = numberOfPlayers + 1;
            List<Noble> nobles = new List<Noble>();

            for (int i = 0; i < numberOfNobles; i++)
            {
                nobles.Add(new Noble());
            }

            return nobles;
        }
        List<Player> SetNumberOfPlayers()
        {
            string input;
            input = Console.ReadLine();

            int numberOfPlayers = Convert.ToInt32(input);

            List<Player> players = new List<Player>();

            for (int i = 0; i < numberOfPlayers; i++)
            {
                players.Add(new Player());
            }

            return players;
        }

        void GameLoop(int numberOfPlayers)
        {
            int currentTurn = 0;
            while(currentTurn != numberOfPlayers)
            {
                //Mechanika gry?
            }
        }

        

        List<Card> Shuffling(List<Card> lista, Random random)
        {
            List<Card> wylosowaneKarty = new List<Card>();
            List<Card> duplikatListy = new List<Card>(lista);

           for(int i = 0; i < lista.Count; i++)
           {
            int index = random.Next(duplikatListy.Count);
            wylosowaneKarty.Add(duplikatListy[index]);
            duplikatListy.RemoveAt(index);     
           }
           return wylosowaneKarty;
        }
    }
}
