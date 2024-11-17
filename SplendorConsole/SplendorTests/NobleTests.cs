using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SplendorConsole;

namespace SplendorTests
{
    internal class NobleTests
    {
        [Test]
        public void Constructor_ShouldInitializeProperties()
        {
            var requiredBonuses = new Resources();
            requiredBonuses.gems[GemColor.WHITE] = 3;
            requiredBonuses.gems[GemColor.BLUE] = 2;

            int points = 3;

            var noble = new Noble(points, requiredBonuses);

            Assert.That(noble.Points, Is.EqualTo(points));
            Assert.That(noble.RequiredBonuses, Is.EqualTo(requiredBonuses));
        }

        [Test]
        public void PointsProperty_ShouldGetAndSetValues()
        {
            var noble = new Noble(3, new Resources());

            noble.Points = 5;

            Assert.That(noble.Points, Is.EqualTo(5));
        }

        [Test]
        public void RequiredBonusesProperty_ShouldGetAndSetValues()
        {
            var initialBonuses = new Resources();
            initialBonuses.gems[GemColor.RED] = 2;
            var noble = new Noble(3, initialBonuses);

            var newBonuses = new Resources();
            newBonuses.gems[GemColor.BLACK] = 4;

            noble.RequiredBonuses = newBonuses;

            Assert.That(noble.RequiredBonuses, Is.EqualTo(newBonuses));
        }

        [Test]
        public void ToString_ShouldReturnFormattedString()
        {
            var requiredBonuses = new Resources();
            requiredBonuses.gems[GemColor.WHITE] = 2;
            requiredBonuses.gems[GemColor.BLUE] = 1;

            var noble = new Noble(5, requiredBonuses);

            var result = noble.ToString();

            Assert.That(result, Is.EqualTo("Arystokrata dodający 5 punktów. Wymagania: WHITE: 2, BLUE: 1"));
        }
    }
}
