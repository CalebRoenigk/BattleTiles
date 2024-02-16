using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand
{
    public List<Tile> Tiles = new List<Tile>();

    public Hand()
    {
        
    }

    public void AddTile(Tile tile)
    {
        Tiles.Add(tile);
    }

    public Tile RemoveTile(Tile tile)
    {
        Tiles.Remove(tile);
        return tile;
    }

    public List<Interface> GetMatchingInterfaces(Interface inputInterface)
    {
        List<Interface> matchedInterfaces = new List<Interface>();
        foreach (Tile tile in Tiles)
        {
            matchedInterfaces.AddRange(tile.GetMatchingInterfaces(inputInterface));
        }

        return matchedInterfaces;
    }
    
    public Tile GetHighestInHand()
    {
        // More than one tile in hand, need to sort thru them
        if (Tiles.Count > 1)
        {
            Tile highest = Tiles[0];

            for (int i = 1; i < Tiles.Count; i++)
            {
                if (highest.IsGreater(Tiles[i]))
                {
                    highest = Tiles[i];
                }
            }

            return highest;
        }

        // Just one tile in hand, return it
        if (Tiles.Count == 1)
        {
            return Tiles[0];
        }

        // No tiles in hand, return null
        return null;
    }
}
