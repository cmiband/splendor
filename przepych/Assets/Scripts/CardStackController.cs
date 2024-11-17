using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStackController : MonoBehaviour
{
    public List<CardController> cardsInStack = new List<CardController>();

    public void SetCardsInStack(List<CardController> cards)
    {
        this.cardsInStack = cards;
    }

    public CardController PopCardFromStack()
    {
        CardController targetedCard = this.cardsInStack[this.cardsInStack.Count - 1];
        this.cardsInStack.RemoveAt(this.cardsInStack.Count - 1);

        return targetedCard;
    }
}
