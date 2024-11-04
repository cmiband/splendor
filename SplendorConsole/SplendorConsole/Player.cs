using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SplendorConsole
{
    internal class Player
    {
        private Resources resources = new Resources();
        private Resources bonusResources = new Resources();
        private Card[] hand;
        private Card[] reservedCard;
        private Noble[] nobles;
        private int points;
        public void BuyCard(Card card)
        {
            throw new NotImplementedException();
        }
        public void ReserveCard(Card card)
        {
            throw new NotImplementedException();
        }
        public void TakeTwoTokens(Resources resources, GemColor color)
        {

            if (this.resources.gems.ContainsKey(color))
            {
                this.resources.gems[color] += 2;

            }
            else this.resources.gems.Add(color,2);
        }
        public void TakeThreeTokens(Resources resources, GemColor[] colors)
        {
            for (int i = 0; i < 3; i++)
            {
                if (this.resources.gems.ContainsKey(colors[i]))
                {
                    this.resources.gems[colors[i]] += 1;

                }
                else this.resources.gems.Add(colors[i], 1);
            }
        }
        public Noble GetNoble(Noble noble)
        {
            throw new NotImplementedException();
        }
        public bool CanGetNoble(Noble noble)
        {
            throw new NotImplementedException();
        }
        public bool CanAffordCard(Card card)
        {
            var price = card.DetailedPrice.gems;
            var playerResources = this.resources.gems;
            var playerBonuses = this.bonusResources.gems;
            var wantToSpendGoldCoins = WantToSpendGoldCoin();

            int goldTokens = playerResources.ContainsKey(GemColor.GOLDEN) ? playerResources[GemColor.GOLDEN] : 0;

            foreach (var colorOnCard in price)
            {
                GemColor color = colorOnCard.Key;
                int requiredAmount = colorOnCard.Value;

                int playerAmount = playerResources.ContainsKey(color) ? playerResources[color] : 0;;
                int bonusAmount = playerBonuses.ContainsKey(color) ? playerBonuses[color] : 0;
                int totalPlayersCoins = playerAmount + bonusAmount;

                if (wantToSpendGoldCoins && totalPlayersCoins<requiredAmount)
                {
                    int deficit = requiredAmount - totalPlayersCoins;
                    if (goldTokens>=deficit)
                        goldTokens -=deficit;
                    else
                        return false;
                }
                if (!wantToSpendGoldCoins && totalPlayersCoins < requiredAmount)
                    return false;
            }
            return true;
        }
        public void PassTurn()
        {
            throw new NotImplementedException();
        }
        public bool WantToSpendGoldCoin()
        {
            Console.WriteLine("Czy chcesz użyć złotego żetonu aby płacić za kartę?");
            Console.WriteLine("1 - Tak");
            Console.WriteLine("2 - Nie");
            var wantTo = Convert.ToInt32(Console.ReadLine());
            while (true)
            {
                if (wantTo == 2)
                    return false;
                else if (wantTo == 1)
                    return true;
                else
                    Console.WriteLine("Podano zły klawisz. 1 lub 2");
            }
        }
    }
}
