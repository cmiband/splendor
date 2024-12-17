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
        int N = 1000000;
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
                (int winner, float[]? state) = await game.GameStart();
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
}