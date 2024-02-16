using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tile
{
    public Player Owner;
    public TileVisual TileVisual;
    public List<Interface> Interfaces = new List<Interface>();

    public Tile(int valueTop, int valueBottom, TileVisual tileVisual = null)
    {
        Interfaces.Add(new Interface(this, valueTop, TileSide.Top));
        Interfaces.Add(new Interface(this, valueBottom, TileSide.Bottom));

        if (valueTop == valueBottom)
        {
            Interfaces.Add(new Interface(this, valueTop, TileSide.Left));
            Interfaces.Add(new Interface(this, valueBottom, TileSide.Right));
        }

        TileVisual = tileVisual;
    }

    public void SetOwner(Player player)
    {
        Owner = player;
    }

    public Vector2Int ValuesAsVector()
    {
        int topValue = Interfaces.Find(i => i.Side == TileSide.Top).Value;
        int bottomValue = Interfaces.Find(i => i.Side == TileSide.Bottom).Value;
        return new Vector2Int(topValue, bottomValue);
    }

    public bool IsDouble()
    {
        return Interfaces.FindIndex(i => i.Side == TileSide.Left) != -1;
    }

    public bool IsEqual(Tile tile)
    {
        Vector2Int values = ValuesAsVector();
        Vector2Int compareValues = tile.ValuesAsVector();

        return values == compareValues || values == new Vector2Int(compareValues.y, compareValues.x);
    }

    public List<Interface> GetOpenInterfaces()
    {
        return Interfaces.Where(i => i.IsOpen()).ToList();
    }

    public List<Interface> GetMatchingInterfaces(Interface inputInterface, bool openOnly = false)
    {
        return openOnly
            ? Interfaces.Where(i => i.Value == inputInterface.Value && i.IsOpen()).ToList()
            : Interfaces.Where(i => i.Value == inputInterface.Value).ToList();
    }

    public bool HasOpenInterfaces()
    {
        return GetOpenInterfaces().Count > 0;
    }
    
    // Method to return a list of all tiles connected to this tile
    public List<Tile> GetConnectedTiles()
    {
        List<Tile> connectedTiles = new List<Tile>();
        HashSet<Tile> visited = new HashSet<Tile>();

        // Start DFS traversal from each interface
        foreach (var tileInterface in Interfaces)
        {
            if (!visited.Contains(tileInterface.Parent))
            {
                DFS(tileInterface.Parent, connectedTiles, visited);
            }
        }

        return connectedTiles;
    }

    // Depth-first search to traverse connected tiles
    private void DFS(Tile currentTile, List<Tile> connectedTiles, HashSet<Tile> visited)
    {
        visited.Add(currentTile);
        connectedTiles.Add(currentTile);

        foreach (var tileInterface in currentTile.Interfaces)
        {
            if (tileInterface.Connected != null && !visited.Contains(tileInterface.Connected.Parent))
            {
                DFS(tileInterface.Connected.Parent, connectedTiles, visited);
            }
        }
    }
    
    // Returns true if the total value of this tile is greater than the passed tile value
    public bool IsGreater(Tile comparison)
    {
        return SumTileValue() > comparison.SumTileValue();
    }

    public int SumTileValue()
    {
        Vector2Int values = ValuesAsVector();
        return values.x + values.y;
    }
}
