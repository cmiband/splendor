using NUnit.Framework;
using SplendorConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplendorTests
{
    [TestFixture]
    public class ResourcesTests
    {

        [Test]
        public void Equals_SameGems_ShouldReturnTrue()
        {
            var resources1 = new Resources();
            resources1.gems[GemColor.RED] = 2;
            resources1.gems[GemColor.BLUE] = 3;

            var resources2 = new Resources();
            resources2.gems[GemColor.RED] = 2;
            resources2.gems[GemColor.BLUE] = 3;

            Assert.That(resources1.Equals(resources2), Is.True);
        }

        [Test]
        public void GetHashCode_DiffrentGems_ShouldReturnFalse()
        {
            var resources1 = new Resources();
            resources1.gems[GemColor.RED] = 5;
            resources1.gems[GemColor.BLUE] = 5;
            var hash1 = resources1.GetHashCode();

            var resources2 = new Resources();
            resources2.gems[GemColor.RED] = 2;
            resources2.gems[GemColor.BLUE] = 3;
            var hash2 = resources2.GetHashCode();

            Assert.That(hash1.Equals(hash2), Is.False);
        }

        [Test]
        public void AddResource_MultipleAdd_ShouldReturnTrue()
        {
            var resources = new Resources();
            resources.gems[GemColor.RED] = 5;
            resources.gems[GemColor.BLUE] = 5;

            resources.AddResource(GemColor.RED);
            resources.AddResource(GemColor.BLUE);

            var totalGemCount = resources.gems.Values.Sum();


            Assert.That(totalGemCount.Equals(12), Is.True);
        }

        [Test]
        public void ToString_ShouldReturnFormattedString()
        {
            var resources = new Resources();
            resources.gems[GemColor.RED] = 2;
            resources.gems[GemColor.BLUE] = 3;

            string expected = "RED: 2, BLUE: 3";
            Assert.That(expected.Equals(resources.ToString()));
        }
    }
}
