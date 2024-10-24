using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplendorConsole
{
    internal class Game
    {
        int currentTurn = 0;

        public void GameStart()
        {
            List<Player> listOfPlayers = SetNumberOfPlayers();
            List<Noble> listOfNobles = SetNumberOfNobles(listOfPlayers.Count);
            Bank bank = new Bank();

            
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

        void PlayerTurn()
        {

        }

        void GrabTokens()
        {

        }

        void BuyCard()
        {

        }

        void BuyMine()
        {

        }

        static List<Card> StworzDeck(List<Card> level1Cards, List<Card> level2Cards, List<Card> level3Cards) //4 karty kazdego poziomu 
        {
            Random random = new Random();
            List<Card> deck = new List<Card>();

            deck.Add(TasowanieListy(level1Cards, 4, random))
            deck.Add(TasowanieListy(level2Cards, 4, random))
            deck.Add(TasowanieListy(level3Cards, 4, random))

            return deck;
        }

        static List<Card> TasowanieListy(List<Card> lista, int liczbaKart, Random random)
        {
            List<Card> wylosowaneKarty = new List<Card>();
            List<Card> duplikatListy = new List<Card>(lista);

           for(int i = 0; i < liczbaKart, i++)
           {
            int index = random.Next(duplikatListy.Count)
            wylosowaneKarty.Add(duplikatListy[index]);
            duplikatListy.Remove(index);     
           }

           return wylosowaneKarty;

        }
    }
}
