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
    public List<Interface> ActiveMatches = new List<Interface>();

    [SerializeField] private int _boardTileMask = 7;

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
        placedInterface.Parent.Owner.Hand.HandVisual.PlaceTile(placedInterface.Parent, connectedInterface.GetPlacementPosition(), Quaternion.Euler(0f, 0f, 0f));
        _contentsChanged = true;
        // After placement do a check to see if any interfaces that are open now need to be closed given the newly placed tiles proximity to other tiles
        CheckNearbyInterfaces(connectedInterface.GetPlacementPosition(), placedInterface.Parent);
        UpdateCache();
    }

    public void UpdateTileParent(Tile tile)
    {
        tile.TileVisual.gameObject.layer = _boardTileMask;
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
        ActiveMatches.Clear();

        ActiveMatches = GetActiveMatches(tile);

        if (ActiveMatches.Count > 0)
        {
            BoardVisual.DrawGhosts(tile, ActiveMatches);
        }
    }

    public List<Interface> GetActiveMatches(Tile tile)
    {
        // Filter the open interfaces on the board to match with this one
        List<Interface> topMatches = GetMatchingOpenInterfaces(tile.Interfaces.Find(i => i.Side == TileSide.Top));
        List<Interface> bottomMatches = GetMatchingOpenInterfaces(tile.Interfaces.Find(i => i.Side == TileSide.Bottom));

        if (tile.IsDouble())
        {
            bottomMatches.Clear();
        }

        List<Interface> activeMatches = new List<Interface>();
        if (topMatches.Count > 0 || bottomMatches.Count > 0)
        {
            // Cull matches to a distinct list
            activeMatches.AddRange(topMatches);
            activeMatches.AddRange(bottomMatches);

            activeMatches.Distinct();

            Shuffle(activeMatches, GameManager.Instance.PlayerTurn + GameManager.Instance.TurnCount);
        }

        return activeMatches;
    }

    public void ClearActiveMatches()
    {
        ActiveMatches.Clear();
    }
    
    // Called after a tile is placed, this method checks for any open interfaces nearby a placement that are now obscured by the placement and need to be marked as closed
    private void CheckNearbyInterfaces(Vector3 placementLocation, Tile tile)
    {
        // Find all tiles within a radius of the placed tile
        float checkRadius = 1.25f;
        Collider[] hitColliders = Physics.OverlapSphere(placementLocation, checkRadius, _boardTileMask);
        List<Interface> openInterfacesInRange = new List<Interface>();
        foreach (var hitCollider in hitColliders)
        {
            // Only check for tiles, filter via tag
            if (hitCollider.CompareTag("Board Tile"))
            {
                // Get a reference to the hit tile
                Tile hitTile = hitCollider.GetComponent<TileVisual>().Tile;
                
                // Avoid direct closures on the placed tile
                if (hitTile != tile)
                {
                    // Check for interfaces within that radius
                    foreach (Interface openInterface in hitTile.GetOpenInterfaces())
                    {
                        // If there are any open ones within that radius, close them
                        if (Vector3.Distance(openInterface.GetPosition(), placementLocation) <= checkRadius)
                        {
                            // Close this interface
                            openInterface.SetOpenState(false);
                            // TODO: Need to figure out a way to determine if when this interface closes the closest interface to this interface on the placed tile needs to also close? Maybe do this by closing any interface within 1.01f of the closed interface?
                            Debug.Log("Forcing Interface: " + openInterface.Parent.TileVisual.Tile.ValuesAsString() + " " + openInterface.Side + " to close due to proximity to placed tile " + tile.ValuesAsString());
                        }
                    } 
                }
            }
        }
    }
    
    // TODO: Move this method into its own class of list stuff, so it can be used more broadly
    private static void Shuffle<T>(List<T> list, int seed = 0)
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
}
