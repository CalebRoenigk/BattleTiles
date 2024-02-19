using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ListUtility
{
    // Shuffles the order of a list via a seeded random sort
    public static void Shuffle<T>(List<T> list, int seed = 0)
    {
        System.Random rng = new System.Random(seed);
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    
    public static bool AnyCommonValue(List<int> listA, List<int> listB)
    {
        return listA.Any(v => listB.Contains(v));
    }
    
    public static string ConvertListToString<T>(List<T> list)
    {
        return string.Join(", ", list);
    }
}
