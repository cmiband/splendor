using System;
using System.Collections.Generic;
using System.Drawing;
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

        public Card(int level, GemColor bonusColor, int points, string illustration, Resources detailedPrice)
        {
            this.detailedPrice = detailedPrice;
            this.level = level;
            this.bonusColor = bonusColor;
            this.points = points;
            this.illustration = illustration;
        }
        //dodane
        public void Echo(bool alter)
        {
            string valuee = "";
            string price = "";
            bool boolean = false;
            List<string> color = new List<string>();

            foreach (var gem in detailedPrice.gems)
            {
                if (color.Count == 0) color.Add(Convert.ToString(gem.Key));
                foreach (var value in color)
                {
                    if (value != Convert.ToString(gem.Key))
                    {
                        boolean = true;
                        break;
                    }
                }
                if (boolean)
                {
                    color.Add(Convert.ToString(gem.Key));
                    boolean = false;
                }
            }

            foreach (var gem in detailedPrice.gems)
            {
                price += Convert.ToString(gem.Value);
            }
            price += "!";

            if (alter)
            {
                for (int i = 0; i < color.Count; i++)
                {
                    if (price[i] != '!') valuee += $"{color[i]} = {price[i]} ";
                    else valuee += $"{color[i]} = {0} ";
                }
                Console.WriteLine($"level: {level}");
                Console.WriteLine($"bonus: {bonusColor}");
                Console.WriteLine($"points: {points}");
                Console.WriteLine($"price: {valuee}");
                Console.WriteLine($"illustration: {illustration}");
                Console.WriteLine();
            }
            else
            {
                for (int i = 0; i < color.Count; i++)
                {
                    if (price[i] != '!') valuee += price[i];
                    else valuee += 0;
                }
                Console.WriteLine(level);
                Console.WriteLine(bonusColor);
                Console.WriteLine(points);
                Console.WriteLine(valuee);
                Console.WriteLine(illustration);
                Console.WriteLine();
            }
        }
    }
}
