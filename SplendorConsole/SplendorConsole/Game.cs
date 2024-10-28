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

        public Bank Bank
        {
            get => bank;
        }


        public void GameStart()
        {
            availableCards.LoadCardsFromExcel();
            Random random = new Random();
            listOfPlayers = SetNumberOfPlayers();
            listOfNobles = SetNumberOfNobles(listOfPlayers.Count);

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
            int numberOfPlayers;
            do
            {
                Console.WriteLine("=== Podaj ilość graczy (od 2 do 4) ===");
                string input = Console.ReadLine();

                
                if (!int.TryParse(input, out numberOfPlayers) || numberOfPlayers < 2 || numberOfPlayers > 4)
                {
                    Console.WriteLine("Nieprawidłowa liczba graczy. Wprowadź wartość od 2 do 4.");
                }
            } while (numberOfPlayers < 2 || numberOfPlayers > 4);

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
            while (true)
            {
                if (currentTurn > numberOfPlayers) currentTurn = 0;
                Turn(listOfPlayers[currentTurn]);

                // więcej logiki GameLoopa
                currentTurn += 1;
            }
        }

        void Turn(Player player)
        {
            ChoiceOfAction(player);

            // więcej logiki w turze?
        }

        void ChoiceOfAction(Player player)
        {
            int input;
            do
            {
                Console.WriteLine("=== Wybierz akcję do wykonania ===");
                Console.WriteLine("1. Weź 3 klejnoty różnych kolorów");
                Console.WriteLine("2. Weź 2 klejnoty tego samego koloru");
                Console.WriteLine("3. Zarezerwuj kartę niedorozwoju i weź złoty klejnot");
                Console.WriteLine("4. Kup kartę niedorozwoju lub wcześniej zarezerwowaną kartę i puść złoty klejnot");
                Console.WriteLine("5. Spasuj byczku sobie turke");
                Console.Write("Wprowadź numer akcji (1-5): ");

                while (!int.TryParse(Console.ReadLine(), out input) || input < 1 || input > 5)
                {
                    Console.Write("Niepoprawny wybór. Wprowadź numer akcji (1-5): ");
                }

                switch (input)
                {
                    case 1:
                        if (!TakeThreeDifferentGems(player))
                        {
                            Console.WriteLine("Spróbuj ponownie.");
                            continue; // Wraca do wyboru akcji
                        }
                        break;
                    case 2:
                        if(!TakeTwoSameGems(player))
                        {
                            Console.WriteLine("Spróbuj ponownie.");
                            continue; 
                        }
                        break;
                    case 3:
                        // Logika dla rezerwacji karty niedorozwoju
                        break;
                    case 4:
                        // Logika dla kupna karty niedorozwoju
                        break;
                    case 5:
                        Pass();
                        break;
                }
            } while (input != 5);

        }

        void Pass()
        {
            //Implementacja logiki passa
        }

        bool TakeThreeDifferentGems(Player player)
        {
            if (bank.resources.gems.Count < 3)
            {
                Console.WriteLine("Brak wystarczające ilośći klejnotów na planszy, wybierz inną akcję.");
                return false;
            }

            GemColor[] colors = ChoiceOfColors();
            player.TakeThreeTokens(colors);
            for (int i = 0; i < 3; i++)
            {
                bank.TakeOutResources(1, colors[i]);
            }
            return true;
        }

        bool TakeTwoSameGems(Player player)
        {
            GemColor color = ChoiceOfColor();
            if (bank.resources.gems[color] < 4)
            {
                Console.WriteLine($"Brak wystarczające ilośći klejnotów koloru {color} na planszy, wybierz inną akcję.");
                return false;
            }
            else if(bank.resources.gems.Count < 2)
            {
                Console.WriteLine("Brak wystarczające ilośći klejnotów na planszy, wybierz inną akcję.");
                return false;
            }
            player.TakeTwoTokens(color);
            bank.TakeOutResources(2, color);
            return true;
        }

        GemColor ChoiceOfColor()
        {
            List<GemColor> avaiableTokens = ShowAvaiableTokens();
            GemColor color;

            int i = 1;
            Console.WriteLine("=== Wybierz kolor === ");
            foreach (GemColor item in avaiableTokens)
            {
                Console.WriteLine($"{i} {item}");
                i += 1;
            }
            int input = Convert.ToInt32(Console.ReadLine());
            color = avaiableTokens[input];
            return color;
        }

        GemColor[] ChoiceOfColors()
        {
            List<GemColor> avaiableTokens = ShowAvaiableTokens();
            GemColor[] colors = new GemColor[3];

            int i = 1;
            Console.WriteLine("=== Wybierz kolor === ");
            foreach (GemColor item in avaiableTokens)
            {
                Console.WriteLine($"{i} {item}");
                i += 1;
            }
            for (int j = 0; j < 3; j++)
            {
                int input = Convert.ToInt32(Console.ReadLine());
                colors[j] = avaiableTokens[input];
            }

            return colors;
        }

        List<GemColor> ShowAvaiableTokens()
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

        void SetVisibleCards()
        {
            for (int i = 0; i < 4; i++)
            {
                level1VisibleCards.Add(level1Shuffled[0]);
                level2VisibleCards.Add(level2Shuffled[0]);
                level3VisibleCards.Add(level3Shuffled[0]);

                level1Shuffled.RemoveAt(0);
                level2Shuffled.RemoveAt(0);
                level3Shuffled.RemoveAt(0);
            }
        }

        List<Card> Shuffling(List<Card> lista, Random random)
        {
           List<Card> wylosowaneKarty = new List<Card>();
            
           for(int i = 0; i < lista.Count; i++)
           {
                int index = random.Next(lista.Count);
                wylosowaneKarty.Add(lista[index]);
           }

           return wylosowaneKarty;
        }
    }
}
