using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneyardNEW
{
    public Queue<Tile> Pile = new Queue<Tile>();
    private int _maxValue = 9; // The max count of a single value in the value pairs

    public BoneyardNEW()
    {
        
    }
    
    // Fills the Boneyard with a new set of randomly ordered Tiles
    private void FillPile()
    {
        List<Tile> tiles = new List<Tile>();
        
        // Generate and print all possible dominos
        for (int i = 0; i <= _maxValue; i++)
        {
            for (int j = i; j <= _maxValue; j++)
            {
                Tile tile = new Tile(i,j);
                if (tiles.FindIndex(t => t.IsEqual(tile))== -1)
                {
                    tiles.Add(tile);
                }
            }
        }
        
        // Shuffle the list randomly, seeded
        System.Random rng = new System.Random(Mathf.RoundToInt(UnityEngine.Random.Range(0, 10000000)));
        Shuffle(tiles, rng);
        
        // Add each item in the list to the queue
        tiles.ForEach(t => Pile.Enqueue(t));
    }

    // Returns a tile from the pile
    public Tile DrawTile()
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
