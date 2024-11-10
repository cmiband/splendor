@ -1,12 +1,9 @@
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2013.Excel;
using DocumentFormat.OpenXml.Wordprocessing;

namespace SplendorConsole
@ -38,17 +35,17 @@ namespace SplendorConsole

        public void GameStart()
        {

            
            availableCards.LoadCardsFromExcel();
            Random random = new Random();
            listOfPlayers = SetNumberOfPlayers();
            listOfNobles = SetNumberOfNobles(listOfPlayers.Count);

            
            level1Shuffled = Shuffling(availableCards.level1Cards, random);
            level2Shuffled = Shuffling(availableCards.level2Cards, random);
            level2Shuffled = Shuffling(availableCards.level2Cards, random);                    
            level3Shuffled = Shuffling(availableCards.level3Cards, random);


            
           
            AddResourcesToBank(bank, listOfPlayers.Count);
            SetVisibleCards();
            board = new Board(level1VisibleCards, level2VisibleCards, level3VisibleCards, level1Shuffled, level2Shuffled, level3Shuffled);
@ -82,14 +79,14 @@ namespace SplendorConsole

        private void AddResourcesToBank(Bank bank, int numberOfPlayers)
        {

            
            foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
            {
                if (color == GemColor.GOLDEN || color == GemColor.NONE) break;
                bank.resources.gems.Add(color, 7);
            }
            bank.resources.gems.Add(GemColor.GOLDEN, 5);

            
        }

        private void GameLoop(int numberOfPlayers)
@ -97,46 +94,48 @@ namespace SplendorConsole
            bool gameInProgress = true;
            while (gameInProgress)
            {
                Console.WriteLine($"-----------------Aktualna kolejka należy do gracza {currentTurn}-----------------------");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"----------------- Aktualna kolejka należy do gracza {currentTurn} -----------------------");
                Console.ResetColor();
                Turn(listOfPlayers[currentTurn]);

                // więcej logiki GameLoopa
                currentTurn = (currentTurn + 1) % numberOfPlayers;
                if (currentTurn == 0)
                if (currentTurn==0)
                {
                    int winnersCount = 0;
                    List<Player> winners = new List<Player>();
                    foreach (Player player in listOfPlayers)
                    foreach(Player player in listOfPlayers)
                    {
                        player.PointsCounter();
                        if (CheckIfWinner(player))
                        if(CheckIfWinner(player))
                        {
                            winnersCount++;
                            winners.Add(player);
                        }
                    }
                    if (winnersCount == 1)
                    if(winnersCount==1)
                    {
                        Console.WriteLine($"Zwycięzca to gracz: {listOfPlayers.IndexOf(winners[0])}");
                        Console.WriteLine($"Jego liczba punktów to: {winners[0].Points}");
                        gameInProgress = false;
                    }
                    else if (winnersCount > 1)
                    else if(winnersCount>1)
                    {
                        winnersCount = 0;
                        int winnersPoints = 0;
                        int playerIndex = 0;
                        foreach (Player player in winners)
                        foreach(Player player in winners)
                        {
                            if (player.Points == winnersPoints) winnersCount++;
                            if (player.Points > winnersPoints)
                            if(player.Points > winnersPoints)
                            {
                                winnersPoints = player.Points;
                                winnersCount = 1;
                                playerIndex = listOfPlayers.IndexOf(player);
                            }
                        }
                        if (winnersCount == 1)
                        if(winnersCount==1)
                        {
                            Console.WriteLine($"Zwycięzca to gracz: {playerIndex}");
                            Console.WriteLine($"Jego liczba punktów to: {listOfPlayers[playerIndex].Points}");
@ -144,12 +143,11 @@ namespace SplendorConsole
                        else
                        {
                            Player OfficialWinner = MoreThan1Winner(winners);
                            if (OfficialWinner != null)
                            if(OfficialWinner != null)
                            {
                                Console.WriteLine($"Zwycięzca to gracz: {listOfPlayers.IndexOf(OfficialWinner)}");
                                Console.WriteLine($"Jego liczba punktów to: {OfficialWinner.Points}");
                            }
                            else
                            } else
                            {
                                Console.WriteLine("Remis");
                            }
@ -165,11 +163,11 @@ namespace SplendorConsole
            int minimum = 100;
            int playerIndex = 0;
            int winnersCount = 0;
            foreach (Player player in winners)
            foreach(Player player in winners)
            {
                int cardsCount = player.hand.Count;
                if (cardsCount == minimum) winnersCount++;
                if (cardsCount < minimum)
                if(cardsCount < minimum )
                {
                    minimum = cardsCount;
                    playerIndex = winners.IndexOf(player);
@ -209,7 +207,7 @@ namespace SplendorConsole
                Console.WriteLine("4. Kup kartę(nową lub zarezerwowaną)");
                Console.WriteLine("5. Spasuj");
                Console.WriteLine("======================================================================");
                Console.WriteLine("Twoje żetony: \n" + listOfPlayers[currentTurn].Resources.ToString());
                Console.WriteLine("Twoje żetony: " + listOfPlayers[currentTurn].Resources.ToString());
                Console.WriteLine("Twoje surowce z kopalń: " + listOfPlayers[currentTurn].BonusResources.ToString());
                Console.WriteLine("Twoje zakupione karty: " + listOfPlayers[currentTurn].handToString());
                Console.WriteLine("Punkty zwycięstwa: " + listOfPlayers[currentTurn].Points);
@ -222,57 +220,36 @@ namespace SplendorConsole
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
                        actionSuccess = player.BuyCardAction(this.board, this.bank);
                        actionSuccess = BuyCardAction(this.board, this.bank, player);
                        break;

                    case 5:
                        Pass();
                        actionSuccess = true;
                        actionSuccess = true;                      
                        break;
                }
            } while (!actionSuccess);
            Console.Clear();
        }
        


        private void Pass()
        {
@ -300,7 +277,7 @@ namespace SplendorConsole
            }

            GemColor[] colors = ChoiceOfColors();
            player.TakeThreeTokens(bank.resources, colors);
            player.TakeThreeTokens(bank.resources,colors);
            for (int i = 0; i < 3; i++)
            {
                bank.TakeOutResources(1, colors[i]);
@ -308,47 +285,15 @@ namespace SplendorConsole
            return true;
        }

        
        private void ChoiceOfColorWithdraw(int liczbaZetonow)
        {
            int i = 1;
            List<GemColor> recources = new List<GemColor>();

            for (int j = 0; j < liczbaZetonow; j++)
            {
                List<GemColor> playerTokens = ShowPlayerTokens();
                Console.WriteLine("Twoje żetony: \n" + listOfPlayers[currentTurn].Resources.ToString());
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
        
        private bool TakeTwoSameGems(Player player)
        {
            bool hasSufficientGems = false;
            foreach (var gem in bank.resources.gems)
            {
                if (gem.Value >= 4 && gem.Key != GemColor.GOLDEN)
                if (gem.Value >= 4 && gem.Key != GemColor.GOLDEN) 
                {
                    hasSufficientGems = true;
                    break;
                    break; 
                }
            }

@ -437,7 +382,7 @@ namespace SplendorConsole
                        {
                            colors[j] = selectedColor;
                            selectedColors.Add(selectedColor);
                            break;
                            break; 
                        }
                        else
                        {
@ -469,28 +414,6 @@ namespace SplendorConsole
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
        
        private int NumberOfPlayerTokens()
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
@ -523,7 +446,7 @@ namespace SplendorConsole
            if (player.ReservedCardsCounter >= 3)
            {
                Console.WriteLine("Nie mozna zarezerwowac wiecej kart!");
                Console.WriteLine();
                Console.WriteLine( );
                return false;
            }

@ -688,5 +611,240 @@ namespace SplendorConsole
            }

        }
        public int BuyCardOption()
        {
            int opChoice;
            while (!int.TryParse(Console.ReadLine(), out opChoice) || opChoice < 1 || opChoice > 2)
            {
                Console.WriteLine("Niepoprawny poziom. Wprowadź 1 lub 2");
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
        public GemColor ColorToBePaidWithGolden()
        {
            Console.WriteLine("Podaj kolor żetonu, zamiast którego użyjesz złotego żetonu:");
            Console.WriteLine("Opcje do wyboru: ");
            foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
            {
                if (color != GemColor.GOLDEN)
                {
                    Console.WriteLine($"- {color}");
                }
            }
            int input;
            while (true)
            {
                Console.Write("Wprowadź numer koloru: ");
                if (int.TryParse(Console.ReadLine(), out input) && input > 0 && input <= Enum.GetValues(typeof(GemColor)).Length - 1)
                    return (GemColor)(input - 1);
                else
                    Console.WriteLine("Niepoprawny wybór. Wprowadź numer odpowiadający dostępnym kolorom.");
            }
        }
        public bool BuyCardAction(Board board, Bank bank, Player player)
        {
            Console.WriteLine("Chcesz kupić nową kartę czy kupić zarezerwowaną?");
            Console.WriteLine("[1] Nowa");
            Console.WriteLine("[2] Zarerwowana");
            int opChoice = BuyCardOption();

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
            player.ReservedCards.Remove(selectedCard);

            return this.BuyCard(board, selectedCard, bank, player.BUYING_RESERVED_CARD, player);
        }
        public bool BuyCard(Board board, Card card, Bank bank, bool isBuyingReservedCard, Player player)
        {
            Console.WriteLine("Czy chcesz użyć złotego żetonu aby zapłacić za kartę?");
            Console.WriteLine("1 - Tak");
            Console.WriteLine("2 - Nie");
            var choiceForGolden = WantToSpendGoldCoin();
            GemColor colorReplacedWithGolden = GemColor.NONE;
            GemColor colorOfChoice = GemColor.NONE;
            if (choiceForGolden)
            {
                colorOfChoice = ColorToBePaidWithGolden();
                colorReplacedWithGolden = colorOfChoice;
            }

            if (!player.CanAffordCardWithGolden(card) && !player.CanAffordCard(card))
            {
                Console.WriteLine("Nie stać cię na tę kartę.");
                return false;
            }

            int cardToBuyLevel = card.Level;
            int indexInVisibleCards = board.GetCardIndexInVisibleCards(card, cardToBuyLevel);

            if (indexInVisibleCards == -1 && !isBuyingReservedCard)
            {
                Console.WriteLine("Karta nie jest widoczna na stole.");
                return false;
            }

            var cardPrice = card.DetailedPrice.gems;
            bool goldenWasUsed = false;
            foreach (var colorOnCard in cardPrice)
            {
                GemColor color = colorOnCard.Key;
                int requiredAmount = colorOnCard.Value;

                int bonusAmount = player.BonusResources.gems.TryGetValue(color, out var bonus) ? bonus : 0;
                if (bonusAmount > 0)
                {
                    int amountToUse = Math.Min(bonusAmount, requiredAmount);
                    player.BonusResources.gems[color] -= amountToUse;
                    requiredAmount -= amountToUse;
                }
                if (requiredAmount > 0)
                {
                    int playerAmount = player.Resources.gems.TryGetValue(color, out var playerR) ? playerR : 0;
                    if (playerAmount >= requiredAmount)
                    {
                        player.Resources.gems[color] -= requiredAmount;
                    }
                    else
                    {
                        int deficit = requiredAmount - playerAmount;
                        if (choiceForGolden && colorOfChoice == color)
                        {
                            int goldenTokens = player.Resources.gems.TryGetValue(GemColor.GOLDEN, out var golden) ? golden : 0;
                            if (goldenTokens >= deficit)
                            {
                                player.Resources.gems[GemColor.GOLDEN] -= deficit;
                                goldenWasUsed = true;
                                bank.AddGoldenGem();
                            }
                            else
                            {
                                Console.WriteLine("Nie masz wystarczającej liczby złotych żetonów.");
                                return false;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Nie masz wystarczającej ilości żetonów koloru {color}.");
                            return false;
                        }
                    }
                }
            }

            player.AddCardToPlayer(card);
            player.BonusResources.AddResource(card.BonusColor);
            player.Points += card.Points;

            board.ReplaceMissingCard(cardToBuyLevel, card);
            this.RefillBankResources(bank, card, colorReplacedWithGolden);
            if (isBuyingReservedCard && !goldenWasUsed)
            {
                player.Resources.gems[GemColor.GOLDEN]--;
                bank.AddGoldenGem();
            }
            Console.WriteLine($"Karta {card} została zakupiona!");
            return true;
        }
        private void RefillBankResources(Bank bank, Card card, GemColor colorReplacedWithGolden)
        {
            foreach (var x in card.DetailedPrice.gems)
            {
                int amountToReplace = x.Value;
                if (x.Key == colorReplacedWithGolden)
                {
                    amountToReplace--;
                }

                bank.AddResources(amountToReplace, x.Key);
            }
        }
    }
}
