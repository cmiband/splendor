using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private Noble[] nobles;
    private static Noble[] visibleNobles;
    public static Noble[] VisibleNobles
    {
        get => visibleNobles;
        set => visibleNobles = value;
    }
    private List<Card> level1Deck = new List<Card>();
    private List<Card> level2Deck = new List<Card>();
    private List<Card> level3Deck = new List<Card>();
    private List<Card> level1VisibleCards;
    private List<Card> level2VisibleCards;
    private List<Card> level3VisibleCards;

    public Board(List<Card> level1VisibleCards, List<Card> level2VisibleCards, List<Card> level3VisibleCards, List<Card> level1Deck, List<Card> level2Deck, List<Card> level3Deck)
    {
        this.level1VisibleCards = level1VisibleCards;
        this.level2VisibleCards = level2VisibleCards;
        this.level3VisibleCards = level3VisibleCards;
        this.level1Deck = level1Deck;
        this.level2Deck = level2Deck;
        this.level3Deck = level3Deck;
    }

    public List<Card> Level1VisibleCards { get => level1VisibleCards; }
    public List<Card> Level2VisibleCards { get => level2VisibleCards; }
    public List<Card> Level3VisibleCards { get => level3VisibleCards; }
    public int GetCardIndexInVisibleCards(Card card, int level)
    {
        List<Card> visibleCards = GetVisibleCards(level);
        for (int i = 0; i < visibleCards.Count; i++)
        {
            if (card.Equals(visibleCards[i]))
                return i;
        }
        return -1;
    }
    public List<Card> GetVisibleCards(int? level)
    {
        return level switch
        {
            1 => Level1VisibleCards,
            2 => Level2VisibleCards,
            3 => Level3VisibleCards,
            _ => throw new ArgumentException("Niepoprawny poziom karty")
        };
    }
    public void ReplaceMissingCard(int level, Card card)
    {
        switch (level)
        {
            case 1:
                level1VisibleCards.Remove(card);
                if (level1Deck.Count > 1)
                {
                    level1VisibleCards.Add(level1Deck[0]);
                    level1Deck.Remove(level1Deck[0]);
                }
                break;
            case 2:
                level2VisibleCards.Remove(card);
                if (level2Deck.Count > 1)
                {
                    level2VisibleCards.Add(level2Deck[0]);
                    level2Deck.Remove(level2Deck[0]);
                }
                break;
            case 3:
                level3VisibleCards.Remove(card);
                if (level3Deck.Count > 1)
                {
                    level3VisibleCards.Add(level3Deck[0]);
                    level3Deck.Remove(level3Deck[0]);
                }
                break;
            default: throw new ArgumentException("Niepoprawny poziom karty");
        }

    }
}
