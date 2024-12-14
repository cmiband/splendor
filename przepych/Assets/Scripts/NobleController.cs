using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


    public class NobleController :  MonoBehaviour 
    {
        public ResourcesController detailedPrice;
        public int points;
        public string illustration;
        private Image selectedNobleImage;
        public string detailedPriceInfo = "";

        public NobleController(int points, ResourcesController detailedPrice, string illustration)
        {
            this.points = points;
            this.detailedPrice = detailedPrice;
            this.illustration = illustration;

            this.detailedPriceInfo = this.detailedPrice.ToString();
        }
        

        public void InitNobleData(NobleController noble)
        {
            this.points = noble.points;
            this.detailedPrice = noble.detailedPrice;
            this.illustration = noble.illustration;

            this.detailedPriceInfo = this.detailedPrice.ToString();
        }
}

