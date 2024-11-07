using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    private Resources detailedPrice;
    private int level;
    private GemColor bonusColor;
    private int points;
    private string illustration;
    public int Points { get => points; }

    public int Level
    {
        get { return level; }
    }

    public Resources DetailedPrice
    {
        get { return detailedPrice; }
    }

    public GemColor BonusColor
    {
        get => bonusColor;
    }
    public int Points2 { get; internal set; }

    public Card(int level, GemColor bonusColor, int points, string illustration, Resources detailedPrice)
    {
        this.detailedPrice = detailedPrice;
        this.level = level;
        this.bonusColor = bonusColor;
        this.points = points;
        this.illustration = illustration;
    }
    public override bool Equals(object obj)
    {
        if (obj.GetType() != typeof(Card))
            return false;

        Card other = (Card)obj;

        return level == other.level &&
               bonusColor == other.bonusColor &&
               points == other.points &&
               illustration == other.illustration &&
               detailedPrice.Equals(other.detailedPrice);
    }
    public override string ToString()
    {
        string priceDescription = DetailedPrice.ToString();
        return $"Karta koloru: {bonusColor}, cena: {priceDescription}, dodaj¹ca {points} punktów.";
    }
}

