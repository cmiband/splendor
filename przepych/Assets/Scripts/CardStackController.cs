using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStackController : MonoBehaviour
{
    public List<CardController> cardsInStack = new List<CardController>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCardsInStack(List<CardController> cards)
    {
        this.cardsInStack = cards;

        Debug.Log("set card stack");
    }

    public CardController PopCardFromStack()
    {
        CardController targetedCard = this.cardsInStack[this.cardsInStack.Count - 1];
        this.cardsInStack.Remove(targetedCard);

        return targetedCard;
    }
}
