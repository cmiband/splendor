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
        int N = 200;
        int errorCounter = 1;
        int errorCounterLoop = 0;
        int errorCounterCollect = 0;
        int errorCounterNull = 0;
        int errorCounterBound = 0;
        int errorCounterOther = 0;
        int tieCounter = 0;
        int maxErrorGap = 0;
        int minErrorGap = N;
        int errorGap = 0;

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
                    errorGap = 0;
                }
                else
                {
                    errorGap++;
                }
                Console.WriteLine($"{i}tą grę wygrywa {winner}, błąd zepętlenia - {errorCounterLoop}, błąd kolekcji - {errorCounterCollect}, błąd indeksu - {errorCounterBound}, błąd null - {errorCounterNull}, nieobsługiwany błąd - {errorCounterOther}");
                if (winner == -1)
                {
                    tieCounter++;
                }
                game = null;
            }
            catch (Exception e)
            {
                if(e.Message=="Collection was modified; enumeration operation may not execute.")
                {
                    errorCounterCollect++;
                }
                else if(e.Message=="Index was outside the bounds of the array.")
                {
                    errorCounterBound++;
                }
                else if(e.Message== "Nie działa try, catch :/")
                {
                    errorCounterNull++;
                }
                else
                {
                    errorCounterOther++;
                }
                int winner = -3000;
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
                Console.WriteLine($"{i}tą grę wygrywa {winner}, błąd zepętlenia - {errorCounterLoop}, błąd kolekcji - {errorCounterCollect}, błąd indeksu - {errorCounterBound}, błąd null - {errorCounterNull}, nieobsługiwany błąd - {errorCounterOther}");
                Console.WriteLine(e.Message);
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
    Console.WriteLine($"Liczba remisów - {tieCounter}, maxErrorGap - {maxErrorGap}, minErrorGap - {minErrorGap}, AvgGap - {N / errorCounter}");
    }

    public static float AwardWinner(float przewaga, float zetony_liczba, int ruchy)
    {
        float reward = 0;

        reward += (float)100;
        reward -= (float)(0.65 * (ruchy - 30));
        reward += (float)0.8 * przewaga;
        reward -= (float)0.25 * zetony_liczba;

        return reward;
    }
    public static float AwardLossWinner(float[] arr, int numer_ruchu)
    {
        float zetony_suma = arr[174] + arr[175] + arr[176] + arr[177] + arr[178] + arr[179];
        float przewaga = ((arr[168] - arr[213]) + (arr[168] - arr[258]) + (arr[168] - arr[303])) / 3;

        return AwardWinner(przewaga, zetony_suma, numer_ruchu);
    }
    public static float AwardLossLoser(float przewaga, float zetony_liczba, int ruchy)
    {
        float reward = 0;

        reward -= (float)70;
        reward -= (float)0.65 * (ruchy - 30);
        reward += (float)0.8 * przewaga;
        reward += (float)0.25 * zetony_liczba;

        return reward;
    }

    public static float AwardLossP1(float[] arr, int numer_ruchu)
    {
        float zetony_suma = arr[219] + arr[220] + arr[221] + arr[222] + arr[223] + arr[224];
        float przewaga = arr[168] - arr[213];

        return AwardLossLoser(-przewaga, zetony_suma, numer_ruchu);
    }
    public static float AwardLossP2(float[] arr, int numer_ruchu)
    {
        float zetony_suma = arr[265] + arr[266] + arr[267] + arr[268] + arr[269] + arr[264];
        float przewaga = arr[168] - arr[258];

        return AwardLossLoser(-przewaga, zetony_suma, numer_ruchu);
    }
    public static float AwardLossP3(float[] arr, int numer_ruchu)
    {
        float zetony_suma = arr[309] + arr[310] + arr[311] + arr[312] + arr[313] + arr[314];
        float przewaga = arr[168] - arr[303];

        return AwardLossLoser(-przewaga, zetony_suma, numer_ruchu);
    }

    static public float[] AwardsAfterGame(int winner, float[] stateZPerspektywyWinnera, int numer_ruchu)
    {
        float[] tab = new float[4];

        tab[winner] = AwardLossWinner(stateZPerspektywyWinnera, numer_ruchu);

        if (winner == 0)
        {
            tab[1] = AwardLossP1(stateZPerspektywyWinnera, numer_ruchu);
            tab[2] = AwardLossP2(stateZPerspektywyWinnera, numer_ruchu);
            tab[3] = AwardLossP3(stateZPerspektywyWinnera, numer_ruchu);
        }
        else if (winner == 1)
        {
            tab[2] = AwardLossP1(stateZPerspektywyWinnera, numer_ruchu);
            tab[3] = AwardLossP2(stateZPerspektywyWinnera, numer_ruchu);
            tab[0] = AwardLossP3(stateZPerspektywyWinnera, numer_ruchu);
        }
        else if (winner == 2)
        {
            tab[3] = AwardLossP1(stateZPerspektywyWinnera, numer_ruchu);
            tab[0] = AwardLossP2(stateZPerspektywyWinnera, numer_ruchu);
            tab[1] = AwardLossP3(stateZPerspektywyWinnera, numer_ruchu);
        }
        else if (winner == 3)
        {
            tab[0] = AwardLossP1(stateZPerspektywyWinnera, numer_ruchu);
            tab[1] = AwardLossP1(stateZPerspektywyWinnera, numer_ruchu);
            tab[2] = AwardLossP1(stateZPerspektywyWinnera, numer_ruchu);
        }

        return tab;
    }
}