using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourcesController : MonoBehaviour, IEnumerable<KeyValuePair<GemColor, int>>
{
    public Dictionary<GemColor, int> gems = new Dictionary<GemColor, int>();

    public override bool Equals(object obj)
    {
        if (obj.GetType() != typeof(ResourcesController))
            return false;

        if (obj == null)
            return false;

        ResourcesController otherResources = (ResourcesController)obj;

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
    }
    public void RemoveResource(GemColor color, int amount)
    {
        if (gems.TryGetValue(color, out int currentAmount))
        {
            gems[color] = Mathf.Max(0, currentAmount - amount);
        }
        else
        {
            Debug.LogWarning($"Próba usunięcia zasobów koloru {color}, którego nie ma w zasobach.");
        }
    }

    public void AddResource(GemColor color, int amount)
    {
        gems[color] += amount;
    }
    public int GetResourceAmount(GemColor color)
    {
        if (gems.TryGetValue(color, out int amount))
        {
            return amount;
        }
        return 0;
    }
    public IEnumerator<KeyValuePair<GemColor, int>> GetEnumerator()
    {
        return gems.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void FillDictionaryWithZeros()
    {
        foreach (GemColor color in Enum.GetValues(typeof(GemColor)))
        {
            this.gems.Add(color, 0);
        }
    }
}
