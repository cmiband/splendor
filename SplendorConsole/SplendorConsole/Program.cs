using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using SplendorConsole;

public class Program
{
    async public static Task Main(string[] args)
    {
        WebserviceClient client = new WebserviceClient("ws://localhost:8765");
        await client.ConnectToWebsocket();
        Game? game;
        int N = 100000;
        int errorCounter = 1;
        int errorCounterLoop = 0;
        int errorCounterCollect = 0;
        int errorCounterNull = 0;
        int errorCounterBound = 0;
        int errorCounterOther = 0;
        int tieCounter = 0;
        int maxErrorGap = 0;
        int minErrorGap = N;
        int errorGap = 1;
        float turnSum = 0;
        int maxTurn = 0;
        int minTurn = N;
        float[] awards = new float[4];
        float maxAward = 0f;
        float minAward = 1f;
        float avgAward = 0f;
        float maxLoss = 0f;
        float minLoss = -1f;
        Stopwatch stopwatch = Stopwatch.StartNew();



        for (int i = 1; i <= N; i++)
        {
            game = new Game(client);
            try
            {
                (int turnsNumber, int winner, int[]? state) = game.GameStart();
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
                    Console.WriteLine($"Wystąpił błąd w grze numer {i}, błąd zapętlenia");
                    errorGap = 0;
                }
                else if (winner == -1)
                {
                    Console.WriteLine($"W grze nr {i} doszło do remisu w {turnsNumber} tur");
                    tieCounter++;
                    turnSum += turnsNumber;
                }
                else
                {
                    errorGap++;
                    Console.WriteLine($"Grę nr {i} wygrywa gracz nr {winner} w {turnsNumber} tur, zap - {errorCounterLoop}, col - {errorCounterCollect}, idx - {errorCounterBound}, null - {errorCounterNull}, ??? - {errorCounterOther}");
                    turnSum += turnsNumber;
                    if(turnsNumber>maxTurn)
                    {
                        maxTurn = turnsNumber;
                    }
                    if(turnsNumber<minTurn)
                    {
                        minTurn = turnsNumber;
                    }
                    awards = AwardsAfterGame(winner, state, turnsNumber);
                    Console.WriteLine($"Nagrody dla poszczególnych graczy: {awards[0]}, {awards[1]}, {awards[2]}, {awards[3]}");
                    foreach (var item in awards)
                    {
                        avgAward += item;
                        if(item>0)
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
            catch (Exception e)
            {
                if(e.Message=="Collection was modified; enumeration operation may not execute.")
                {
                    errorCounterCollect++;
                    Console.WriteLine($"Wystąpił błąd w grze numer {i}, błąd kolekcji");
                }
                else if(e.Message=="Index was outside the bounds of the array.")
                {
                    errorCounterBound++;
                    Console.WriteLine($"Wystąpił błąd w grze numer {i}, błąd idx");
                }
                else if(e.Message== "Nie działa try, catch :/")
                {
                    errorCounterNull++;
                    Console.WriteLine($"Wystąpił błąd w grze numer {i}, błąd null");
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
                game = null;
            }
        }
    stopwatch.Stop();
    if(errorCounter!=1)
    {
            errorCounter--;
    }
        Console.WriteLine($"Cała pętla zakończona w czasie: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Średni czas symulacji jednej rozgrywki: {stopwatch.ElapsedMilliseconds / N} ms");
        Console.WriteLine($"Liczba remisów = {tieCounter}, maxErrorGap = {maxErrorGap}, minErrorGap = {minErrorGap}, AvgGap = {N / errorCounter}");
        Console.WriteLine($"Tury: min = {minTurn}, max = {maxTurn} avg = {turnSum/(N-errorCounter+1)}");
        Console.WriteLine($"Nagrody: min = {minAward}, max = {maxAward}");
        Console.WriteLine($"Kary: min = {minLoss}, max = {maxLoss}");
    }

    public static float AwardWinner(int advantage, int tokensCount, int moves)
    {
        int reward = 0;

        if(moves < 20)
        {
            reward += 75;
        }
        else if (moves < 25)
        {
            reward += 65;
        }
        else if(moves < 30)
        {
            reward += 60;
        }
        else if (moves < 35)
        {
            reward += 55;
        }
        else if(moves < 40)
        {
            reward += 50;
        }
        else
        {
            reward += 30;
        }
        if(advantage>=5)
        {
            reward += 20;
        }
        if(tokensCount>=5)
        {
            reward -= 10;
        }

        return reward / (float)100;
    }

    public static float AwardLossWinner(int[] arr, int moveNumber)
    {
        int tokensSum = arr[174] + arr[175] + arr[176] + arr[177] + arr[178] + arr[179];
        int advantage = ((arr[168] - arr[213]) + (arr[168] - arr[258]) + (arr[168] - arr[303])) / 3;

        return AwardWinner(advantage, tokensSum, moveNumber);
    }

    public static float AwardLossLoser(int advantage, int tokensCount, int moves)
    {
        int reward = -50;
        if (advantage >= 5)
        {
            reward -= 20;
        }
        if (tokensCount >= 5)
        {
            reward -= 5;
        }
        return reward/ (float)100;
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
        rewards[(winner+1)%4] = AwardLossP1(stateFromWinnerPerspective, moveNumber);
        rewards[(winner+2)%4] = AwardLossP2(stateFromWinnerPerspective, moveNumber);
        rewards[(winner+3)%4] = AwardLossP3(stateFromWinnerPerspective, moveNumber);

        return rewards;
    }

}