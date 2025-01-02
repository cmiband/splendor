using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json.Linq;
using static ClosedXML.Excel.XLPredefinedFormat;
using static SplendorConsole.WebserviceClient;

namespace SplendorConsole
{
    public class Game
    {
        //shuffling array
        private static Random random = new Random();

        private static int[] Shuffle(int[] array)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));

            for (int i = 0; i < array.Length - 1; ++i)
            {
                int r = random.Next(i, array.Length);
                (array[r], array[i]) = (array[i], array[r]);
            }
            return array;
        }
        private static int[] move = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42};


        private int currentTurn = 0;
        private float feedbackFromPreviousRequest = 0;
        private static AvailableCards availableCards = new AvailableCards();
        private static AvailableNobles availableNobles = new AvailableNobles();

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

        private WebserviceClient client;

        static Game()
        {
            availableCards.LoadCardsFromExcel();
            availableNobles.LoadNoblesFromExcel();
        }

        public Game(WebserviceClient client)
        {
            this.client = client;
            level1Shuffled.Clear();
            level2Shuffled.Clear();
            level3Shuffled.Clear();
            level1VisibleCards.Clear();
            level2VisibleCards.Clear();
            level3VisibleCards.Clear();
            listOfNobles.Clear();
        }

        public Bank Bank
        {
            get => bank;
        }
        public Board Board { get => board; }


        public (float, int, int, int[]?) GameStart()
        {
            Random random = new Random();
            listOfPlayers = SetNumberOfPlayers();
            listOfNobles = SetNumberOfNobles(listOfPlayers.Count);

            List<Card> level1LoadedCards = new List<Card>(availableCards.level1Cards);
            List<Card> level2LoadedCards = new List<Card>(availableCards.level2Cards);
            List<Card> level3LoadedCards = new List<Card>(availableCards.level3Cards);

            level1Shuffled = Shuffling(level1LoadedCards, random);
            level2Shuffled = Shuffling(level2LoadedCards, random);
            level3Shuffled = Shuffling(level3LoadedCards, random);


            AddResourcesToBank(bank, listOfPlayers.Count);
            SetVisibleCards();
            board = new Board(level1VisibleCards, level2VisibleCards, level3VisibleCards, level1Shuffled, level2Shuffled, level3Shuffled, listOfNobles);
            

            return GameLoop(listOfPlayers.Count);
        }


        private List<Noble> SetNumberOfNobles(int numberOfPlayers)
        {
            int numberOfNobles = numberOfPlayers + 1;
            List<Noble> nobles = new List<Noble>();
            List<Noble> loadedNobles = new List<Noble>(availableNobles.noblesList);
            List<Noble> allNobles = ShuffledNobles(loadedNobles);

            for(int i = 0; i < numberOfNobles; i++)
            {
                nobles.Add(allNobles[i]);
            }           
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


        private void AddResourcesToBank(Bank bank, int numberOfPlayers)
        {

            foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
            {
                if (color == GemColor.GOLDEN || color == GemColor.NONE) break;
                bank.resources.gems.Add(color, 7);
            }
            bank.resources.gems.Add(GemColor.GOLDEN, 5);
        }

        private (float, int, int, int[]?) GameLoop(int numberOfPlayers)
        {
            int securityCounter = 0;
            while (true)
            {
                securityCounter++;
                if(securityCounter>=10000)
                {
                    return (feedbackFromPreviousRequest, securityCounter/4, -200, ToArray());
                }
                Turn(listOfPlayers[currentTurn]);

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
                        //1 zwyciezca to listOfPlayers.IndexOf(winners[0]);
                        // Trzeba odpowiednio ustawić currentTurn żeby ToArray() zaczął od winnera
                        currentTurn = listOfPlayers.IndexOf(winners[0]);
                        return (feedbackFromPreviousRequest, securityCounter / 4, currentTurn, ToArray());
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
                            //2 zwyciezca playerIndex
                            // Trzeba odpowiednio ustawić currentTurn żeby ToArray() zaczął od winnera
                            currentTurn = playerIndex;
                            return (feedbackFromPreviousRequest, securityCounter / 4, currentTurn, ToArray());
                        }
                        else
                        {
                            Player OfficialWinner = MoreThan1Winner(winners);
                            if (OfficialWinner != null)
                            {
                                // 3 zwyciezca listOfPlayers.IndexOf(OfficialWinner)
                                // Trzeba odpowiednio ustawić currentTurn żeby ToArray() zaczął od winnera
                                currentTurn = listOfPlayers.IndexOf(OfficialWinner);
                                return (feedbackFromPreviousRequest, securityCounter / 4, currentTurn, ToArray());
                            }
                            else
                            {
                                // 4 remis (na zwrocie nie ma znaczenia kto jest brany jako main bo nikt nie wygrywa)
                                return (feedbackFromPreviousRequest, securityCounter / 4, -1, ToArray());
                            }
                        }
                    }
                }
            }
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

            RequestMoveFromServerAndExecuteIt(feedbackFromPreviousRequest, player);
            GettingNobles();
        }


        internal void Pass()
        {
            return;
        }                            

        internal int NumberOfPlayerTokens()
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

        private List<Noble> ShuffledNobles(List<Noble> deck)
        {
            System.Random random = new System.Random();

            for (int i = deck.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);

                Noble temporary = deck[i];
                deck[i] = deck[j];
                deck[j] = temporary;
            }
            return deck;
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
            foreach (GemColor requiredBonus in noble.RequiredBonuses.gems.Keys)
            {
                GemColor color = requiredBonus;
                int requiredAmount = noble.RequiredBonuses.gems[requiredBonus];
                listOfPlayers[currentTurn].BonusResources.gems.TryGetValue(color, out int playerAmount);

                if (color == GemColor.NONE)
                {
                    continue;
                }

                if (requiredAmount > playerAmount)
                {
                    return false;
                }
            }
            return true;
        }

        public void GettingNobles()
        {          
            //getting nobles zrobione pod bota bez mozliwosci wyboru. Pobiera pierwszego lepszego
            foreach (Noble noble in listOfNobles)
                if (CanGetNoble(noble))
                {
                    listOfPlayers[currentTurn].GetNoble(noble);
                    listOfNobles.Remove(noble);
                    break;
                }                  
        }                                                 

        internal void RefillBankResources(Bank bank, Card card, Dictionary<GemColor, int> resourcesUsed)
        {
            if(card==null)
            {
                throw new Exception("Nie działa try, catch :/");
            }
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
                        
                    }
                }
            }

            int currentGoldenInBank = bank.resources.gems.TryGetValue(GemColor.GOLDEN, out var currentGold) ? currentGold : 0;
            int maxGoldToAdd = 5 - currentGoldenInBank;
            if (resourcesUsed[GemColor.GOLDEN] > maxGoldToAdd)
            {               
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

        async public Task<int[]?> RequestMovesListFromServer(float feedback)
        {

            float[] gameState = Standartize(ToArray());

            var request = new
            {
                Id = 1,
                CurrentPlayer = currentTurn,
                Feedback = feedback, 
                GameState = gameState
            };

            string response = await client.SendAndFetchDataFromSocket(JsonSerializer.Serialize(request));

            JObject json = JObject.Parse(response);
            var moves = json["MovesList"]?.ToObject<int[]>();

            return moves;
        }

        async public Task RequestMoveFromServerAndExecuteIt(float feedbackForPreviousMove, Player currentPlayer)
        {
            if(this.currentTurn==0)
            {
                //w tym fragmencie kodu zakomentować if(true) żeby nie widzieć logów
                if (true)
                {
                    await Console.Out.WriteLineAsync($"[C#] Wysłano na serwer zapytanie o ruch gracza {currentTurn} oraz feedback {feedbackFromPreviousRequest} dla poprzedniego gracza.");
                }
                int[] moves = await RequestMovesListFromServer(feedbackForPreviousMove);

                var validator = new ResponseValidator();
                int numberOfInvalidMoves = validator.CheckMoves(moves, currentPlayer, this, bank, board);
                if (true)
                {
                    await Console.Out.WriteLineAsync($"[C#] gracz {currentTurn}, feedback {feedbackFromPreviousRequest} dla poprzedniego, {moves[numberOfInvalidMoves]}");
                }
                if (numberOfInvalidMoves == 0)
                {
                    if (moves[numberOfInvalidMoves] == 0)
                    {
                        feedbackFromPreviousRequest = 0;
                    }
                    else
                    {
                        feedbackFromPreviousRequest = (float)0.1;
                    }
                }
                else if (numberOfInvalidMoves <= 10)
                {
                    if (moves[numberOfInvalidMoves] == 0)
                    {
                        feedbackFromPreviousRequest = (float)-0.15;
                    }
                    else
                    {
                        feedbackFromPreviousRequest = (float)-0.05;
                    }
                }
                else
                {
                    feedbackFromPreviousRequest = (float)-0.25;
                }
            }
            else
            {
                int[] moves = Shuffle(move);
                var validator = new ResponseValidator();
                int numberOfInvalidMoves = validator.CheckMoves(moves, currentPlayer, this, bank, board);
                if (true)
                {
                    await Console.Out.WriteLineAsync($"[C#] gracz {currentTurn}, feedback {feedbackFromPreviousRequest} dla poprzedniego, {moves[numberOfInvalidMoves]}");
                }
            }
        }
    }
}
