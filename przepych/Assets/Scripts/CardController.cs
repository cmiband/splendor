using System;
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
    public bool isReserved;

    public int Points => points;

    public int Level => level;

    public ResourcesController DetailedPrice => detailedPrice;

    public GemColor CardBonus => bonusColor;

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
        if (selectedCardImage == null)
        {
            Debug.LogWarning("Nie znaleziono komponentu Image na karcie! Upewnij się, że karta ma komponent Image.");
        }
    }

    public override bool Equals(object obj)
    {
        if (obj == null || obj.GetType() != typeof(CardController))
            return false;

        CardController other = (CardController)obj;

        return level == other.level &&
               bonusColor == other.bonusColor &&
               points == other.points &&
               illustration == other.illustration &&
               detailedPrice.Equals(other.detailedPrice);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(level, bonusColor, points, illustration, detailedPrice);
    }

    public override string ToString()
    {
        string priceDescription = DetailedPrice.ToString();
        return $"Karta koloru: {bonusColor}, cena: {priceDescription}, dodająca {points} punktów.";
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
