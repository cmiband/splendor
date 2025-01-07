using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardStackController : MonoBehaviour
{
    public List<CardController> cardsInStack = new List<CardController>();
    public bool isSelected;
    private Image selectedStackImage;

    public void SetCardsInStack(List<CardController> cards)
    {
        selectedStackImage = GetComponent<Image>();
        this.cardsInStack = cards;
    }

    public CardController PopCardFromStack()
    {
        CardController targetedCard = this.cardsInStack[this.cardsInStack.Count - 1];
        this.cardsInStack.RemoveAt(this.cardsInStack.Count - 1);

        return targetedCard;
    }
    public int ChceckCardsCount()
    {
        return this.cardsInStack.Count;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (selectedStackImage != null)
        {
            selectedStackImage.color = selected ? Color.blue : Color.white;
        }
        else
        {
            Debug.LogWarning("Nie znaleziono komponentu Image na karcie!");
        }
    }
}
