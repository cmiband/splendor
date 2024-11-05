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
        private int currentTurn = 0;
        private AvailableCards availableCards = new AvailableCards();

        private List<Card> level1Shuffled = new List<Card>();
        private List<Card> level2Shuffled = new List<Card>();
        private List<Card> level3Shuffled = new List<Card>();
        private Bank bank = new Bank();
        private Board board;

        private List<Card> level1VisibleCards = new List<Card>();
        private List<Card> level2VisibleCards = new List<Card>();
        private List<Card> level3VisibleCards = new List<Card>();

        private List<Player> listOfPlayers = new List<Player>();
        private List<Noble> listOfNobles = new List<Noble>();

        public Bank Bank
        {
            get => bank;
        }
        public Board Board { get => board; }


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
            board = new Board(level1VisibleCards, level2VisibleCards, level3VisibleCards);
            GameLoop(listOfPlayers.Count);
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
            List<Player> players = new List<Player>();

            for (int i = 0; i < 4; i++)
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
                Console.WriteLine($"-----------------Aktualna kolejka należy do gracza {currentTurn}-----------------------");
                Turn(listOfPlayers[currentTurn]);

                // więcej logiki GameLoopa
                currentTurn = (currentTurn + 1) % numberOfPlayers;

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
            bool actionSuccess;

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
                    while (true)
                    {
                        actionSuccess = TakeThreeDifferentGems(player);
                        if (actionSuccess)
                            break; // Jeśli operacja się powiedzie, wychodzimy z wewnętrznej pętli

                    }
                    break;

                case 2:
                    while (true)
                    {
                        actionSuccess = TakeTwoSameGems(player);
                        if (actionSuccess)
                            break;

                    }
                    break;

                case 3:
                    // Logika dla rezerwacji karty niedorozwoju
                    throw new NotImplementedException();

                case 4:
                    player.BuyCardAction(this.board, this.bank);
                    break;

                case 5:
                    Pass();
                    break;
            }


        }

        void Pass()
        {
            //Implementacja logiki passa
            throw new NotImplementedException();
        }

        bool TakeThreeDifferentGems(Player player)
        {
            if (bank.resources.gems.Count < 3)
            {
                Console.WriteLine("Brak wystarczające ilośći klejnotów na planszy, wybierz inną akcję.");
                return false;
            }

            GemColor[] colors = ChoiceOfColors();
            player.TakeThreeTokens(bank.resources,colors);
            for (int i = 0; i < 3; i++)
            {
                bank.TakeOutResources(1, colors[i]);
            }
            return true;
        }

        bool TakeTwoSameGems(Player player)
        {
            bool hasSufficientGems = false;
            foreach (var gem in bank.resources.gems)
            {
                if (gem.Value >= 4) 
                {
                    hasSufficientGems = true;
                    break; 
                }
            }

            if (!hasSufficientGems)
            {
                Console.WriteLine("Brak wystarczających klejnotów w banku. Wybierz inną akcję.");
                return false;
            }

            GemColor color = ChoiceOfColor();
            if (bank.resources.gems[color] < 4)
            {
                Console.WriteLine($"Brak wystarczającej ilości klejnotów koloru {color} na planszy, wybierz inną akcję.");
                return false;
            }

            if (bank.resources.gems.Count < 2)
            {
                Console.WriteLine("Brak wystarczającej ilości klejnotów na planszy, wybierz inną akcję.");
                return false;
            }

            player.TakeTwoTokens(bank.resources, color);
            bank.TakeOutResources(2, color);
            return true;
        }


        GemColor ChoiceOfColor()
        {
            List<GemColor> availableTokens = ShowAvaiableTokens();
            GemColor color;

            int i = 1;
            Console.WriteLine("=== Wybierz kolor === ");
            foreach (GemColor item in availableTokens)
            {
                Console.WriteLine($"{i} {item}");
                i += 1;
            }

            int input;

            while (true)
            {
                Console.Write("Wprowadź numer koloru: ");
                if (int.TryParse(Console.ReadLine(), out input) && input >= 1 && input <= availableTokens.Count)
                {
                    color = availableTokens[input - 1];
                    return color;
                }
                else
                {
                    Console.WriteLine("Niepoprawny wybór. Wprowadź numer odpowiadający dostępnym kolorom.");
                }
            }
        }


        GemColor[] ChoiceOfColors()
        {
            List<GemColor> availableTokens = ShowAvaiableTokens();
            GemColor[] colors = new GemColor[3];

            int i = 1;
            Console.WriteLine("=== Wybierz kolory (3 różne) === ");
            foreach (GemColor item in availableTokens)
            {
                Console.WriteLine($"{i} {item}");
                i += 1;
            }

            List<GemColor> selectedColors = new List<GemColor>();

            for (int j = 0; j < 3; j++)
            {
                int input;

                while (true)
                {
                    Console.Write($"Wprowadź numer koloru {j + 1}: ");
                    if (int.TryParse(Console.ReadLine(), out input) && input >= 1 && input <= availableTokens.Count)
                    {
                        GemColor selectedColor = availableTokens[input - 1];
                        if (!selectedColors.Contains(selectedColor))
                        {
                            colors[j] = selectedColor;
                            selectedColors.Add(selectedColor);
                            break; 
                        }
                        else
                        {
                            Console.WriteLine("Już wybrałeś ten kolor. Wybierz inny.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Niepoprawny wybór. Wprowadź numer odpowiadający dostępnym kolorom.");
                    }
                }
            }

            return colors;
        }


        List<GemColor> ShowAvaiableTokens()
        {
            List<GemColor> avaiableTokens = new List<GemColor>();

            foreach (KeyValuePair<GemColor, int> tokens in bank.resources.gems)
            {
                if (tokens.Value > 0 && tokens.Key != GemColor.GOLDEN)
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

        List<Card> Shuffling(List<Card> deck, Random random)
        {
            for (int i = deck.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);

                Card temporary = deck[i];
                deck[i] = deck[j];
                deck[j] = temporary;
            }
            return deck;
        }
    }
}
