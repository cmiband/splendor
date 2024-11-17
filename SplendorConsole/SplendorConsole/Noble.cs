using DocumentFormat.OpenXml.Office2013.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplendorConsole
{
    public class Noble
    {
        
        public Noble(int points, Resources requiredBonuses) 
        {
            Points = points;
            RequiredBonuses = requiredBonuses;
        }
        
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

        
        public override string ToString()
        {
            return $"Arystokrata dodający {points} punktów. Wymagania: {requiredBonuses.ToString()}";
        }


        public int[] ToArray()
        {
            return new int[]   {points,
                                requiredBonuses.gems[GemColor.WHITE],
                                requiredBonuses.gems[GemColor.BLUE],
                                requiredBonuses.gems[GemColor.GREEN],
                                requiredBonuses.gems[GemColor.RED],
                                requiredBonuses.gems[GemColor.BLACK]
                                };
        }

    }
}
