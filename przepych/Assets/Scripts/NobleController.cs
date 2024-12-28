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
    private Image selectedNobleImage;
    [SerializeField]
    private Image playerImage;   
    public PlayerController assignedPlayer;
    public string detailedPriceInfo = "";

    public void Awake()
    {    
        playerImage.gameObject.SetActive(false);
        assignedPlayer = null;
          
    }   
    private void Update()
    {
        if(assignedPlayer != null)
        {

            // trzeba zainicjalizować awatary w PlayerController
            // trzeba zrobić Noble Stack Controllera aby kontrollować noble na boardzie w grze
            //Sprite playerAvatar = assignedPlayer.avatar;
            //if (playerAvatar != null)
            //{
            //    playerImage.GetComponent<Image>().sprite = playerAvatar;
            //}               
            playerImage.gameObject.SetActive(true);            
        }      
    }
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

