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

    public NobleController(int points, ResourcesController detailedPrice, string illustration)
    {
        this.points = points;
        this.detailedPrice = detailedPrice;
        this.illustration = illustration;
    }

    public void InitNobleData(NobleController noble)
    {
        this.points = noble.points;
        this.detailedPrice = noble.detailedPrice;
        this.illustration = noble.illustration;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(illustration, points, detailedPrice);
    }

    public override bool Equals(object obj)
    {
        if (obj is not NobleController card)
            return false;

        return this.illustration == card.illustration &&
               this.points == card.points &&
               this.detailedPrice.Equals(card.detailedPrice);
    }
    public void Init(int points, ResourcesController detailedPrice, string illustration)
    {
        this.points = points;
        this.detailedPrice = detailedPrice;
        this.illustration = illustration;
    }
}

