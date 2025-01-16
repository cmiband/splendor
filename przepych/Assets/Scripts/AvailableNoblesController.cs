using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using UnityEngine;

    public class AvailableNoblesController : MonoBehaviour, IEnumerable<NobleController>
    {
        public List<NobleController> noblesList = new List<NobleController>();

        public void LoadNoblesFromExcel(string filePath = "NoblesWykaz.xlsx")
        {
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheets.First();
                foreach (var row in worksheet.RowsUsed())
                {
                    int points = int.Parse(row.Cell(1).GetString());

                    var detailedPrice = new ResourcesController();
                    detailedPrice.gems[GemColor.WHITE] = int.Parse(row.Cell(2).GetString());
                    detailedPrice.gems[GemColor.BLUE] = int.Parse(row.Cell(3).GetString());
                    detailedPrice.gems[GemColor.GREEN] = int.Parse(row.Cell(4).GetString());
                    detailedPrice.gems[GemColor.RED] = int.Parse(row.Cell(5).GetString());
                    detailedPrice.gems[GemColor.BLACK] = int.Parse(row.Cell(6).GetString());
                    detailedPrice.gems[GemColor.GOLDEN] = 0;
                    detailedPrice.gems[GemColor.NONE] = 0;

                    string illustration = row.Cell(7).GetString();

                    var noble = new NobleController(points, detailedPrice, illustration);

                    noblesList.Add(noble);
                }
            }
        }
        public IEnumerator<NobleController> GetEnumerator()
        {
            return noblesList.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
