using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hand
{
    public Player Owner;
    public PlayerHandVisual HandVisual;
    
    public List<Tile> Tiles = new List<Tile>();

    public Hand(Player owner)
    {
        Owner = owner;
    }

    public void AddTile(Tile tile)
    {
        tile.SetOwner(Owner);
        Tiles.Add(tile);
    }

    public void RemoveTile(Tile tile)
    {
        Tiles.Remove(tile);
        HandVisual.RefreshTileVisualList();
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

    public void DrawTile()
    {
        Tile drawnTile = GameManager.Instance.DrawTile();
        AddTile(drawnTile);
        HandVisual.AddTile(drawnTile);
    }

    // Instantly hide all tiles in the hand
    public void ForceHideHand()
    {
        foreach (Tile tile in Tiles)
        {
            tile.TileVisual.SetVisibility(0f, true);
        }
    }

    // Returns a list of values that are in the hand
    public List<int> GetHandValues()
    {
        List<int> values = new List<int>();
        foreach (Tile tile in Tiles)
        {
            values.AddRange(tile.GetValues());
        }

        return values.Distinct().ToList();
    }
}
