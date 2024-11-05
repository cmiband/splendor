using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplendorConsole
{
    public class Resources : IEnumerable<KeyValuePair<GemColor, int>>
    {
        public Dictionary<GemColor, int> gems = new Dictionary<GemColor, int>();
        public IEnumerator<KeyValuePair<GemColor, int>> GetEnumerator()
        {
            return gems.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
