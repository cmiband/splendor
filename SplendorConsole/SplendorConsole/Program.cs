using SplendorConsole;

public class Program
{
    async public static Task Main(string[] args)
    {
        Game game = new Game();
        await game.GameStart();
    }
}