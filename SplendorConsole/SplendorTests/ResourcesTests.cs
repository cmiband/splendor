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
    class ResourcesTests
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
        public void GetHashCode_SameGems_ShouldReturnTrue()
        {
            var resources1 = new Resources();
            resources1.gems[GemColor.RED] = 2;
            resources1.gems[GemColor.BLUE] = 3;

            var resources2 = new Resources();
            resources2.gems[GemColor.RED] = 2;
            resources2.gems[GemColor.BLUE] = 3;

            int hashcode1 = resources1.GetHashCode();
            int hashcode2 = resources2.GetHashCode();

            Assert.That(hashcode1.Equals(hashcode2), Is.True);
        }

        [Test]
        public void ToString_ShouldReturnCorrectString()
        {
            var resources = new Resources();
            resources.gems[GemColor.RED] = 2;
            resources.gems[GemColor.BLUE] = 3;

            string expected = "RED: 2, BLUE: 3";
            Assert.That(expected.Equals(resources.ToString()), Is.True);
        }

        [Test]
        public void ToStringWithdraw_ShouldReturnCorrectString()
        {
            var resources = new Resources();
            resources.gems[GemColor.RED] = 2;
            resources.gems[GemColor.BLUE] = 3;

            string expected = "\n1. RED:\t2\n2. BLUE:\t3";
            Assert.That(expected.Equals(resources.ToStringForWithdraw()), Is.True);
        }

        [Test]
        public void AddRecource_MultipleAddGems()
        {
            var resources = new Resources();
            resources.AddResource(GemColor.RED);
            resources.AddResource(GemColor.RED);

            Assert.That(resources.gems[GemColor.RED].Equals(2));
        }
      

    }
}
