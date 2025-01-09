using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SplendorConsole;
using System.Globalization;

public class Program
{

    public const bool EXTENDED_LOGGER_MODE = false;
    static WebserviceClient client = new WebserviceClient("ws://localhost:8765");

    async public static Task Main(string[] args)
    {

        await client.ConnectToWebsocket();
        Game? game;

        int N = 300;

        int errorCounter = 1;
        int errorCounterLoop = 0;
        int errorCounterCollect = 0;
        int errorCounterNull = 0;
        int errorCounterBound = 0;
        int errorCounterOther = 0;
        int errorGameBreak = 0;
        int errorLossAboveZero = 0;
        int tieCounter = 0;
        int maxErrorGap = 0;
        int minErrorGap = N;
        int errorGap = 1;
        float turnSum = 0;
        int maxTurn = 0;
        int minTurn = int.MaxValue;
        float[] awards = new float[4];
        float maxAward = 0f;
        float minAward = 1f;
        float avgAward = 0f;
        float maxLoss = 0f;
        float minLoss = -1f;
        int sumTurns = 0;
        int sumuTurnsAfterAll = 0;
        //tutaj zmieniasz gapa zapisu, jak coś
        int intSaveGap = 200;
        float floatSaveGap = intSaveGap;
        float lowestAvgRounds = 32f;
        Stopwatch stopwatch = Stopwatch.StartNew();

        string filePath = "game_results.csv";
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Zapisz nagłówki pliku CSV
            writer.WriteLine("GameIndex;AvgTurns");
        }


