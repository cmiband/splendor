using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bank : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Resources resources = new Resources();

    public void TakeOutResources(int amount, GemColor color)
    {
        this.resources.gems[color] -= amount;
    }

    public void AddResources(int amount, GemColor color)
    {
        this.resources.gems[color] += amount;
    }

    public void AddGoldenGem()
    {
        this.resources.gems[GemColor.GOLDEN]++;
    }
}
