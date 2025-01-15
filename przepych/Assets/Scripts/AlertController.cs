using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertController : MonoBehaviour
{
    public GameObject InvalidOperationAlert;
    public GameObject NotEnoughGems;
    public GameObject TooManyReservedCards;
    void Start()
    {
        InvalidOperationAlert.SetActive(false);
        NotEnoughGems.SetActive(false);
        TooManyReservedCards.SetActive(false);
    }

    public void ShowInvalidOperationAlert()
    {
        InvalidOperationAlert.SetActive(true);
        StartCoroutine(HideAfterDelay(InvalidOperationAlert, 3f));
    }
    public void HideInvalidOperationAlert()
    {
        InvalidOperationAlert.SetActive(false);
    }
    public void ShowNotEnoughGems()
    {
        NotEnoughGems.SetActive(true);
        StartCoroutine(HideAfterDelay(NotEnoughGems, 3f));
    }
    public void HideNotEnoughGems()
    {
        NotEnoughGems.SetActive(false);
    }
    public void ShowTooManyReservedCards()
    {
        TooManyReservedCards.SetActive(true);
        StartCoroutine(HideAfterDelay(TooManyReservedCards, 3f));
    }
    public void HideTooManyReservedCards()
    {
        TooManyReservedCards.SetActive(false);
    }

    private IEnumerator HideAfterDelay(GameObject alert, float delay)
    {
        yield return new WaitForSeconds(delay);
        alert.SetActive(false);
    }
}
