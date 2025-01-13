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

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowInvalidOperationAlert()
    {
        InvalidOperationAlert.SetActive(true);  
    }
    public void HideInvalidOperationAlert()
    {
        InvalidOperationAlert.SetActive(false);
    }
    public void ShowNotEnoughGems()
    {
        NotEnoughGems.SetActive(true);
    }
    public void HideNotEnoughGems()
    {
        NotEnoughGems.SetActive(false);
    }
    public void ShowTooManyReservedCards()
    {
        TooManyReservedCards.SetActive(true);
    }
    public void HideTooManyReservedCards()
    {
        TooManyReservedCards.SetActive(false);
    }
}
