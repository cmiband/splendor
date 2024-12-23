using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickedCardController : MonoBehaviour, IPointerClickHandler
{
    public CardController targetedCard;

    public void SetCard(CardController card)
    {
        this.targetedCard = card;
        this.SetCardSprite(targetedCard.illustration);
    }

    private void SetCardSprite(string cardIllustration)
    {
        Sprite cardSprite = UnityEngine.Resources.Load<Sprite>(cardIllustration);

        Image cardImage = this.gameObject.GetComponent<Image>();
        cardImage.sprite = cardSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        this.gameObject.SetActive(false);
    }
}
