using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noble : MonoBehaviour
{
    private int points;
    public int Points
    {
        get => points;
        set => points = value;
    }
    private Resources requiredBonuses;

    public Resources RequiredBonuses
    {
        get => requiredBonuses;
        set => requiredBonuses = value;
    }
    private string illustration;

    public override string ToString()
    {
        return $"Arystokrata dodaj¹cy {points} punktów.";
    }
}
