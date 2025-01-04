using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NobleController :  MonoBehaviour 
{
    public ResourcesController detailedPrice;
    public int points;
    public string illustration;
    public Image playerImage;   
    public PlayerController assignedPlayer;
    public string detailedPriceInfo = "";

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
        this.detailedPriceInfo = this.detailedPrice.ToString();
        this.playerImage.gameObject.SetActive(false);

        this.SetNobleSprite();
    }

    public void SetPlayerImage(Image playerImage)
    {
        this.playerImage.gameObject.SetActive(true);
        this.playerImage.sprite = playerImage.sprite;
    }

    private void SetNobleSprite()
    {
        Sprite nobleSprite = UnityEngine.Resources.Load<Sprite>(this.illustration);

        Image nobleImage = this.gameObject.GetComponent<Image>();
        nobleImage.sprite = nobleSprite;
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
        this.detailedPriceInfo = this.detailedPrice.ToString();
    }
}

