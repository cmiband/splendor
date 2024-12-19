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

        Stopwatch stopwatch = Stopwatch.StartNew();



        for (int i = 0; i < N; i++)
        {
            game = new Game(client);
            try
            {
                (int turnsNumber, int winner, float[]? state) = game.GameStart();
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
    Console.WriteLine($"Liczba remisów - {tieCounter}, maxErrorGap - {maxErrorGap}, minErrorGap - {minErrorGap}, AvgGap - {N / errorCounter}, AvgTurn - {turnSum/(N-errorCounter+1)}");
    }

    public static float AwardWinner(float advantage, float tokensCount, int moves)
    {
        float reward = 0;

        if(moves<20)
        {
            reward += 0.75f;
        }
        else if(moves<=40)
        {
            reward += 0.5f;
        }
        else
        {
            reward += 0.3f;
        }
        if(advantage>=5)
        {
            reward += 0.2f;
        }
        if(tokensCount>=5)
        {
            reward -= 0.1f;
        }

        return reward;
    }

    public static float AwardLossWinner(float[] arr, int moveNumber)
    {
        float tokensSum = arr[174] + arr[175] + arr[176] + arr[177] + arr[178] + arr[179];
        float advantage = ((arr[168] - arr[213]) + (arr[168] - arr[258]) + (arr[168] - arr[303])) / 3;

        return AwardWinner(advantage, tokensSum, moveNumber);
    }

    public static float AwardLossLoser(float advantage, float tokensCount, int moves)
    {
        float reward = -0.5f;
        if(advantage>=5)
        {
            reward -= 0.2f;
        }
        if(tokensCount>=5)
        {
            reward = -0.1f;
        }

        return reward;
    }

    public static float AwardLossP1(float[] arr, int moveNumber)
    {
        float tokensSum = arr[219] + arr[220] + arr[221] + arr[222] + arr[223] + arr[224];
        float advantage = arr[168] - arr[213];

        return AwardLossLoser(-advantage, tokensSum, moveNumber);
    }

    public static float AwardLossP2(float[] arr, int moveNumber)
    {
        float tokensSum = arr[265] + arr[266] + arr[267] + arr[268] + arr[269] + arr[264];
        float advantage = arr[168] - arr[258];

        return AwardLossLoser(-advantage, tokensSum, moveNumber);
    }

    public static float AwardLossP3(float[] arr, int moveNumber)
    {
        float tokensSum = arr[309] + arr[310] + arr[311] + arr[312] + arr[313] + arr[314];
        float advantage = arr[168] - arr[303];

        return AwardLossLoser(-advantage, tokensSum, moveNumber);
    }

    public static float[] AwardsAfterGame(int winner, float[] stateFromWinnerPerspective, int moveNumber)
    {
        float[] rewards = new float[4];

        rewards[winner] = AwardLossWinner(stateFromWinnerPerspective, moveNumber);

        if (winner == 0)
        {
            rewards[1] = AwardLossP1(stateFromWinnerPerspective, moveNumber);
            rewards[2] = AwardLossP2(stateFromWinnerPerspective, moveNumber);
            rewards[3] = AwardLossP3(stateFromWinnerPerspective, moveNumber);
        }
        else if (winner == 1)
        {
            rewards[2] = AwardLossP1(stateFromWinnerPerspective, moveNumber);
            rewards[3] = AwardLossP2(stateFromWinnerPerspective, moveNumber);
            rewards[0] = AwardLossP3(stateFromWinnerPerspective, moveNumber);
        }
        else if (winner == 2)
        {
            rewards[3] = AwardLossP1(stateFromWinnerPerspective, moveNumber);
            rewards[0] = AwardLossP2(stateFromWinnerPerspective, moveNumber);
            rewards[1] = AwardLossP3(stateFromWinnerPerspective, moveNumber);
        }
        else if (winner == 3)
        {
            rewards[0] = AwardLossP1(stateFromWinnerPerspective, moveNumber);
            rewards[1] = AwardLossP1(stateFromWinnerPerspective, moveNumber);
            rewards[2] = AwardLossP1(stateFromWinnerPerspective, moveNumber);
        }

        return rewards;
    }

}