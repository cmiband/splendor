using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplendorConsole
{
    internal class Bank
    {
        public Resources resources;

        public void TakeOutResources(int amount, GemColor color)
        {
            resources.gems.Remove(color, out amount);
        }
    }
}
