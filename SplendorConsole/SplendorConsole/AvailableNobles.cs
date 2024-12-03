using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace SplendorConsole
{
    public class AvailableNobles
    {
        public List<Noble> noblesList = new List<Noble>();

        public void LoadNoblesFromExcel(string filePath = "NoblesWykaz.xlsx")
        {
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheets.First();
                foreach (var row in worksheet.RowsUsed())
                {
                    int points = int.Parse(row.Cell(1).GetString());

                    var detailedPrice = new Resources();
                    detailedPrice.gems[GemColor.WHITE] = int.Parse(row.Cell(2).GetString());
                    detailedPrice.gems[GemColor.BLUE] = int.Parse(row.Cell(3).GetString());
                    detailedPrice.gems[GemColor.GREEN] = int.Parse(row.Cell(4).GetString());
                    detailedPrice.gems[GemColor.RED] = int.Parse(row.Cell(5).GetString());
                    detailedPrice.gems[GemColor.BLACK] = int.Parse(row.Cell(6).GetString());

                    string illustration = row.Cell(7).GetString();

                    var noble = new Noble(points, detailedPrice, illustration);

                    noblesList.Add(noble);
                }
            }
        }
    }

}