using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Resources : MonoBehaviour, IEnumerable<KeyValuePair<GemColor, int>>
{
    public Dictionary<GemColor, int> gems = new Dictionary<GemColor, int>();
    public override bool Equals(object obj)
    {
        if (obj.GetType() != typeof(Resources))
            return false;

        if (obj == null)
            return false;

        Resources otherResources = (Resources)obj;

        if (gems.Count != otherResources.gems.Count)
            return false;

        foreach (var kvp in gems)
        {
            if (!otherResources.gems.TryGetValue(kvp.Key, out int otherValue) || kvp.Value != otherValue)
                return false;
        }
        return true;
    }
    public override int GetHashCode()
    {
        int hash = 17;
        foreach (var kvp in gems)
        {
            hash = HashCode.Combine(hash, kvp.Key, kvp.Value);
        }
        return hash;
    }
    public override string ToString()
    {
        return string.Join(", ", gems.Select(gem => $"{gem.Key}: {gem.Value}"));
    }
    public void AddResource(GemColor color)
    {
        if (gems.ContainsKey(color))
        {
            gems[color] += 1;
        }
        else
        {
            gems[color] = 1;
        }

        Console.WriteLine($"Dodano 1 zasób koloru {color}. £¹czna iloœæ: {gems[color]}.");
    }
    public IEnumerator<KeyValuePair<GemColor, int>> GetEnumerator()
    {
        return gems.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

