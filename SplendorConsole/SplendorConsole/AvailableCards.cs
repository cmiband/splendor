using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplendorConsole
{
    public class AvailableCards
    {
        private List<string, int, string, int, int, int, int, int> karty_1_Poziomu = new List<string, int, string, int, int, int, int, int>; //gem (card) color, PV, type of card ex. mine, koszt kupna kart po kolei white, blue, green, red, black
        private List<string, int, string, int, int, int, int, int> karty_2_Poziomu = new List<string, int, string, int, int, int, int, int>;
        private List<string, int, string, int, int, int, int, int> karty_3_Poziomu = new List<string, int, string, int, int, int, int, int>;
        
        //Czarne karty 1 poziomu
        karty_1_poziomu.Add("black", 0, "mine", 1, 1, 1, 1, 0);
        karty_1_poziomu.Add("black", 0, "mine", 1, 2, 1, 1, 0);
        karty_1_poziomu.Add("black", 0, "mine", 2, 2, 0, 1, 0);
        karty_1_poziomu.Add("black", 0, "mine", 0, 0, 1, 3, 1);
        karty_1_poziomu.Add("black", 0, "mine", 0, 0, 2, 1, 0);
        karty_1_poziomu.Add("black", 0, "mine", 2, 0, 2, 0, 0);
        karty_1_poziomu.Add("black", 0, "mine", 0, 0, 3, 0, 0);
        karty_1_poziomu.Add("black", 1, "mine", 0, 4, 0, 0, 0);
        //Niebieskie karty 1 poziomu
        karty_1_poziomu.Add("blue", 0, "mine", 1, 0, 1, 1, 1);
        karty_1_poziomu.Add("blue", 0, "mine", 1, 0, 1, 2, 1);
        karty_1_poziomu.Add("blue", 0, "mine", 1, 0, 2, 2, 0);
        karty_1_poziomu.Add("blue", 0, "mine", 0, 1, 3, 1, 0);
        karty_1_poziomu.Add("blue", 0, "mine", 1, 0, 0, 0, 2);
        karty_1_poziomu.Add("blue", 0, "mine", 0, 0, 2, 0, 2);
        karty_1_poziomu.Add("blue", 0, "mine", 0, 0, 0, 0, 3);
        karty_1_poziomu.Add("blue", 1, "mine", 0, 0, 0, 4, 0);
        //Białe karty 1 poziomu
        karty_1_poziomu.Add("white", 0, "mine", 0, 1, 1, 1, 1);
        karty_1_poziomu.Add("white", 0, "mine", 0, 1, 2, 1, 1);
        karty_1_poziomu.Add("white", 0, "mine", 0, 2, 2, 0, 1);
        karty_1_poziomu.Add("white", 0, "mine", 3, 1, 0, 0, 1);
        karty_1_poziomu.Add("white", 0, "mine", 0, 0, 0, 2, 1);
        karty_1_poziomu.Add("white", 0, "mine", 0, 2, 0, 0, 2);
        karty_1_poziomu.Add("white", 0, "mine", 0, 3, 0, 0, 0);
        karty_1_poziomu.Add("white", 1, "mine", 0, 0, 4, 0, 0);
        //Zielone karty 1 poziomu
        karty_1_poziomu.Add("green", 0, "mine", 1, 1, 0, 1, 1);
        karty_1_poziomu.Add("green", 0, "mine", 1, 1, 0, 1, 2);
        karty_1_poziomu.Add("green", 0, "mine", 0, 1, 0, 2, 2);
        karty_1_poziomu.Add("green", 0, "mine", 1, 3, 1, 0, 0);
        karty_1_poziomu.Add("green", 0, "mine", 2, 1, 0, 0, 0);
        karty_1_poziomu.Add("green", 0, "mine", 0, 2, 0, 2, 0);
        karty_1_poziomu.Add("green", 0, "mine", 0, 0, 0, 3, 0);
        karty_1_poziomu.Add("green", 1, "mine", 0, 0, 0, 0, 4);
        //Czerwone karty 1 poziomu
        karty_1_poziomu.Add("red", 0, "mine", 1, 1, 1, 0, 1);
        karty_1_poziomu.Add("red", 0, "mine", 2, 1, 1, 0, 1);
        karty_1_poziomu.Add("red", 0, "mine", 2, 0, 1, 0, 2);
        karty_1_poziomu.Add("red", 0, "mine", 1, 0, 0, 1, 3);
        karty_1_poziomu.Add("red", 0, "mine", 0, 2, 1, 0, 0);
        karty_1_poziomu.Add("red", 0, "mine", 2, 0, 0, 2, 0);
        karty_1_poziomu.Add("red", 0, "mine", 3, 0, 0, 0, 0);
        karty_1_poziomu.Add("red", 1, "mine", 4, 0, 0, 0, 0);

        //Czarne karty 2 poziomu
        karty_2_Poziomu.Add("black", 1, "camels", 3, 2, 2, 0, 0);
        karty_2_Poziomu.Add("black", 1, "camels", 3, 0, 3, 0, 2);
        karty_2_Poziomu.Add("black", 2, "camels", 0, 1, 4, 2, 0);
        karty_2_Poziomu.Add("black", 2, "lapidary", 0, 0, 5, 3, 0);
        karty_2_Poziomu.Add("black", 2, "lapidary", 5, 0, 0, 0, 0);
        karty_2_Poziomu.Add("black", 3, "lapidary", 0, 0, 0, 0, 6);
        //Niebieskie karty 2 poziomu
        karty_2_Poziomu.Add("blue", 1, "elephants", 0, 2, 2, 3, 0);
        karty_2_Poziomu.Add("blue", 1, "elephants", 0, 2, 3, 0, 3);
        karty_2_Poziomu.Add("blue", 2, "elephants", 5, 3, 0, 0, 0);  
        karty_2_Poziomu.Add("blue", 2, "lapidary", 2, 0, 0, 1, 4); 
        karty_2_Poziomu.Add("blue", 2, "lapidary", 0, 5, 0, 0, 0);
        karty_2_Poziomu.Add("blue", 3, "lapidary", 0, 6, 0, 0, 0);
        //Białe karty 2 poziomu
        karty_2_Poziomu.Add("white", 1, "people in snow", 0, 0, 3, 2, 2);
        karty_2_Poziomu.Add("white", 1, "people in snow", 2, 3, 0, 3, 0);
        karty_2_Poziomu.Add("white", 2, "people in snow", 0, 0, 1, 4, 2);
        karty_2_Poziomu.Add("white", 2, "lapidary", 0, 0, 0, 5, 3);
        karty_2_Poziomu.Add("white", 2, "lapidary", 0, 0, 0, 5, 0);
        karty_2_Poziomu.Add("white", 3, "lapidary", 6, 0, 0, 0, 0);
        //Zielone karty 2 poziomu
        karty_2_Poziomu.Add("green", 1, "guy", 3, 0, 2, 3, 0);
        karty_2_Poziomu.Add("green", 1, "guy", 2, 3, 0, 0, 2);
        karty_2_Poziomu.Add("green", 2, "guy", 4, 2, 0, 0, 1);
        karty_2_Poziomu.Add("green", 2, "carrack", 0, 5, 3, 0, 0);
        karty_2_Poziomu.Add("green", 2, "carrack", 0, 0, 5, 0, 0);
        karty_2_Poziomu.Add("green", 3, "carrack", 0, 0, 6, 0, 0);
        //Czerwone karty 2 poziomu
        karty_2_Poziomu.Add("green", 1, "felucca", 2, 0, 0, 2, 3);
        karty_2_Poziomu.Add("green", 1, "felucca", 0, 3, 0, 2, 3);
        karty_2_Poziomu.Add("green", 2, "felucca", 1, 4, 2, 0, 0);
        karty_2_Poziomu.Add("green", 2, "lapidary", 3, 0, 0, 0, 5);
        karty_2_Poziomu.Add("green", 2, "lapidary", 0, 0, 0, 0, 5);
        karty_2_Poziomu.Add("green", 3, "lapidary", 0, 0, 0, 6, 0);

        //Czarne karty 3 poziomu 
        karty_3_Poziomu.Add("black", 3, "street1", 3, 3, 5, 3, 0);
        karty_3_Poziomu.Add("black", 4, "street2", 0, 0, 0, 7, 0);
        karty_3_Poziomu.Add("black", 4, "street2", 0, 0, 3, 6, 3);
        karty_3_Poziomu.Add("black", 5, "street1", 0, 0, 0, 7, 3);
        //Niebieskie karty 3 poziomu 
        karty_3_Poziomu.Add("blue", 3, "Venice", 3, 0, 3, 3, 5);
        karty_3_Poziomu.Add("blue", 4, "Venice", 7, 0, 0, 0, 0);
        karty_3_Poziomu.Add("blue", 4, "diamond shop", 6, 3, 0, 0, 3);
        karty_3_Poziomu.Add("blue", 5, "diamond shop", 7, 3, 0, 0, 0);
        //Białe karty 3 poziomu 
        karty_3_Poziomu.Add("white", 3, "building", 0, 3, 3, 5, 3);
        karty_3_Poziomu.Add("white", 4, "Gioielleria", 0, 0, 0, 0, 7);
        karty_3_Poziomu.Add("white", 4, "Gioielleria", 3, 0, 0, 3, 6);
        karty_3_Poziomu.Add("white", 5, "building", 3, 0, 0, 0, 7);
        //Zielone karty 3 poziomu 
        karty_3_Poziomu.Add("green", 3, "timbered house", 5, 3, 0, 3, 3);
        karty_3_Poziomu.Add("green", 4, "bridge", 0, 7, 0, 0, 0);
        karty_3_Poziomu.Add("green", 4, "bridge", 3, 6, 3, 0, 0);
        karty_3_Poziomu.Add("green", 5, "timbered house", 0, 7, 3, 0, 0);
        //Czerwone karty 3 poziomu 
        karty_3_Poziomu.Add("red", 3, "equestrian statue", 3, 5, 3, 0, 3);
        karty_3_Poziomu.Add("red", 4, "equestrian statue", 0, 0, 7, 0, 0);
        karty_3_Poziomu.Add("red", 4, "building", 0, 3, 6, 3, 0);
        karty_3_Poziomu.Add("red", 5, "building", 0, 0, 7, 3, 0);
    }

}