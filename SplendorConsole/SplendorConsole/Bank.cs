using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplendorConsole
{
    public class Bank
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

        public void AddGoldenGem(int amount)
        {
            this.resources.gems[GemColor.GOLDEN] += amount;
           
        }
        public bool CanTakeGoldenGem()
        {

            return this.resources.gems[GemColor.GOLDEN] >0;
        }

        public int[] ToArray()
        {
            return new int[] { resources.gems.ContainsKey(GemColor.WHITE) ? resources.gems[GemColor.WHITE] : 0,
                               resources.gems.ContainsKey(GemColor.BLUE) ? resources.gems[GemColor.BLUE] : 0,
                               resources.gems.ContainsKey(GemColor.GREEN) ? resources.gems[GemColor.GREEN] : 0,
                               resources.gems.ContainsKey(GemColor.RED) ? resources.gems[GemColor.RED] : 0,
                               resources.gems.ContainsKey(GemColor.BLACK) ? resources.gems[GemColor.BLACK] : 0,
                               resources.gems.ContainsKey(GemColor.GOLDEN) ? resources.gems[GemColor.GOLDEN] : 0
                             };
        }
    }
}
