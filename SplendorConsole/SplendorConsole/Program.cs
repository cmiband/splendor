using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SplendorConsole;

public class Program
{
    async public static Task Main(string[] args)
    {
        WebserviceClient client = new WebserviceClient("ws://localhost:8765");
        await client.ConnectToWebsocket();
        Game? game;
        int N = 1000;

        Stopwatch stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < N; i++)
        {
            game = new Game(client);
            (int winner, float[]? state) = await game.GameStart();
            Console.WriteLine(i + "tą grę wygrywa " + winner);
            game = null;
        }

        stopwatch.Stop();
        Console.WriteLine($"Cała pętla zakończona w czasie: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Średni czas symulacji jednej rozgrywki: {stopwatch.ElapsedMilliseconds/N} ms");

    }
}
