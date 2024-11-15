using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
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
        System.Random random = new System.Random();
        listOfPlayers = SetNumberOfPlayers();
        listOfNobles = SetNumberOfNobles(listOfPlayers.Count);

        level1Shuffled = Shuffling(availableCards.level1Cards, random);
        level2Shuffled = Shuffling(availableCards.level2Cards, random);
        level3Shuffled = Shuffling(availableCards.level3Cards, random);


        AddResourcesToBank(bank, listOfPlayers.Count);
        SetVisibleCards();
        board = new Board(level1VisibleCards, level2VisibleCards, level3VisibleCards, level1Shuffled, level2Shuffled, level3Shuffled);
        GameLoop(listOfPlayers.Count);
    }

    private List<Noble> SetNumberOfNobles(int numberOfPlayers)
    {
        int numberOfNobles = numberOfPlayers + 1;
        List<Noble> nobles = new List<Noble>();

        for (int i = 0; i < numberOfNobles; i++)
        {
            nobles.Add(new Noble());
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

    private void GameLoop(int numberOfPlayers)
    {
        bool gameInProgress = true;
        while (gameInProgress)
        {
            Console.WriteLine($"-----------------Aktualna kolejka nale¿y do gracza {currentTurn}-----------------------");
            Turn(listOfPlayers[currentTurn]);

            // wiêcej logiki GameLoopa
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
                    Console.WriteLine($"Zwyciêzca to gracz: {listOfPlayers.IndexOf(winners[0])}");
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
                        Console.WriteLine($"Zwyciêzca to gracz: {playerIndex}");
                        Console.WriteLine($"Jego liczba punktów to: {listOfPlayers[playerIndex].Points}");
                    }
                    else
                    {
                        Player OfficialWinner = MoreThan1Winner(winners);
                        if (OfficialWinner != null)
                        {
                            Console.WriteLine($"Zwyciêzca to gracz: {listOfPlayers.IndexOf(OfficialWinner)}");
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
            Console.WriteLine("=== Wybierz akcjê do wykonania ===");
            Console.WriteLine("1. WeŸ 3 klejnoty ró¿nych kolorów");
            Console.WriteLine("2. WeŸ 2 klejnoty tego samego koloru");
            Console.WriteLine("3. Zarezerwuj kartê i weŸ z³oty klejnot");
            Console.WriteLine("4. Kup kartê(now¹ lub zarezerwowan¹)");
            Console.WriteLine("5. Spasuj");
            Console.WriteLine("======================================================================");
            Console.WriteLine("Twoje ¿etony: " + listOfPlayers[currentTurn].Resources.ToString());
            Console.WriteLine("Twoje surowce z kopalñ: " + listOfPlayers[currentTurn].BonusResources.ToString());
            Console.WriteLine("Twoje zakupione karty: " + listOfPlayers[currentTurn].handToString());
            Console.WriteLine("Punkty zwyciêstwa: " + listOfPlayers[currentTurn].Points);
            Console.WriteLine("Twoi arystokraci: " + listOfPlayers[currentTurn].nobleToString());
            Console.WriteLine("======================================================================");
            Console.Write("WprowadŸ numer akcji (1-5): ");

            while (!int.TryParse(Console.ReadLine(), out input) || input < 1 || input > 5)
            {
                Console.Write("Niepoprawny wybór. WprowadŸ numer akcji (1-5): ");
            }


            actionSuccess = false;

            switch (input)
            {
                case 1:
                    actionSuccess = TakeThreeDifferentGems(player);
                    break;

                case 2:
                    actionSuccess = TakeTwoSameGems(player);
                    break;

                case 3:
                    actionSuccess = ReserveCard(player);
                    break;

                case 4:
                    actionSuccess = player.BuyCardAction(this.board, this.bank);
                    break;

                case 5:
                    Pass();
                    actionSuccess = true;
                    break;
            }
        } while (!actionSuccess);
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

        if (counter > 3) hasSufficientGems = true;

        if (!hasSufficientGems)
        {
            Console.WriteLine("Brak wystarczaj¹cych klejnotów w banku. Wybierz inn¹ akcjê.");
            return false;
        }

        GemColor[] colors = ChoiceOfColors();
        player.TakeThreeTokens(bank.resources, colors);
        for (int i = 0; i < 3; i++)
        {
            bank.TakeOutResources(1, colors[i]);
        }
        return true;
    }

    private bool TakeTwoSameGems(Player player)
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
            Console.WriteLine("Brak wystarczaj¹cych klejnotów w banku. Wybierz inn¹ akcjê.");
            return false;
        }

        GemColor color = ChoiceOfColor();
        if (bank.resources.gems[color] < 4)
        {
            Console.WriteLine($"Brak wystarczaj¹cej iloœci klejnotów koloru {color} na planszy, wybierz inn¹ akcjê.");
            return false;
        }

        if (bank.resources.gems.Count < 2)
        {
            Console.WriteLine("Brak wystarczaj¹cej iloœci klejnotów na planszy, wybierz inn¹ akcjê.");
            return false;
        }

        player.TakeTwoTokens(bank.resources, color);
        bank.TakeOutResources(2, color);
        return true;
    }


    private GemColor ChoiceOfColor()
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
            Console.Write("WprowadŸ numer koloru: ");
            if (int.TryParse(Console.ReadLine(), out input) && input >= 1 && input <= availableTokens.Count)
            {
                color = availableTokens[input - 1];
                return color;
            }
            else
            {
                Console.WriteLine("Niepoprawny wybór. WprowadŸ numer odpowiadaj¹cy dostêpnym kolorom.");
            }
        }
    }


    private GemColor[] ChoiceOfColors()
    {
        List<GemColor> availableTokens = ShowAvaiableTokens();
        GemColor[] colors = new GemColor[3];

        int i = 1;
        Console.WriteLine("=== Wybierz kolory (3 ró¿ne) === ");
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
                Console.Write($"WprowadŸ numer koloru {j + 1}: ");
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
                        Console.WriteLine("Ju¿ wybra³eœ ten kolor. Wybierz inny.");
                    }
                }
                else
                {
                    Console.WriteLine("Niepoprawny wybór. WprowadŸ numer odpowiadaj¹cy dostêpnym kolorom.");
                }
            }
        }

        return colors;
    }


    private List<GemColor> ShowAvaiableTokens()
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

    private List<Card> Shuffling(List<Card> deck, System.Random random)
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

        Console.WriteLine("=== Wybierz metodê rezerwowania ===");
        Console.WriteLine("1. Rezerwuj kartê ze stolika");
        Console.WriteLine("2. Rezerwuj kartê w ciemno ze stosu");
        int reserveinput;
        while (!int.TryParse(Console.ReadLine(), out reserveinput) || reserveinput < 1 || reserveinput > 2)
        {
            Console.Write("Niepoprawny wybór. WprowadŸ numer akcji (1-2): ");
        }

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
        }

        if (reserveinput == 1)
        {
            Console.WriteLine("=== Wybierz którego poziomu kartê chcesz zarezerwowaæ ===");
            Console.WriteLine("1 poziom");
            Console.WriteLine("2 poziom");
            Console.WriteLine("3 poziom");
            int cardLevel;
            while (!int.TryParse(Console.ReadLine(), out cardLevel) || cardLevel < 1 || cardLevel > 3)
            {
                Console.Write("Niepoprawny wybór. WprowadŸ numer akcji (1-3): ");
            }
            int input;
            Console.WriteLine("=== Wybierz kartê do zarezerwowania ===");
            Card[] cardsOnTable = VisibleCardsOnTable(cardLevel);
            while (!int.TryParse(Console.ReadLine(), out input) || input < 1 || input > 4)
            {
                Console.Write("Niepoprawny wybór. WprowadŸ numer akcji (1-4): ");
            }
            player.ReserveCard(cardsOnTable[input - 1]);
            board.ReplaceMissingCard(cardLevel, cardsOnTable[input - 1]);
        }
        else
        {
            Console.WriteLine("=== Wybierz którego poziomu kartê chcesz zarezerwowaæ ===");
            Console.WriteLine("1 poziom");
            Console.WriteLine("2 poziom");
            Console.WriteLine("3 poziom");
            int cardLevel;
            while (!int.TryParse(Console.ReadLine(), out cardLevel) || cardLevel < 1 || cardLevel > 3)
            {
                Console.Write("Niepoprawny wybór. WprowadŸ numer akcji (1-3): ");
            }
            switch (cardLevel)
            {
                case 1:
                    System.Random randomLevel1Card = new System.Random();
                    Card level1CardToReserve = level1Shuffled[randomLevel1Card.Next(level1Shuffled.Count)];
                    player.ReserveCard(level1CardToReserve);
                    level1Shuffled.Remove(level1CardToReserve);
                    break;
                case 2:
                    System.Random randomLevel2Card = new System.Random();
                    Card level2CardToReserve = level2Shuffled[randomLevel2Card.Next(level2Shuffled.Count)];
                    player.ReserveCard(level2CardToReserve);
                    level2Shuffled.Remove(level2CardToReserve);
                    break;
                case 3:
                    System.Random randomLevel3Card = new System.Random();
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


    public void GettingNobles()
    {
        if (listOfPlayers[currentTurn].CanGetMultipleNobles() == false)
        {
            foreach (Noble noble in Board.VisibleNobles)
                if (listOfPlayers[currentTurn].CanGetNoble(noble))
                    listOfPlayers[currentTurn].GetNoble(noble);
        }
        else
        {
            List<int> AvailableIndexNobles = new List<int>();
            for (int i = 0; i < Board.VisibleNobles.Length; i++)
            {
                Noble noble = Board.VisibleNobles[i];
                if (listOfPlayers[currentTurn].CanGetNoble(noble))
                    AvailableIndexNobles.Add(i);
            }

            Console.WriteLine("Arystokraci, których mo¿esz zdobyæ: ");
            for (int i = 0; i < AvailableIndexNobles.Count; i++)
                Console.WriteLine(AvailableIndexNobles[i]);


            bool IsChoiceMade = false;
            int choice = 0;
            while (IsChoiceMade == false)
            {
                try
                {
                    Console.WriteLine("Wybierz arystokratê: ");
                    choice = int.Parse(Console.ReadLine());
                    IsChoiceMade = true;
                }
                catch
                {
                    Console.WriteLine("Niepoprawny numer, podaj jeszcze raz");
                }
            }

            Noble playerChoice = Board.VisibleNobles[choice];
            listOfPlayers[currentTurn].GetNoble(playerChoice);

        }

    }
}
