using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boneyard
{
    public Queue<ValuePair> Pile = new Queue<ValuePair>();
    private int _maxValue = 9; // The max count of a single value in the value pairs

    // Fills the Boneyard with a new set of randomly ordered pairs
    private void FillPile()
    {
        List<ValuePair> valuePairs = new List<ValuePair>();
        
        // Generate and print all possible dominos
        for (int i = 0; i <= _maxValue; i++)
        {
            for (int j = i; j <= _maxValue; j++)
            {
                ValuePair pair = new ValuePair(i, j);
                if (valuePairs.FindIndex(p => p.IsEqual(pair))== -1)
                {
                    valuePairs.Add(pair);
                }
            }
        }
        
        // Shuffle the list randomly, seeded
        System.Random rng = new System.Random(Mathf.RoundToInt(UnityEngine.Random.Range(0, 10000000)));
        Shuffle(valuePairs, rng);
        
        // Add each item in the list to the queue
        valuePairs.ForEach(i => Pile.Enqueue(i));
    }

    // Returns a value pair from the pile
    public ValuePair DrawDomino()
    {
        if (Pile.Count <= 0)
        {
            FillPile();
        }
        
        return Pile.Dequeue();
    }
    
    private void Shuffle<T>(IList<T> list, System.Random rng)
    {
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
}
