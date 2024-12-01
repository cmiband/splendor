using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;

namespace SplendorConsole
{
    public class Game
    {
        private int currentTurn = 0;
        private AvailableCards availableCards = new AvailableCards();

        private static List<Card> level1Shuffled = new List<Card>();
        private static List<Card> level2Shuffled = new List<Card>();
        private static List<Card> level3Shuffled = new List<Card>();
        private Bank bank = new Bank();
        private Board board;


        private static List<Card> level1VisibleCards = new List<Card>();
        private static List<Card> level2VisibleCards = new List<Card>();
        private static List<Card> level3VisibleCards = new List<Card>();

        private List<Player> listOfPlayers = new List<Player>();
        private static List<Noble> listOfNobles = new List<Noble>();
        public static List<Noble> ListOfNobles
        {
            get => listOfNobles;
            set => listOfNobles = value;
        }



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
            board = new Board(level1VisibleCards, level2VisibleCards, level3VisibleCards, level1Shuffled, level2Shuffled, level3Shuffled, listOfNobles);
            GameLoop(listOfPlayers.Count);
        }


        private List<Noble> SetNumberOfNobles(int numberOfPlayers)
        {
            int numberOfNobles = numberOfPlayers + 1;
            List<Noble> nobles = new List<Noble>();

            Resources firstResources = new Resources();
            firstResources.AddResource(GemColor.WHITE);
            firstResources.AddResource(GemColor.RED);
            firstResources.AddResource(GemColor.BLUE);
            nobles.Add(new Noble(3, firstResources));

            Resources secondResources = new Resources();
            secondResources.AddResource(GemColor.WHITE);
            secondResources.AddResource(GemColor.BLUE);
            nobles.Add(new Noble(3, secondResources));

            Resources thirdResources = new Resources();
            thirdResources.AddResource(GemColor.BLACK);
            thirdResources.AddResource(GemColor.GREEN);
            nobles.Add(new Noble(3, thirdResources));

            Resources fourthResources = new Resources();
            fourthResources.AddResource(GemColor.BLACK);
            fourthResources.AddResource(GemColor.BLUE);
            fourthResources.AddResource(GemColor.GREEN);
            nobles.Add(new Noble(3, fourthResources));

            Resources fifthResources = new Resources();
            fifthResources.AddResource(GemColor.WHITE);
            fifthResources.AddResource(GemColor.WHITE);
            fifthResources.AddResource(GemColor.WHITE);
            nobles.Add(new Noble(3, fifthResources));
            return nobles;
        }


        private List<Player> SetNumberOfPlayers()
        {
            List<Player> players = new List<Player>();

            for (int i = 0; i < 4; i++)
            {
                players.Add(new Player());
            }

            return players;
        }


        public void AddResourcesToBank(Bank bank, int numberOfPlayers)
        {

            foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
            {
                if (color == GemColor.GOLDEN || color == GemColor.NONE) break;
                bank.resources.gems.Add(color, 7);
            }
            bank.resources.gems.Add(GemColor.GOLDEN, 5);
        }

        private void GameLoop(int numberOfPlayers)
        {
            bool gameInProgress = true;
            while (gameInProgress)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"----------------- Aktualna kolejka należy do gracza {currentTurn} -----------------------");
                Console.ResetColor();
                Turn(listOfPlayers[currentTurn]);