        for (int i = 1; i <= N; i++)
        {
            game = new Game(client);
            Console.WriteLine();
            try
            {
                (float lastFeedback, int turnsNumber, int winner, int[]? state, int lastMove) =game.GameStart();
                if (winner == -200)
                {
                    errorCounterLoop++;
                    if (maxErrorGap <= errorGap)
                    {
                        maxErrorGap = errorGap;
                    }
                    if (minErrorGap >= errorGap)
                    {
                        minErrorGap = errorGap;
                    }
                    errorCounter++;
                    Console.WriteLine($"[C#] Wystąpił błąd w grze numer {i}, błąd zapętlenia");
                    errorGap = 0;
                    awards = new float[] { 0, 0, 0, 0 };
                    sumTurns += (int)lowestAvgRounds;
                    if (i % intSaveGap == 0)
                    {
                        // Dodane zapisywanie do pliku CSV
                        float avgTurns = sumTurns / floatSaveGap;
                        using (StreamWriter writer = new StreamWriter(filePath, append: true))
                        {
                            writer.WriteLine($"{i.ToString(CultureInfo.InvariantCulture)};{avgTurns.ToString(CultureInfo.InvariantCulture)}");
                        }
                        sumTurns = 0;
                        if (avgTurns < lowestAvgRounds)
                        {
                            await InformServerAboutFinishedGame(awards, -2, 0, 0, game.Standartize(game.PlayZeroToArray()));
                            lowestAvgRounds = avgTurns; 
                        }
                        else
                        {
                            await InformServerAboutFinishedGame(awards, -1, 0, 0, game.Standartize(game.PlayZeroToArray()));
                        }
                    }
                    else
                    {
                        await InformServerAboutFinishedGame(awards, -1, 0, 0, game.Standartize(game.PlayZeroToArray()));
                    }
                }
                else if (winner == -1)
                {
                    Console.WriteLine($"[C#] W grze nr {i} doszło do remisu w {turnsNumber} tur");
                    tieCounter++;
                    sumTurns += turnsNumber;
                    sumuTurnsAfterAll += turnsNumber;
                    awards = new float[] { 0, 0, 0, 0 };
                    if (i % intSaveGap == 0)
                    {
                        // Dodane zapisywanie do pliku CSV
                        float avgRounds = sumTurns / floatSaveGap;
                        using (StreamWriter writer = new StreamWriter(filePath, append: true))
                        {
                            writer.WriteLine($"{i.ToString(CultureInfo.InvariantCulture)};{avgRounds.ToString(CultureInfo.InvariantCulture)}");
                        }
                        sumTurns = 0;
                        if (avgRounds < lowestAvgRounds)
                        {
                            await InformServerAboutFinishedGame(awards, -2, 0, 0, game.Standartize(game.PlayZeroToArray()));
                            lowestAvgRounds = avgRounds;
                        }
                        else
                        {
                            await InformServerAboutFinishedGame(awards, -1, 0, 0, game.Standartize(game.PlayZeroToArray()));
                        }
                    }
                    else
                    {
                        await InformServerAboutFinishedGame(awards, -1, 0, 0, game.Standartize(game.PlayZeroToArray()));
                    }
                }
                else
                {
                    errorGap++;
                    Console.WriteLine($"[C#] Grę nr {i} wygrywa gracz nr {winner} w {turnsNumber} tur, zap - {errorCounterLoop}, col - {errorCounterCollect}, idx - {errorCounterBound}, null - {errorCounterNull}, Game Break - {errorGameBreak}, Award Error - {errorLossAboveZero}, ??? - {errorCounterOther}");
                    if (turnsNumber > maxTurn)
                    {
                        maxTurn = turnsNumber;
                    }
                    if (turnsNumber < minTurn)
                    {
                        minTurn = turnsNumber;
                    }
                    sumTurns += turnsNumber;
                    sumuTurnsAfterAll += turnsNumber;
                    awards = AwardsAfterGame(winner, state, turnsNumber);
                    if (i % intSaveGap == 0)
                    {
                        // Dodane zapisywanie do pliku CSV
                        float avgTurns = sumTurns / floatSaveGap;
                        using (StreamWriter writer = new StreamWriter(filePath, append: true))
                        {
                            writer.WriteLine($"{i.ToString(CultureInfo.InvariantCulture)};{avgTurns.ToString(CultureInfo.InvariantCulture)}");
                        }
                        sumTurns = 0;
                        if (avgTurns < lowestAvgRounds)
                        {
                            await InformServerAboutFinishedGame(awards, winner + 4, lastFeedback, lastMove, game.Standartize(game.PlayZeroToArray()));
                            lowestAvgRounds = avgTurns;
                        }
                        else
                        {
                            await InformServerAboutFinishedGame(awards, winner, lastFeedback, lastMove, game.Standartize(game.PlayZeroToArray()));
                        }
                    }
                    else
                    {
                        await InformServerAboutFinishedGame(awards, winner, lastFeedback, lastMove, game.Standartize(game.PlayZeroToArray()));
                    }

                    Console.WriteLine($"[C#] Nagrody dla poszczególnych graczy: {awards[0]}, {awards[1]}, {awards[2]}, {awards[3]}");
                    foreach (var item in awards)
                    {
                        avgAward += item;
                        if (item > 0)
                        {
                            if (item > maxAward)
                            {
                                maxAward = item;
                            }
                            if (item < minAward)
                            {
                                minAward = item;
                            }
                        }
                        else
                        {
                            if (item < maxLoss)
                            {
                                maxLoss = item;
                            }
                            if (item > minLoss)
                            {
                                minLoss = item;
                            }
                        }
                    }
                }
                game = null;
            }
            catch (TimeoutException)
            {
                Console.WriteLine($"[C#] W grze nr {i} przekroczono limit czasu.");
                errorCounterOther++;
                awards = new float[] { 0, 0, 0, 0 };
                sumTurns += (int)lowestAvgRounds;
                if (i % intSaveGap == 0)
                {
                    // Dodane zapisywanie do pliku CSV
                    float avgTurns = sumTurns / floatSaveGap;
                    using (StreamWriter writer = new StreamWriter(filePath, append: true))
                    {
                        writer.WriteLine($"{i.ToString(CultureInfo.InvariantCulture)};{avgTurns.ToString(CultureInfo.InvariantCulture)}");
                    }
                    sumTurns = 0;
                    if (avgTurns < lowestAvgRounds)
                    {
                        await InformServerAboutFinishedGame(awards, -2, 0, 0, game.Standartize(game.PlayZeroToArray()));
                        lowestAvgRounds = avgTurns;
                    }
                    else
                    {
                        await InformServerAboutFinishedGame(awards, -1, 0, 0, game.Standartize(game.PlayZeroToArray()));
                    }
                }
                else
                {
                    await InformServerAboutFinishedGame(awards, -1, 0, 0, game.Standartize(game.PlayZeroToArray()));
                }

                game = null;
            }
            catch (Exception e)
            {
                if (e.Message == "Collection was modified; enumeration operation may not execute.")
                {
                    errorCounterCollect++;
                    Console.WriteLine($"[C#] Wystąpił błąd w grze numer {i}, błąd kolekcji");
                }
                else if (e.Message == "Index was outside the bounds of the array.")
                {
                    errorCounterBound++;
                    Console.WriteLine($"[C#] Wystąpił błąd w grze numer {i}, błąd idx");
                }
                else if (e.Message == "Nie działa try, catch :/")
                {
                    errorCounterNull++;
                    Console.WriteLine($"[C#] Wystąpił błąd w grze numer {i}, błąd null");
                }
                else if(e.Message == "Game break ;[")
                {
                    errorGameBreak++;
                    Console.WriteLine($"[C#] Wystąpił błąd w grze numer {i}, GameBreak");
                }
                else if(e.Message == "Kara większa niż 0")
                {
                    errorLossAboveZero++;
                    Console.WriteLine($"[C#] Wystąpił błąd w grze numer {i}, błąd kary");
                }
                else
                {
                    Console.WriteLine(e.Message);
                    errorCounterOther++;
                }
                if (maxErrorGap <= errorGap)
                {
                    maxErrorGap = errorGap;
                }
                if (minErrorGap >= errorGap)
                {
                    minErrorGap = errorGap;
                }
                errorCounter++;
                errorGap = 0;
                awards = new float[] { 0, 0, 0, 0 };
                sumTurns += (int)lowestAvgRounds;
                if (i % intSaveGap == 0)
                {
                    // Dodane zapisywanie do pliku CSV
                    float avgTurns = sumTurns / floatSaveGap;
                    using (StreamWriter writer = new StreamWriter(filePath, append: true))
                    {
                        writer.WriteLine($"{i.ToString(CultureInfo.InvariantCulture)};{avgTurns.ToString(CultureInfo.InvariantCulture)}");
                    }
                    sumTurns = 0;
                    if (avgTurns < lowestAvgRounds)
                    {
                        await InformServerAboutFinishedGame(awards, -2, 0, 0, game.Standartize(game.PlayZeroToArray()));
                        lowestAvgRounds = avgTurns;
                    }
                    else
                    {
                        await InformServerAboutFinishedGame(awards, -1, 0, 0, game.Standartize(game.PlayZeroToArray()));
                    }
                }
                else
                {
                    await InformServerAboutFinishedGame(awards, -1, 0, 0, game.Standartize(game.PlayZeroToArray()));
                }
                game = null;

                
            }
        }

