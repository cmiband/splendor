using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGemInfoController : MonoBehaviour
{
    public int amountOfGems;
    public Text amountInfo;

    public void SetAmountOfGems(int amountOfGems)
    {
        this.amountOfGems = amountOfGems;

        amountInfo.text = this.amountOfGems.ToString();
    }
}
