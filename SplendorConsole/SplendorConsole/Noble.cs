using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplendorConsole
{
    public class Noble:IEnumerable<Resources>
    {
        private int points;
        public int Points
        {
            get => points;
            set => points = value;
        }
        private Resources requiredBonuses;

        public Resources RequiredBonuses
        {
            get=> requiredBonuses;
            set => requiredBonuses = value;
        }
        private string illustration;

        public IEnumerator<Resources> GetEnumerator()
        {
            return (IEnumerator<Resources>)requiredBonuses.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
