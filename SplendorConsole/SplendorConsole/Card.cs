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
        public int Points { get => points; }

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
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Card))
                return false;

            Card other = (Card)obj;

            return level == other.level &&
                   bonusColor == other.bonusColor &&
                   points == other.points &&
                   illustration == other.illustration &&
                   detailedPrice.Equals(other.detailedPrice);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(level, bonusColor, points, illustration, detailedPrice);
        }

    }
}
