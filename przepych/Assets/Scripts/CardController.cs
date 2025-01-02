using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardController : MonoBehaviour, IPointerClickHandler
{
    public GameController gameController;
    public ResourcesController detailedPrice;
    public int level;
    public GemColor bonusColor;
    public int points;
    public string illustration;
    public bool isSelected;
    private Image selectedCardImage;
    public bool isReserved;

    public string priceInfo = "";

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

        this.priceInfo = "Price: " + detailedPrice.ToString();
    }

    private void Start()
    {
        selectedCardImage = GetComponent<Image>();
        if (selectedCardImage == null)
        {
            Debug.LogWarning("Nie znaleziono komponentu Image na karcie! Upewnij się, że karta ma komponent Image.");
        }

        this.priceInfo = "Price: " +this.detailedPrice.ToString();
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

        this.SetCardSprite();
    }

    private void SetCardSprite()
    {
        Sprite cardSprite = UnityEngine.Resources.Load<Sprite>(this.illustration);

        Image cardImage = this.gameObject.GetComponent<Image>();
        cardImage.sprite = cardSprite;
    }

    private void OnMouseDown()
    {
        GameController gameController = FindObjectOfType<GameController>();
        if (gameController != null)
        {
            gameController.SelectCard(this);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            SetSelected(!isSelected); 
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            this.gameController.ShowClickedCard(this);
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
