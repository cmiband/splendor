using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplendorConsole
{
    internal class Bank
    {
        public Resources resources = new Resources();

        public void TakeOutResources(int amount, GemColor color)
        {
            this.resources.gems[color] -= amount;
            
        }

        public void AddResources(int amount, GemColor color)
        {
            this.resources.gems[color] += amount;
        }

        public void AddGoldenGem()
        {
            this.resources.gems[GemColor.GOLDEN]++;
           
        }
        public bool CanTakeGoldenGem()
        {

            return this.resources.gems[GemColor.GOLDEN] >0;
        }
    }
}
