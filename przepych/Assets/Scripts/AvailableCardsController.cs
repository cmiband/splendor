using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using UnityEngine;

public class AvailableCardsController : MonoBehaviour
{
    public List<CardController> level1Cards = new List<CardController>();
    public List<CardController> level2Cards = new List<CardController>();
    public List<CardController> level3Cards = new List<CardController>();

    public void LoadCardsFromExcel(string filePath = "KartyWykaz.xlsx")
    {
        using (var workbook = new XLWorkbook(filePath))
        {
            var worksheet = workbook.Worksheets.First();
            foreach (var row in worksheet.RowsUsed())
            {
                int level = int.Parse(row.Cell(1).GetString());
                string bonusColorStr = row.Cell(2).GetString();
                GemColor bonusColor = (GemColor)Enum.Parse(typeof(GemColor), bonusColorStr.ToUpper());
                int points = int.Parse(row.Cell(3).GetString());
                string illustration = row.Cell(4).GetString();

                var detailedPrice = new ResourcesController();
                detailedPrice.gems[GemColor.WHITE] = int.Parse(row.Cell(5).GetString());
                detailedPrice.gems[GemColor.BLUE] = int.Parse(row.Cell(6).GetString());
                detailedPrice.gems[GemColor.GREEN] = int.Parse(row.Cell(7).GetString());
                detailedPrice.gems[GemColor.RED] = int.Parse(row.Cell(8).GetString());
                detailedPrice.gems[GemColor.BLACK] = int.Parse(row.Cell(9).GetString());

                var card = new CardController(level, bonusColor, points, illustration, detailedPrice);

                AddCard(card);
            }
        }
    }
    public void AddCard(CardController card)
    {
        switch (card.Level)
        {
            case 1:
                level1Cards.Add(card);
                break;
            case 2:
                level2Cards.Add(card);
                break;
            case 3:
                level3Cards.Add(card);
                break;
            default:
                throw new ArgumentException("Wrong card level!");
        }
    }
}
