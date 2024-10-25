using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;

namespace SplendorConsole
{
    internal class Game
    {
        int currentTurn = 0;
        bool endGame = false;
        AvailableCards availableCards = new AvailableCards();
        List<Card> level1Shuffled = new List<Card>();
        List<Card> level2Shuffled = new List<Card>();
        List<Card> level3Shuffled = new List<Card>();
        Bank bank = new Bank();

        List<Card> level1VisibleCards = new List<Card>();
        List<Card> level2VisibleCards = new List<Card>();
        List<Card> level3VisibleCards = new List<Card>();

        List<Player> listOfPlayers = new List<Player>();
        List<Noble> listOfNobles = new List<Noble>();



        public void GameStart()
        {
            Random random = new Random();
            List<Player> listOfPlayers = SetNumberOfPlayers();
            List<Noble> listOfNobles = SetNumberOfNobles(listOfPlayers.Count);

            level1Shuffled = Shuffling(availableCards.level1Cards, random);
            level2Shuffled = Shuffling(availableCards.level2Cards, random);
            level3Shuffled = Shuffling(availableCards.level3Cards, random);
            AddResourcesToBank(bank, listOfPlayers.Count);
            SetVisibleCards();

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
            while (endGame != true)
            {
                if (currentTurn > numberOfPlayers) currentTurn = 0;
                Turn(listOfPlayers[currentTurn]);
            }
        }

        void Turn(Player player)
        {
            ChoiceOfAction(player);
        }

        void ChoiceOfAction(Player player)
        {
            GemColor color;
            GemColor[] colors;
            Console.WriteLine("=== Wybierz akcję do wykonania ===");
            Console.WriteLine("1. Weź 3 klejnoty różnych kolorów");
            Console.WriteLine("2. Weź 2 klejnoty tego samego koloru");
            Console.WriteLine("3. Zarezerwuj kartę rozwoju i weź złoty klejnot");
            Console.WriteLine("4. Kup kartę rozwoju lub wcześniej zarezerwowaną kartę i oddaj złoty klejnot");
            Console.Write("Wprowadź numer akcji (1-4): ");
            int input;
            while (!int.TryParse(Console.ReadLine(), out input) || input < 1 || input > 4)
            {
                Console.Write("Niepoprawny wybór. Wprowadź numer akcji (1-4): ");
            }
            switch (input)
            {
                case 1:
                    colors = ChoiceOfColors();
                    player.TakeThreeTokens(bank.resources, colors);
                    break;
                case 2:
                    //color = ChoiceOfColor();
                    //player.TakeTwoTokens(bank.resources, color);
                    break;
            }
        }

        GemColor[] ChoiceOfColors()
        {
            List<GemColor> avaiableTokens = ChoiceOfColor3();
            GemColor[] colors = new GemColor[3];

            int i = 0;
            Console.WriteLine("=== Wybierz kolor === ");
            foreach (GemColor item in avaiableTokens)
            {
                
            }

            return colors;
        }

        List<GemColor> ChoiceOfColor3()
        {
            List<GemColor> avaiableTokens = new List<GemColor>();

            foreach (KeyValuePair<GemColor, int> tokens in bank.resources.gems)
            {
                if (tokens.Value > 0)
                {
                    avaiableTokens.Add(tokens.Key);
                }
            }
            return avaiableTokens;
        }

        /*GemColor ChoiceOfColor()
        {
            GemColor output;
            Console.WriteLine("=== Wybierz kolor === ");
            Console.WriteLine("1. Biały");
            Console.WriteLine("2. Niebieski");
            Console.WriteLine("3. Zielony");
            Console.WriteLine("4. Czerwony");
            Console.WriteLine("5. Czarny");
            Console.Write("Wprowadź numer koloru (1-5): ");
            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 5)
            {
                Console.Write("Niepoprawny wybór. Wprowadź numer akcji (1-5): ");
            }
            switch(choice)
            {
                case 1:
                    output = GemColor.WHITE;
                    break;
                case 2:
                    output = GemColor.BLUE;
                    break;
                case 3:
                    output = GemColor.GREEN;
                    break;
                case 4:
                    output = GemColor.RED;
                    break;
                case 5:
                    output = GemColor.BLACK;
                    break;
                default:
                    throw new ArgumentException("Coś pan odwalił");
            }
            return output;

        }*/

        void SetVisibleCards()
        {
            for (int i = 0; i < 4; i++)
            {
                level1VisibleCards.Add(level1Shuffled[i]);
                level2VisibleCards.Add(level2Shuffled[i]);
                level3VisibleCards.Add(level3Shuffled[i]);
            }
        }

        List<Card> Shuffling(List<Card> lista, Random random)
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
