using SplendorConsole;


public class Program
{
    public static void Main(string[] args)
    {
        SplendorConsole.AvailableCards card = new SplendorConsole.AvailableCards();
        card.LoadCardsFromExcel("KartyWykaz.xlsx");
        //wypisuje karty z informacjami co jest co
        card.Echo(true);
        //wypisuje same dane kart
        card.Echo(false);
    }
}