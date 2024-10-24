using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplendorConsole
{
    internal class Game
    {
        public void Start()
        {

        }

        public Player SetNumberOfPlayers()
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
