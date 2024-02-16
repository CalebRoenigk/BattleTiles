using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardNEW
{
    public Tile Root;
    public List<Tile> OpenTiles = new List<Tile>(); // Cache
    public List<Interface> OpenInterfaces = new List<Interface>(); // Cache
    public List<int> OpenValues = new List<int>(); // Cache

    public BoardNEW()
    {
        
    }

    public void PlaceRoot(Tile tile)
    {
        Root = tile;
        OpenTiles.Add(tile);
        UpdateCache();
    }

    // Updates the data cache of the board
    public void UpdateCache()
    {
        // Update the Open Tiles Cache
        // Update the Open Interfaces Cache
        OpenTiles.Clear();
        OpenInterfaces.Clear();
        foreach (Tile tile in Root.GetConnectedTiles())
        {
            List<Interface> openInterfaces = tile.GetOpenInterfaces();
            if (openInterfaces.Count > 0)
            {
                OpenTiles.Add(tile);
                OpenInterfaces.AddRange(openInterfaces);
            }
            
        }
        
        // Update the Open Values Cache
        OpenValues.Clear();
        foreach (Interface openInterface in OpenInterfaces)
        {
            if (!OpenValues.Contains(openInterface.Value))
            {
                OpenValues.Add(openInterface.Value); 
            }
        }
    }

    public List<Interface> GetMatchingOpenInterfaces(Interface inputInterface)
    {
        return OpenInterfaces.Where(i => i.Value == inputInterface.Value && i.IsOpen()).ToList();
    }

    public void AddTile(Interface placedInterface, Interface connectedInterface)
    {
        placedInterface.ConnectInterface(connectedInterface);
        UpdateCache();
    }
}
