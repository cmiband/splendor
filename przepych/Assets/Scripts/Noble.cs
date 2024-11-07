using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noble : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
