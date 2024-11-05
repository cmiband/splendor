using DocumentFormat.OpenXml.Presentation;
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
        public List<Card> hand = new List<Card>();
        private Card[] reservedCard;
        private List<Noble> nobles = new List<Noble>();
        private int points;
        public int Points
        {
            get => points;
        }
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
            else this.resources.gems.Add(color, 2);
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
            if (CanGetMultipleNobles() == false)
            {
                foreach (Noble noble in Board.VisibleNobles)
                    if (CanGetNoble(noble))
                        GetNoble(noble);
            }
            else
            {
                List<int> AvailableIndexNobles = new List<int>();
                for (int i = 0; i < Board.VisibleNobles.Length; i++)
                {
                    Noble noble = Board.VisibleNobles[i];
                    if (CanGetNoble(noble))
                        AvailableIndexNobles.Add(i);
                }

                Console.WriteLine("Arystokraci, których możesz zdobyć: ");
                for (int i = 0; i < AvailableIndexNobles.Count; i++)
                    Console.WriteLine(AvailableIndexNobles[i]);


                bool IsChoiceMade = false;
                int choice=0;
                while (IsChoiceMade == false)
                {
                    try
                    {
                        Console.WriteLine("Wybierz arystokratę: ");
                        choice = int.Parse(Console.ReadLine());
                        IsChoiceMade = true;
                    }
                    catch
                    {
                        Console.WriteLine("Niepoprawny numer, podaj jeszcze raz");
                    }
                }

                Noble playerChoice = Board.VisibleNobles[choice];
                GetNoble(playerChoice);

            }

        }
        public void GetNoble(Noble noble)
        {
            for(int i=0;i<nobles.Length;i++)
            {
                if (nobles[i] == null)
                    nobles[i] = noble;
            }

            Noble[] CopiedVisibleNobles = new Noble[Board.VisibleNobles.Length - 1];
            int j = 0;
            for(int i=0;i<Board.VisibleNobles.Length;i++)
            {
                if (Board.VisibleNobles[i] != noble)
                {
                    CopiedVisibleNobles[j] = Board.VisibleNobles[i];
                    j++;
                }
            }

            Array.Copy(CopiedVisibleNobles, Board.VisibleNobles, CopiedVisibleNobles.Length);


            points += noble.Points;
        }

        public bool CanGetMultipleNobles()
        {
            int counter = 0;
            foreach (Noble noble in Board.VisibleNobles)
            {
                if (CanGetNoble(noble))
                    counter++;
            }

            if (counter > 1)
                return true;
            else
                return false;
        }

        public bool CanGetNoble(Noble noble)
        {
            foreach (var requiredBonus in noble.RequiredBonuses)
            {
                GemColor color = requiredBonus.Key;
                int requiredAmount = requiredBonus.Value;

                if (noble.RequiredBonuses.gems[color] <= bonusResources.gems[color])
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

        public void PointsCounter()
        {
            this.points = 0;
            foreach(Card card in this.hand)
            {
                points += card.Points;
            }
            foreach(Noble noble in this.nobles)
            {
                points += noble.Points;
            }
        }
    }
}
