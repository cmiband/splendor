using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    public ResourcesController detailedPrice;
    public int level;
    public GemColor bonusColor;
    public int points;
    public string illustration;
    public bool isSelected;
    private Image selectedCardImage;
    public int Points { get => points; }

    public int Level
    {
        get { return level; }
    }

    public ResourcesController DetailedPrice
    {
        get { return detailedPrice; }
    }

    public GemColor BonusColor
    {
        get => bonusColor;
    }
    public int Points2 { get; internal set; }

    public CardController(int level, GemColor bonusColor, int points, string illustration, ResourcesController detailedPrice)
    {
        this.detailedPrice = detailedPrice;
        this.level = level;
        this.bonusColor = bonusColor;
        this.points = points;
        this.illustration = illustration;
    }
    private void Start()
    {
        selectedCardImage = GetComponent<Image>();
    }
    public override bool Equals(object obj)
    {
        if (obj.GetType() != typeof(CardController))
            return false;

        CardController other = (CardController)obj;

        return level == other.level &&
               bonusColor == other.bonusColor &&
               points == other.points &&
               illustration == other.illustration &&
               detailedPrice.Equals(other.detailedPrice);
    }

    public override string ToString()
    {
        string priceDescription = DetailedPrice.ToString();
        return $"Karta koloru: {bonusColor}, cena: {priceDescription}, dodaj�ca {points} punkt�w.";
    }

    public void InitCardData(CardController card)
    {
        this.level = card.level;
        this.bonusColor = card.bonusColor;
        this.detailedPrice = card.detailedPrice;
        this.points = card.points;
        this.illustration = card.illustration;
    }
    private void OnMouseDown()
    {
        GameController gameController = FindObjectOfType<GameController>();
        if (gameController != null)
        {
            gameController.SelectCard(this);
        }
    }
    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (selectedCardImage != null)
        {
            selectedCardImage.color = selected ? Color.yellow : Color.white;
        }
        else
        {
            Debug.LogWarning("Nie znaleziono komponentu Image na karcie!");
        }
    }
}
