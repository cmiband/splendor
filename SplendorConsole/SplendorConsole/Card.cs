using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplendorConsole
{
    public class Card
    {
        private Resources detailedPrice;
        private int level;
        private GemColor bonusColor;
        private int points;
        private string illustration;

        public int Level
        {
            get { return level; }
        }

        public Resources DetailedPrice
        {
            get { return detailedPrice; }
        }

        public GemColor BonusColor
        {
            get => bonusColor;
        }

        public Card(int level, GemColor bonusColor, int points, string illustration, Resources detailedPrice)
        {
            this.detailedPrice = detailedPrice;
            this.level = level;
            this.bonusColor = bonusColor;
            this.points = points;
            this.illustration = illustration;
        }
    }
}