                // więcej logiki GameLoopa
                currentTurn = (currentTurn + 1) % numberOfPlayers;
                if (currentTurn == 0)
                {
                    int winnersCount = 0;
                    List<Player> winners = new List<Player>();
                    foreach (Player player in listOfPlayers)
                    {
                        player.PointsCounter();
                        if (CheckIfWinner(player))
                        {
                            winnersCount++;
                            winners.Add(player);
                        }
                    }
                    if (winnersCount == 1)
                    {
                        Console.WriteLine($"Zwycięzca to gracz: {listOfPlayers.IndexOf(winners[0])}");
                        Console.WriteLine($"Jego liczba punktów to: {winners[0].Points}");
                        gameInProgress = false;
                    }
                    else if (winnersCount > 1)
                    {
                        winnersCount = 0;
                        int winnersPoints = 0;
                        int playerIndex = 0;
                        foreach (Player player in winners)
                        {
                            if (player.Points == winnersPoints) winnersCount++;
                            if (player.Points > winnersPoints)
                            {
                                winnersPoints = player.Points;
                                winnersCount = 1;
                                playerIndex = listOfPlayers.IndexOf(player);
                            }
                        }
                        if (winnersCount == 1)
                        {
                            Console.WriteLine($"Zwycięzca to gracz: {playerIndex}");
                            Console.WriteLine($"Jego liczba punktów to: {listOfPlayers[playerIndex].Points}");
                        }
                        else
                        {
                            Player OfficialWinner = MoreThan1Winner(winners);
                            if (OfficialWinner != null)
                            {
                                Console.WriteLine($"Zwycięzca to gracz: {listOfPlayers.IndexOf(OfficialWinner)}");
                                Console.WriteLine($"Jego liczba punktów to: {OfficialWinner.Points}");
                            }
                            else
                            {
                                Console.WriteLine("Remis");
                            }
                        }
                        gameInProgress = false;
                    }
                }
            }
            Console.WriteLine("Koniec gry :)");
        }
        private Player? MoreThan1Winner(List<Player> winners)
        {
            int minimum = 100;
            int playerIndex = 0;
            int winnersCount = 0;
            foreach (Player player in winners)
            {
                int cardsCount = player.hand.Count;
                if (cardsCount == minimum) winnersCount++;
                if (cardsCount < minimum)
                {
                    minimum = cardsCount;
                    playerIndex = winners.IndexOf(player);
                    winnersCount = 1;
                }
            }
            if (winnersCount == 1)
            {
                return winners[playerIndex];
            }

            return null;
        }
        bool CheckIfWinner(Player player)
        {
            player.PointsCounter();
            if (player.Points >= 15) return true;
            else return false;
        }

        private void Turn(Player player)
        {
            ChoiceOfAction(player);
        }

        private void ChoiceOfAction(Player player)
        {
            int input;
            bool actionSuccess;

            do
            {
                Console.WriteLine("=== Wybierz akcję do wykonania ===");
                Console.WriteLine("1. Weź 3 klejnoty różnych kolorów");
                Console.WriteLine("2. Weź 2 klejnoty tego samego koloru");
                Console.WriteLine("3. Zarezerwuj kartę i weź złoty klejnot");
                Console.WriteLine("4. Kup kartę(nową lub zarezerwowaną)");
                Console.WriteLine("5. Spasuj");
                Console.WriteLine("======================================================================");
                Console.WriteLine("Żetony w banku:  " + this.bank.resources);
                Console.WriteLine("Twoje żetony: " + listOfPlayers[currentTurn].Resources.ToString());
                Console.WriteLine("Twoje surowce z kopalń: " + listOfPlayers[currentTurn].BonusResources.ToString());
                Console.WriteLine("Twoje zakupione karty: " + listOfPlayers[currentTurn].handToString());
                Console.WriteLine("Punkty zwycięstwa: " + listOfPlayers[currentTurn].Points);
                Console.WriteLine("Twoi arystokraci: " + listOfPlayers[currentTurn].nobleToString());
                Console.WriteLine("======================================================================");
                Console.WriteLine("Karty 1 poziomu dostępne na stole: \n" + string.Join("\n", board.Level1VisibleCards) + "\n");
                Console.WriteLine("Karty 2 poziomu dostępne na stole: \n" + string.Join("\n", board.Level2VisibleCards) + "\n");
                Console.WriteLine("Karty 3 poziomu dostępne na stole: \n" + string.Join("\n", board.Level3VisibleCards));
                Console.WriteLine("======================================================================");
                Console.WriteLine("Arytokraci na stole: ");

                foreach (Noble noble in listOfNobles)
                {
                    Console.WriteLine(noble.ToString());
                }

                Console.Write("Wprowadź numer akcji (1-5): ");

                while (!int.TryParse(Console.ReadLine(), out input) || input < 1 || input > 5)
                {
                    Console.Write("Niepoprawny wybór. Wprowadź numer akcji (1-5): ");
                }



                actionSuccess = false;

                switch (input)
                {
                    case 1:
                        actionSuccess = TakeThreeDifferentGems(player);
                        if (NumberOfPlayerTokens() > 10)
                        {
                            int leave = NumberOfPlayerTokens() - 10;
                            Console.WriteLine("Posiadasz za dużo żetonów!");
                            Console.WriteLine($"Musisz odrzucić zbędne żetony w liczbie: {leave}");
                            ChoiceOfColorWithdraw(leave);
                        }
                        break;

                    case 2:
                        actionSuccess = TakeTwoSameGems(player);
                        if (NumberOfPlayerTokens() > 10)
                        {
                            int leave = NumberOfPlayerTokens() - 10;
                            Console.WriteLine("Posiadasz za dużo żetonów!");
                            Console.WriteLine($"Musisz odrzucić zbędne żetony w liczbie: {leave}");
                            ChoiceOfColorWithdraw(leave);
                        }
                        break;

                    case 3:
                        actionSuccess = ReserveCard(player);
                        if (NumberOfPlayerTokens() > 10)
                        {
                            int leave = NumberOfPlayerTokens() - 10;
                            Console.WriteLine("Posiadasz za dużo żetonów!");
                            Console.WriteLine($"Musisz odrzucić zbędne żetony w liczbie: {leave}");
                            ChoiceOfColorWithdraw(leave);
                        }

                        break;

                    case 4:
                        actionSuccess = BuyCardAction(this.board, this.bank, player);
                        break;

                    case 5:
                        Pass();
                        actionSuccess = true;
                        break;
                }
            } while (!actionSuccess);
            GettingNobles();
            Console.Clear();
        }


        private void Pass()
        {
            return;
        }

        private bool TakeThreeDifferentGems(Player player)
        {
            bool hasSufficientGems = false;
            int counter = 0;
            foreach (var gem in bank.resources.gems)
            {
                if (gem.Value >= 1 && gem.Key != GemColor.GOLDEN)
                {
                    counter += 1;
                }
            }
            int wantToContinue;
            GemColor[] colors;
            if (counter >= 3)
                hasSufficientGems = true;

            if (counter == 2)
            {
                Console.WriteLine("Zostały tylko 2 kolory klejnotów do wyboru. Możesz wziąć maksymalnie 2 różne spośród pozostałych");
                Console.WriteLine("Czy chcesz kontynuować?");
                Console.WriteLine("1 - Tak");
                Console.WriteLine("2 - Nie");
                wantToContinue = Convert.ToInt32(Console.ReadLine());
                if (wantToContinue == 1)
                {
                    colors = ChoiceOfColors(2);
                    if (colors == null) return false;
                    player.TakeThreeTokens(bank.resources, colors);
                    for (int i = 0; i < 2; i++)
                    {
                        bank.TakeOutResources(1, colors[i]);
                    }
                    return true;
                }
                return false;
            }

            if (counter == 1)
            {
                Console.WriteLine("Został tylko 1 kolor klejnotów do wyboru. Możesz wziąć maksymalnie 1 różny spośród pozostałych");
                Console.WriteLine("Czy chcesz kontynuować?");
                Console.WriteLine("1 - Tak");
                Console.WriteLine("2 - Nie");
                wantToContinue = Convert.ToInt32(Console.ReadLine());
                if (wantToContinue == 1)
                {
                    colors = ChoiceOfColors(1);
                    if (colors == null) return false;
                    player.TakeThreeTokens(bank.resources, colors);
                    bank.TakeOutResources(1, colors[0]);
                    return true;
                }
                return false;
            }

            if (!hasSufficientGems)
            {
                Console.WriteLine("Brak wystarczających klejnotów w banku. Wybierz inną akcję.");
                return false;
            }

            colors = ChoiceOfColors(3);
            if (colors == null) return false;
            player.TakeThreeTokens(bank.resources, colors);
            for (int i = 0; i < 3; i++)
            {
                bank.TakeOutResources(1, colors[i]);
            }
            return true;
        }

        public bool TakeTwoSameGems(Player player)
        {
            bool hasSufficientGems = false;
            foreach (var gem in bank.resources.gems)
            {
                if (gem.Value >= 4 && gem.Key != GemColor.GOLDEN)
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
            if (color == GemColor.NONE) return false;
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

        private void ChoiceOfColorWithdraw(int tokenNumber)
        {
            int i = 1;
            List<GemColor> recources = new List<GemColor>();

            for (int j = 0; j < tokenNumber; j++)
            {
                List<GemColor> playerTokens = ShowPlayerTokens();
                Console.WriteLine("Twoje żetony: " + listOfPlayers[currentTurn].Resources.ToStringForWithdraw());
                int input;

                while (true)
                {
                    Console.Write($"Wprowadź numer koloru {j + 1} (indeksowane od jedynki): ");

                    if (int.TryParse(Console.ReadLine(), out input) && input >= 1 && input <= playerTokens.Count())
                    {
                        GemColor selectedColor = playerTokens[input - 1];
                        listOfPlayers[currentTurn].RemoveOneToken(listOfPlayers[currentTurn].Resources, playerTokens[input - 1]);
                        bank.resources.AddResource(selectedColor);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Niepoprawny wybór. Wprowadź numer odpowiadający dostępnym kolorom.");
                    }
                }
            }

        }

        public GemColor ChoiceOfColor()
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
            Console.WriteLine("Aby wrócić wpisz 0 :)");
            int input;

            while (true)
            {
                Console.Write("Wprowadź numer koloru: ");
                if (int.TryParse(Console.ReadLine(), out input) && input >= 1 && input <= availableTokens.Count)
                {
                    color = availableTokens[input - 1];
                    return color;
                }
                else if (input == 0) return GemColor.NONE;
                else
                {
                    Console.WriteLine("Niepoprawny wybór. Wprowadź numer odpowiadający dostępnym kolorom.");
                }
            }
        }


        private GemColor[] ChoiceOfColors(int numberOfColors)
        {
            List<GemColor> availableTokens = ShowAvaiableTokens();
            GemColor[] colors = new GemColor[3];

            int i = 1;
            Console.WriteLine("=== Wybierz kolory (różne) === ");
            foreach (GemColor item in availableTokens)
            {
                Console.WriteLine($"{i} {item}");
                i += 1;
            }
            Console.WriteLine("Aby wrócić wpisz 0 :)");
            List<GemColor> selectedColors = new List<GemColor>();

            for (int j = 0; j < numberOfColors; j++)
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
                    else if (input == 0)
                    {
                        return null;
                    }
                    else
                    {
                        Console.WriteLine("Niepoprawny wybór. Wprowadź numer odpowiadający dostępnym kolorom.");
                    }
                }
            }

            return colors;
        }


        public List<GemColor> ShowAvaiableTokens()
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

        private List<GemColor> ShowPlayerTokens()
        {
            List<GemColor> playerTokens = new List<GemColor>();

            foreach (KeyValuePair<GemColor, int> tokens in listOfPlayers[currentTurn].Resources.gems)
            {
                playerTokens.Add(tokens.Key);
            }
            return playerTokens;
        }

        public int NumberOfPlayerTokens()
        {
            int counter = 0;
            foreach (KeyValuePair<GemColor, int> token in listOfPlayers[currentTurn].Resources.gems)
            {
                counter += token.Value;
            }
            return counter;
        }

        private void SetVisibleCards()
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

        private List<Card> Shuffling(List<Card> deck, Random random)
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

        private bool ReserveCard(Player player)
        {
            if (player.ReservedCardsCounter >= 3)
            {
                Console.WriteLine("Nie mozna zarezerwowac wiecej kart!");
                Console.WriteLine();
                return false;
            }

            Console.WriteLine("=== Wybierz metodę rezerwowania ===");
            Console.WriteLine("1. Rezerwuj kartę ze stolika");
            Console.WriteLine("2. Rezerwuj kartę w ciemno ze stosu");
            Console.WriteLine("3. Powrót");
            int reserveinput;
            while (!int.TryParse(Console.ReadLine(), out reserveinput) || reserveinput < 1 || reserveinput > 3)
            {
                Console.Write("Niepoprawny wybór. Wprowadź numer akcji (1-3): ");
            }
            if (reserveinput == 3) return false;
            if (bank.resources.gems[GemColor.GOLDEN] > 0)
            {

                if (player.Resources.gems.ContainsKey(GemColor.GOLDEN))
                {
                    player.Resources.gems[GemColor.GOLDEN] += 1;
                }
                else
                {
                    player.Resources.gems.Add(GemColor.GOLDEN, 1);
                }
                bank.TakeOutResources(1, GemColor.GOLDEN);
            }

            if (reserveinput == 1)
            {
                Console.WriteLine("=== Wybierz którego poziomu kartę chcesz zarezerwować ===");
                Console.WriteLine("1 poziom");
                Console.WriteLine("2 poziom");
                Console.WriteLine("3 poziom");
                int cardLevel;
                while (!int.TryParse(Console.ReadLine(), out cardLevel) || cardLevel < 1 || cardLevel > 3)
                {
                    Console.Write("Niepoprawny wybór. Wprowadź numer akcji (1-3): ");
                }
                int input;
                Console.WriteLine("=== Wybierz kartę do zarezerwowania ===");
                Card[] cardsOnTable = VisibleCardsOnTable(cardLevel);
                while (!int.TryParse(Console.ReadLine(), out input) || input < 1 || input > 4)
                {
                    Console.Write("Niepoprawny wybór. Wprowadź numer akcji (1-4): ");
                }
                player.ReserveCard(cardsOnTable[input - 1]);
                board.ReplaceMissingCard(cardLevel, cardsOnTable[input - 1]);
            }
            else
            {
                Console.WriteLine("=== Wybierz którego poziomu kartę chcesz zarezerwować ===");
                Console.WriteLine("1 poziom");
                Console.WriteLine("2 poziom");
                Console.WriteLine("3 poziom");
                int cardLevel;
                while (!int.TryParse(Console.ReadLine(), out cardLevel) || cardLevel < 1 || cardLevel > 3)
                {
                    Console.Write("Niepoprawny wybór. Wprowadź numer akcji (1-3): ");
                }
                switch (cardLevel)
                {
                    case 1:
                        Random randomLevel1Card = new Random();
                        Card level1CardToReserve = level1Shuffled[randomLevel1Card.Next(level1Shuffled.Count)];
                        player.ReserveCard(level1CardToReserve);
                        level1Shuffled.Remove(level1CardToReserve);
                        break;
                    case 2:
                        Random randomLevel2Card = new Random();
                        Card level2CardToReserve = level2Shuffled[randomLevel2Card.Next(level2Shuffled.Count)];
                        player.ReserveCard(level2CardToReserve);
                        level2Shuffled.Remove(level2CardToReserve);
                        break;
                    case 3:
                        Random randomLevel3Card = new Random();
                        Card level3CardToReserve = level3Shuffled[randomLevel3Card.Next(level3Shuffled.Count)];
                        player.ReserveCard(level3CardToReserve);
                        level3Shuffled.Remove(level3CardToReserve);
                        break;
                }
            }
            return true;
        }
        private Card[] VisibleCardsOnTable(int cardlevel)
        {
            Card[] cardsOnTable = new Card[4];
            for (int i = 0; i < cardsOnTable.Length; i++)
            {
                if (cardlevel == 1)
                {
                    cardsOnTable[i] = level1VisibleCards[i];
                }
                else if (cardlevel == 2)
                {
                    cardsOnTable[i] = level2VisibleCards[i];
                }
                else
                {
                    cardsOnTable[i] = level3VisibleCards[i];
                }
            }
            int j = 1;
            foreach (Card card in cardsOnTable)
            {
                Console.WriteLine(j.ToString() + ". Level: " + card.Level + " Karta koloru: " + card.BonusColor + "  Cena: " + Price(card) + " Victory Points: " + card.Points);
                j++;
            }
            return cardsOnTable;
        }

        private string Price(Card card)
        {
            string price = "";
            foreach (KeyValuePair<GemColor, int> tokens in card.DetailedPrice)
            {
                if (tokens.Value != 0)
                    price += tokens.Key + " " + tokens.Value.ToString() + " ";
            }
            return price;
        }

        public bool CanGetNoble(Noble noble)
        {
            int counter = 0;
            int noblesCounter = 0;

            foreach (GemColor requiredBonus in noble.RequiredBonuses.gems.Keys)
            {

                GemColor color = requiredBonus;
                int requiredAmount = noble.RequiredBonuses.gems[requiredBonus];
                int playerAmount = 0;



                foreach (Card card in listOfPlayers[currentTurn].hand)
                {
                    if (listOfPlayers[currentTurn].BonusResources.gems.TryGetValue(color, out int count))
                    {
                        playerAmount = count;
                        break;
                    }
                }

                if (requiredAmount <= playerAmount)
                {


                    counter += 1;
                    noblesCounter += 1;
                }
                else
                    noblesCounter += 1;

            }
            if (counter != noblesCounter) return false;
            else return true;
        }

        public bool CanGetMultipleNobles()
        {
            int counter = 0;
            foreach (Noble noble in listOfNobles)
            {
                if (CanGetNoble(noble))
                    counter++;
            }
            if (counter > 1)
                return true;
            else
                return false;
        }

        public void GettingNobles()
        {
            if (CanGetMultipleNobles() == false)
            {
                foreach (Noble noble in listOfNobles)
                    if (CanGetNoble(noble))
                    {
                        listOfPlayers[currentTurn].GetNoble(noble);
                        listOfNobles.Remove(noble);
                        break;
                    }
            }
            else
            {
                List<int> AvailableIndexNobles = new List<int>();
                for (int i = 0; i < listOfNobles.Count; i++)
                {
                    Noble noble = listOfNobles[i];
                    if (CanGetNoble(noble))
                        AvailableIndexNobles.Add(i);
                }

                Console.WriteLine("Arystokraci, których możesz zdobyć: ");
                for (int i = 0; i < AvailableIndexNobles.Count; i++)
                {
                    Console.WriteLine(AvailableIndexNobles[i] + ". " + listOfNobles[i].ToString());

                }

                int choice = 0;

                while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > AvailableIndexNobles.Count)
                {
                    Console.WriteLine("Niepoprawny numer. Podaj jeszcze raz.");
                    Console.WriteLine("Wybierz arystokratę: ");
                }

                Noble playerChoice = listOfNobles[choice];
                listOfPlayers[currentTurn].GetNoble(playerChoice);
                listOfNobles.Remove(playerChoice);
                Console.WriteLine("Wybrany arystokrata został dodany do gracza.");

            }

        }



        public int BuyCardOption()
        {
            int opChoice;
            while (!int.TryParse(Console.ReadLine(), out opChoice) || opChoice < 1 || opChoice > 3)
            {
                Console.WriteLine("Niepoprawny poziom. Wprowadź 1, 2 lub 3");
            }
            return opChoice;
        }
        public int ChooseLevelOfCard()
        {
            int level;
            while (!int.TryParse(Console.ReadLine(), out level) || level < 1 || level > 3)
            {
                Console.WriteLine("Niepoprawny poziom. Wprowadź 1, 2 lub 3:");
            }
            return level;
        }
        public int ChooseCardIndex(List<Card> visibleCards)
        {
            int cardIndex;
            while (!int.TryParse(Console.ReadLine(), out cardIndex) || cardIndex < 1 || cardIndex > visibleCards.Count)
            {
                Console.WriteLine("Niepoprawny wybór. Wprowadź numer odpowiadający wybranej karcie:");
            }
            return cardIndex;
        }
        public int ChooseReservedCardIndex(List<Card> reservedCards)
        {
            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > reservedCards.Count)
            {
                Console.WriteLine("Niepoprawna karta.");
            }
            return choice;
        }
        public bool WantToSpendGoldCoin()
        {
            int wantTo;
            while (true)
            {
                wantTo = Convert.ToInt32(Console.ReadLine());
                if (wantTo == 2)
                    return false;
                else if (wantTo == 1)
                    return true;
                else
                    Console.WriteLine("Podano zły klawisz. Podaj 1 lub 2");
            }
        }
        public bool BuyCardAction(Board board, Bank bank, Player player)
        {
            Console.WriteLine("Chcesz kupić nową kartę czy kupić zarezerwowaną?");
            Console.WriteLine("[1] Nowa");
            Console.WriteLine("[2] Zarerwowana");
            Console.WriteLine("[3] Powrót");
            int opChoice = BuyCardOption();
            if (opChoice == 3) return false;
            if (opChoice == 1)
            {
                Console.WriteLine("Wybierz poziom karty do zakupu (1, 2 lub 3):");
                int level = ChooseLevelOfCard();
                List<Card> visibleCards = board.GetVisibleCards(level);
                Console.WriteLine("Wybierz numer karty do zakupu:");
                for (int i = 0; i < visibleCards.Count; i++)
                {
                    Console.WriteLine($"{i + 1}: {visibleCards[i]}");
                }

                int cardIndex = ChooseCardIndex(visibleCards);

                Card selectedCard = visibleCards[cardIndex - 1];

                var success = BuyCard(board, selectedCard, bank, player.NOT_BUYING_RESERVED_CARD, player);
                if (success)
                {
                    Console.WriteLine("Karta została pomyślnie zakupiona!");
                    board.ReplaceMissingCard(level, selectedCard);
                    return true;
                }
                else
                {
                    Console.WriteLine("Operacja kupowania karty nie powiodła się.");
                    return false;
                }
            }
            else
            {
                return HandleBuyReservedCard(board, bank, player);
            }
        }
        public bool HandleBuyReservedCard(Board board, Bank bank, Player player)
        {
            if (player.ReservedCards.Count == 0)
            {
                Console.WriteLine("Nie masz zarezerwowanych kart.");
                return false;
            }

            Console.WriteLine("Wybierz zarezerwowaną kartę, którą chcesz kupić: ");
            for (int i = 0; i < player.ReservedCards.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] {player.ReservedCards[i]}");
            }
            int choice = ChooseReservedCardIndex(player.ReservedCards);

            Card selectedCard = player.ReservedCards[choice - 1];
            if(this.BuyCard(board, selectedCard, bank, player.BUYING_RESERVED_CARD, player))
            {
                player.ReservedCards.Remove(selectedCard);
                player.ReservedCardsCounter--;
                return true;
            }


            return false;
        }
        public bool BuyCard(Board board, Card card, Bank bank, bool isBuyingReservedCard, Player player)
        {
            bool choiceForGolden = false;

            Console.WriteLine("Czy chcesz użyć złotego żetonu aby zapłacić za kartę?");
            Console.WriteLine("1 - Tak");
            Console.WriteLine("2 - Nie");

            choiceForGolden = WantToSpendGoldCoin();

            var simulatedResourcesUsed = new Dictionary<GemColor, int>();
            foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
            {
                simulatedResourcesUsed[color] = 0;
            }

            if (!player.CanAffordCardWithGolden(card) && !player.CanAffordCard(card))
            {
                Console.WriteLine("Nie stać cię na tę kartę.");
                return false;
            }

            var cardPrice = card.DetailedPrice.gems;

            foreach (var colorOnCard in cardPrice)
            {
                GemColor color = colorOnCard.Key;
                int requiredAmount = colorOnCard.Value;

                int bonusAmount = player.BonusResources.gems.TryGetValue(color, out var bonus) ? bonus : 0;
                requiredAmount -= Math.Min(bonusAmount, requiredAmount);

                if (requiredAmount > 0)
                {
                    int playerAmount = player.Resources.gems.TryGetValue(color, out var playerR) ? playerR : 0;

                    if (playerAmount >= requiredAmount)
                    {
                        simulatedResourcesUsed[color] += requiredAmount;
                    }
                    else
                    {
                        if (choiceForGolden)
                        {
                            int deficit = requiredAmount - playerAmount;

                            if (player.Resources.gems[GemColor.GOLDEN] >= deficit)
                            {
                                simulatedResourcesUsed[color] += playerAmount;
                                simulatedResourcesUsed[GemColor.GOLDEN] += deficit;
                            }
                            else
                            {
                                Console.WriteLine("Nie masz wystarczających zasobów.");
                                return false;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Nie masz wystarczających zasobów.");
                            return false;
                        }
                    }
                }
            }

            foreach (var resource in simulatedResourcesUsed)
            {
                if (resource.Value > 0 && resource.Key != GemColor.GOLDEN)
                {
                    player.Resources.gems[resource.Key] -= resource.Value;
                    if (player.Resources.gems[resource.Key] == 0)
                        player.Resources.gems.Remove(resource.Key);
                }
            }

            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
            {
                player.Resources.gems[GemColor.GOLDEN] -= simulatedResourcesUsed[GemColor.GOLDEN];
                if (player.Resources.gems[GemColor.GOLDEN] == 0)
                {
                    player.Resources.gems.Remove(GemColor.GOLDEN);
                }
            }

            RefillBankResources(bank, card, simulatedResourcesUsed);

            if (simulatedResourcesUsed[GemColor.GOLDEN] > 0)
            {
                bank.AddGoldenGem(simulatedResourcesUsed[GemColor.GOLDEN]);
            }

            player.AddCardToPlayer(card);
            player.BonusResources.AddResource(card.BonusColor);
            player.Points += card.Points;

            Console.WriteLine($"Karta {card} została zakupiona!");
            return true;
        }

        private void RefillBankResources(Bank bank, Card card, Dictionary<GemColor, int> resourcesUsed)
        {
            foreach (var gemCost in card.DetailedPrice.gems)
            {
                GemColor color = gemCost.Key;

                if (resourcesUsed.TryGetValue(color, out int amountPaidWithColor))
                {
                    int currentBankAmount = bank.resources.gems.TryGetValue(color, out var bankAmount) ? bankAmount : 0;
                    int amountToAdd = Math.Min(amountPaidWithColor, 7 - currentBankAmount);

                    if (amountToAdd > 0)
                    {
                        bank.resources.gems[color] = currentBankAmount + amountToAdd;
                    }

                    if (amountPaidWithColor > amountToAdd)
                    {
                        Console.WriteLine($"Nie można dodać {amountPaidWithColor - amountToAdd} zasobów {color} do banku, ponieważ jest pełny.");
                    }
                }
            }

            int currentGoldenInBank = bank.resources.gems.TryGetValue(GemColor.GOLDEN, out var currentGold) ? currentGold : 0;
            int maxGoldToAdd = 5 - currentGoldenInBank;
            if (resourcesUsed[GemColor.GOLDEN] > maxGoldToAdd)
            {
                Console.WriteLine($"Maksymalna liczba złotych żetonów w banku to 5, więc tylko {maxGoldToAdd} można dodać.");
                bank.resources.gems[GemColor.GOLDEN] = currentGold + maxGoldToAdd;
            }
        }





        public int[] ToArray()
        {
            int[] output = new int[348];
            int pointer = 0;
            foreach (var item in Board.ToArray())
            {
                output[pointer++] = item;
            }
            foreach (var item in Bank.ToArray())
            {
                output[pointer++] = item;
            }
            foreach (var item in listOfNobles)
            {
                foreach (var parameter in item.ToArray())
                {
                    output[pointer++] = parameter;
                }
            }
            while(pointer<167)
            {
                output[pointer++] = 0;
                output[pointer++] = 16;
                output[pointer++] = 16;
                output[pointer++] = 16;
                output[pointer++] = 16;
                output[pointer++] = 16;
            }

            for(int i=0; i<4; i++)
            {
                foreach (var item in listOfPlayers[(currentTurn+i)%4].ToArray())
                {
                    output[pointer++] = item;
                }
            }
            return output;
        }

        public float[] Standartize(int[] array)
        {
            int n = array.Length;
            float sum = 0;

            for (int i = 0; i < n; i++)
            {
              sum += array[i];
            }

            float mean = sum / n;

            float q = 0;
            for (int i = 0; i < n; i++)
            {
               float xiu = (float) Math.Pow((array[i] - mean), 2);
                q += xiu;
            }

            float standardDeviation = (float)Math.Sqrt(q / n);

            float[] finalZScore = new float[348];

            for (int i = 0; i < n; i++)
            {
                float zScore = (array[i] - mean) / standardDeviation;
                finalZScore[i] = zScore;
            }

            return finalZScore;

        }
    }
}
