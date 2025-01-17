using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SplendorConsole;

namespace SplendorTests
{
    [TestFixture]
    class AvailableNoblesTests
    {
        [Test]
        public void LoadNoblesFromExcel_Test()
        {

            var availableNobles = new AvailableNobles();


            availableNobles.LoadNoblesFromExcel("LoadNoblesFromExcelTest.xlsx");
            var noble = availableNobles.noblesList[0].ToString();
            string message = "Arystokrata dodający 3 punktów. Wymagania: WHITE: 3, BLUE: 3, GREEN: 3, RED: 0, BLACK: 0";


            Assert.That(noble, Is.EqualTo(message));
        }

    }
}
