using SplendorConsole;

public class Program
{
    public static void Main(string[] args)
    {
        Game game = new Game();
        game.GameStart();
        Console.WriteLine(game.Bank.resources);
        Console.WriteLine(game.Level1Shuffled);
        Console.WriteLine(game.Level2Shuffled);
        Console.WriteLine(game.Level3Shuffled);
        Console.WriteLine(game.Level1VisibleCards);
        Console.WriteLine(game.Level2VisibleCards);
        Console.WriteLine(game.Level3VisibleCards);
        Console.WriteLine(game.ListOfNobles);
        Console.WriteLine(game.ListOfPlayers);
    }
}