using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Board
{
    public BoardVisual BoardVisual;
    
    public Tile Root;
    public List<Tile> OpenTiles = new List<Tile>(); // Cache
    public List<Interface> OpenInterfaces = new List<Interface>(); // Cache
    public List<int> OpenValues = new List<int>(); // Cache

    private bool _contentsChanged = false;

    public Board()
    {
        BoardVisual = BoardVisual.Instance;
    }

    public void PlaceRoot(Tile tile)
    {
        Root = tile;
        OpenTiles.Add(tile);
        _contentsChanged = true;
        tile.Owner.Hand.HandVisual.PlaceTile(tile, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
        UpdateCache();
    }

    // Updates the data cache of the board
    public void UpdateCache()
    {
        // Debug.Log("Board Attempting to Update Cache!");
        if (_contentsChanged)
        {
            // Debug.Log("Cache out of date, refreshing!");
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

            // Debug.Log("Cache Refreshed!");
            _contentsChanged = false; 
        }
    }

    public List<Interface> GetMatchingOpenInterfaces(Interface inputInterface)
    {
        return OpenInterfaces.Where(i => i.Value == inputInterface.Value && i.IsOpen()).ToList();
    }

    public void AddTile(Interface placedInterface, Interface connectedInterface)
    {
        placedInterface.ConnectInterface(connectedInterface);
        _contentsChanged = true;
        UpdateCache();
    }

    public void UpdateTileParent(Tile tile)
    {
        tile.TileVisual.gameObject.layer = 7;
        CameraManager.Instance.AddTileToBoardTargets(tile);
        BoardVisual.AddTile(tile);
    }

    public void ClearGhosts()
    {
        BoardVisual.ClearGhosts();
    }

    public void DrawGhosts(Tile tile)
    {
        // Clear Ghosts First
        BoardVisual.ClearGhosts(true);
        
        // Filter the open interfaces on the board to match with this one
        List<Interface> topMatches = GetMatchingOpenInterfaces(tile.Interfaces.Find(i => i.Side == TileSide.Top));
        List<Interface> bottomMatches = GetMatchingOpenInterfaces(tile.Interfaces.Find(i => i.Side == TileSide.Bottom));

        if (tile.IsDouble())
        {
            bottomMatches.Clear();
        }

        if (topMatches.Count > 0 || bottomMatches.Count > 0)
        {
            // Cull matches to a distinct list
            List<Interface> allMatches = new List<Interface>();
            allMatches.AddRange(topMatches);
            allMatches.AddRange(bottomMatches);

            allMatches.Distinct();

            BoardVisual.DrawGhosts(tile, allMatches);
        }
    }
}
