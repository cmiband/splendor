using SplendorConsole;


public class Program
{
    public static void Main(string[] args)
    {
        SplendorConsole.AvailableCards card = new SplendorConsole.AvailableCards();
        card.LoadCardsFromExcel("KartyWykaz.xlsx");
        card.Echo();
    }
}