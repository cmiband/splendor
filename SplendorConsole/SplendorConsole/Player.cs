using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplendorConsole
{
    internal class Player
    {
        private Resources resources = new Resources();
        private Resources bonusResources;
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

        public void GettingNobles()
        {
            if(CanGetMultipleNobles()==false)
            {
                foreach(Noble noble in VisibleNobles)
                    if(CanGetNoble(noble))
                        GetNoble(noble);
            }
            else
            {
                int[] AvailableIndexNobles=new Resources();
                for(int i=0;i<VisibleNobles.Length;i++)
                {
                    Noble noble = VisibleNobles[i]
                    if(CanGetNoble(noble))
                        AvailableIndexNobles.Add(i);
                }

                Console.WriteLine("Arystokraci, których możesz zdobyć: ");
                for int i=0;i<AvailableIndexNobles.Length;i++)
                    Console.WriteLine(AvailableIndexNobles[i]);


                bool IsChoiceMade=false;
                while(IsChoiceMade==false)
                {   
                    try
                    {
                        Console.WriteLine("Wybierz arystokratę: ");
                        choice = int.Parse(Console.ReadLine());
                        IsChoiceMade=true;
                    }
                    catch
                    {
                        Console.WriteLine("Niepoprawny numer, podaj jeszcze raz");
                    }
                }

                Noble playerChoice = VisibleNobles[choice];
                GetNoble(playerChoice);
                
        }
        
        public void GetNoble(Noble noble)
        {
                nobles.Add(noble);
                Board.VisibleNobles.Remove(noble);
                points+=noble.points;
        }

        public bool CanGetMultipleNobles()
        {
            int counter=0;
            foreach(Noble noble in VisibleNobles)
            {
                if(CanGetNoble(noble))
                    counter++;
            }

            if(counter>1)
                return true;
            else
                return false;
        }
        
        public bool CanGetNoble(Noble noble)
        {
            foreach (GetColor color in noble.requiredBonuses)
            {
                if(noble.requiredBonuses[color]<=bonusRecourses[color])
                {
                    continue;
                }
                else
                    return false;
            }
            return true;
        }

                    
        public bool CanAffordCard(Card card)
        {
            throw new NotImplementedException();
        }
        public void PassTurn()
        {
            throw new NotImplementedException();
        }

    }
}