        stopwatch.Stop();
        if (errorCounter != 1)
        {
            errorCounter--;
        }

        Console.WriteLine($"\n[C# Summary]\nCała pętla zakończona w czasie: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Średni czas symulacji jednej rozgrywki: {stopwatch.ElapsedMilliseconds / N} ms");
        Console.WriteLine($"Liczba remisów = {tieCounter}, maxErrorGap = {maxErrorGap}, minErrorGap = {minErrorGap}, AvgGap = {N / errorCounter}");
        Console.WriteLine($"Tury: min = {minTurn}, max = {maxTurn}");
        Console.WriteLine($"Nagrody: min = {minAward}, max = {maxAward}");
        Console.WriteLine($"Kary: min = {minLoss}, max = {maxLoss}");
        Console.WriteLine($"Średni czas rozgrywki: {sumuTurnsAfterAll *1f / N}");
        Console.WriteLine();
        using (StreamWriter writer = new StreamWriter(filePath, append: true))
        {
            writer.WriteLine($"Overall;{(sumuTurnsAfterAll * 1f / N).ToString(CultureInfo.InvariantCulture)}");
        }
        Console.WriteLine();
    }

    public static float AwardWinner(int advantage, int tokensCount, int moves)
    {
        float reward;

        if (moves <= 21)
        {
            reward = 0.995f; // Maksymalna nagroda
        }
        else if (moves >= 35)
        {
            reward = 0.025f; // Minimalna nagroda
        }
        else
        {
            // Logarytmiczna zależność skalowana do zakresu 0.025 - 0.995
            float normalizedMoves = (moves - 21f) / 13f; // Normalizacja moves na przedział [0, 1]
            reward = 1.025f - (float)(Math.Log10(1 + 9 *  normalizedMoves) * 0.97f);
        }

        return reward;
    }

    public static float AwardLossLoser(int advantage, int tokensCount, int moves)
    {
        float penalty;

        if (advantage <= 0)
        {
            penalty = -0.005f; // Minimalna kara
        }
        else if (advantage >= 15)
        {
            penalty = -0.975f; // Maksymalna kara
        }
        else
        {
            // Logarytmiczna zależność skalowana do zakresu -0.005 - -0.975
            float normalizedAdvantage = advantage / 15f; // Normalizacja advantage na przedział [0, 1]
            penalty = -0.005f - (float)(Math.Log10(1 + 9 * normalizedAdvantage) * 0.97f);
        }

        if (penalty > 0)
        {
            throw new ArgumentException("Kara większa niż 0");
        }

        return penalty;
    }
    public static float AwardLossWinner(int[] arr, int moveNumber)
    {
        int tokensSum = arr[174] + arr[175] + arr[176] + arr[177] + arr[178] + arr[179];
        int advantage = ((arr[168] - arr[213]) + (arr[168] - arr[258]) + (arr[168] - arr[303])) / 3;

        return AwardWinner(advantage, tokensSum, moveNumber);
    }

    public static float AwardLossP1(int[] arr, int moveNumber)
    {
        int tokensSum = arr[219] + arr[220] + arr[221] + arr[222] + arr[223] + arr[224];
        int advantage = arr[168] - arr[213];

        return AwardLossLoser(advantage, tokensSum, moveNumber);
    }

    public static float AwardLossP2(int[] arr, int moveNumber)
    {
        int tokensSum = arr[265] + arr[266] + arr[267] + arr[268] + arr[269] + arr[264];
        int advantage = arr[168] - arr[258];

        return AwardLossLoser(advantage, tokensSum, moveNumber);
    }

    public static float AwardLossP3(int[] arr, int moveNumber)
    {
        int tokensSum = arr[309] + arr[310] + arr[311] + arr[312] + arr[313] + arr[314];
        int advantage = arr[168] - arr[303];

        return AwardLossLoser(advantage, tokensSum, moveNumber);
    }

    public static float[] AwardsAfterGame(int winner, int[] stateFromWinnerPerspective, int moveNumber)
    {
        float[] rewards = new float[4];

        rewards[winner] = AwardLossWinner(stateFromWinnerPerspective, moveNumber);
        rewards[(winner + 1) % 4] = AwardLossP1(stateFromWinnerPerspective, moveNumber);
        rewards[(winner + 2) % 4] = AwardLossP2(stateFromWinnerPerspective, moveNumber);
        rewards[(winner + 3) % 4] = AwardLossP3(stateFromWinnerPerspective, moveNumber);

        return rewards;
    }

    async public static Task InformServerAboutFinishedGame(float[] rewards, int winner, float lastFeedback, int lastMove, float[] lastGameState)
    {
        var request = new
        {
            Id = (winner >= 0) ? 2 : -1,
            Winner = winner,
            LastFeedback = lastFeedback,
            Rewards = rewards,
            LastMove = lastMove,
            LastGameState = lastGameState
        };

        await client.SendAndFetchDataFromSocket(JsonSerializer.Serialize(request));
    }
    private static (float, int, int, int[]?) ExecuteWithTimeout(Func<(float, int, int, int[]?)> func, int timeoutMilliseconds)
    {
        (float, int, int, int[]?) result = (0f, 0, -1, null);
        Exception? exception = null;

        Thread thread = new Thread(() =>
        {
            try
            {
                result = func();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        });

        thread.Start();
        bool completed = thread.Join(timeoutMilliseconds);

        if (!completed)
        {
            thread.Interrupt();
            throw new TimeoutException("Funkcja przekroczyła limit czasu.");
        }

        if (exception != null)
            throw exception;

        return result;
    }
}