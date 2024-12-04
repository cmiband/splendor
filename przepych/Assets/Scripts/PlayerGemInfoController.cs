using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerGemInfoController : MonoBehaviour
{
    public int amountOfGems;
    public TextMeshProUGUI amountInfo;

    public void SetAmountOfGems(int amountOfGems)
    {
        this.amountOfGems = amountOfGems;

        amountInfo.SetText(this.amountOfGems.ToString());
    }
}
