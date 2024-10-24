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
        Bank bank = new Bank();

        List<Card> level1VisibleCards = new List<Card>();
        List<Card> level2VisibleCards = new List<Card>();
        List<Card> level3VisibleCards = new List<Card>();



        public void GameStart()
        {
            Random random = new Random();
            List<Player> listOfPlayers = SetNumberOfPlayers();
            List<Noble> listOfNobles = SetNumberOfNobles(listOfPlayers.Count);
<<<<<<< HEAD
            level1Shuffled = Shuffling(availableCards.level1Cards, 4,random);
            level2Shuffled = Shuffling(availableCards.level2Cards, 4, random);
            level3Shuffled = Shuffling(availableCards.level3Cards, 4, random);         
=======
            level1Shuffled = Shuffling(availableCards.level1Cards, random);
            level2Shuffled = Shuffling(availableCards.level2Cards, random);
            level3Shuffled = Shuffling(availableCards.level3Cards, random);
            AddResourcesToBank(bank, listOfPlayers.Count);
>>>>>>> 610f1525c2e877281dd2bc149664604310a0d2b6
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

        void AddResourcesToBank(Bank bank, int numberOfPlayers)
        {
            if (numberOfPlayers == 2)
            {
                foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
                {
                    if (color == GemColor.GOLDEN) break;
                    bank.resources.gems.Add(color, 4);
                }
                bank.resources.gems.Add(GemColor.GOLDEN, 5);
            }
            if (numberOfPlayers == 3)
            {
                foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
                {
                    if (color == GemColor.GOLDEN) break;
                    bank.resources.gems.Add(color, 5);
                }
                bank.resources.gems.Add(GemColor.GOLDEN, 5);
            }
            if (numberOfPlayers == 4)
            {
                foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
                {
                    if (color == GemColor.GOLDEN) break;
                    bank.resources.gems.Add(color, 7);
                }
                bank.resources.gems.Add(GemColor.GOLDEN, 5);
            }
        }

        void GameLoop(int numberOfPlayers)
        {
            int currentTurn = 0;
            while(currentTurn != numberOfPlayers)
            {
                //Mechanika gry?
            }
        }

        void VisibleCards()
        {

        }

        List<Card> Shuffling(List<Card> lista, int ilosc, Random random)
        {
            List<Card> wylosowaneKarty = new List<Card>();
            
           for(int i = 0; i < lista.Count; i++)
           {
            int index = random.Next(lista.Count);
            wylosowaneKarty.Add(lista[index]);
            lista.RemoveAt(index);               
           }

           return wylosowaneKarty;
        }
    }
}
