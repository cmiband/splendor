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
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Resources))
                return false;

            if (obj == null)
                return false;

            Resources otherResources = (Resources)obj;

            if (gems.Count != otherResources.gems.Count)
                return false;

            foreach (var kvp in gems)
            {
                if (!otherResources.gems.TryGetValue(kvp.Key, out int otherValue) || kvp.Value != otherValue)
                    return false;
            }
            return true;
        }
        public override int GetHashCode()
        {
            int hash = 17;
            foreach (var kvp in gems)
            {
                hash = HashCode.Combine(hash, kvp.Key, kvp.Value);
            }
            return hash;
        }
        public override string ToString()
        {
            int i = 1;
            string gemsdate = "";
            foreach (var singlegem in gems)
            {
                gemsdate += $"{i}. {singlegem.Key}: {singlegem.Value}\n";
                i++;
            }
            return gemsdate;
        }
        public void AddResource(GemColor color)
        {
            if (gems.ContainsKey(color))
            {
                gems[color] += 1;
            }
            else
            {
                gems[color] = 1;
            }

            Console.WriteLine($"Dodano 1 zasób koloru {color}. Łączna ilość: {gems[color]}.");
        }
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
