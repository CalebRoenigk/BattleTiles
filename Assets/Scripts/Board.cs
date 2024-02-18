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
    public Interface PrimaryMatch;
    public List<Interface> ActiveMatches = new List<Interface>();
    public int BoardTileMask = 7;

    private bool _contentsChanged = false;

    public Board()
    {
        BoardVisual = BoardVisual.Instance;
        BoardVisual.Board = this;
    }

    public void PlaceTile(Tile tile, bool isRoot = false)
    {
        BoardVisual.ClearGhosts();
        
        Vector3 placementPosition = Vector3.zero;
        Quaternion placementRotation = Quaternion.identity;
        if (isRoot)
        {
            Root = tile;
        }
        else
        {
            Interface placedInterface = tile.GetMatchingInterfaces(PrimaryMatch, true)[0];
            placedInterface.ConnectInterface(PrimaryMatch);
            placementPosition = PrimaryMatch.GetPlacementPosition();
            placementRotation = placedInterface.GetOrientationTowards(placementPosition, PrimaryMatch);
        }

        // Remove the tile from the hand
        tile.Owner.Hand.RemoveTile(tile);
        BoardVisual.SetPlacementParent(tile.TileVisual.transform);
        tile.TileVisual.RelocateTile(placementPosition, placementRotation);
        CameraManager.Instance.AddTileToBoardTargets(tile);

        // After placement do a check to see if any interfaces that are open now need to be closed given the newly placed tiles proximity to other tiles
        // TODO: Enable this and work thru it
        // CheckNearbyInterfaces(connectedInterface.GetPlacementPosition(), placedInterface.Parent);
        
        // Flag cache for update
        _contentsChanged = true;
        // Note that placing the tile has been changed to only flag the cache for updates, but no longer forces an update directly after placement (relying on the post-turn checks to do the cache update). This shouldn't be an issue given that post-turn checks fire directly after placement!
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

    public List<Interface> GetActiveMatches(Tile tile)
    {
        // Filter the open interfaces on the board to match with this one
        List<Interface> topMatches = GetMatchingOpenInterfaces(tile.Interfaces.Find(i => i.Side == TileSide.Top));
        List<Interface> bottomMatches = new List<Interface>();

        // If the tile is not a double the matches for top and bottom will be unique, find the bottom matches
        if (!tile.IsDouble())
        {
            bottomMatches = GetMatchingOpenInterfaces(tile.Interfaces.Find(i => i.Side == TileSide.Bottom));
        }

        List<Interface> activeMatches = new List<Interface>();
        if (topMatches.Count > 0 || bottomMatches.Count > 0)
        {
            // Cull matches to a distinct list
            activeMatches.AddRange(topMatches);
            activeMatches.AddRange(bottomMatches);

            activeMatches.Distinct();

            ListUtility.Shuffle(activeMatches, GameManager.Instance.PlayerTurn + GameManager.Instance.TurnCount);
        }

        return activeMatches;
    }

    // Called after a tile is placed, this method checks for any open interfaces nearby a placement that are now obscured by the placement and need to be marked as closed
    private void CheckNearbyInterfaces(Vector3 placementLocation, Tile tile)
    {
        // Find all tiles within a radius of the placed tile
        float checkRadius = 1.25f;
        Collider[] hitColliders = Physics.OverlapSphere(placementLocation, checkRadius, BoardTileMask);
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
    
    // Updates the selection of interfaces given a matching tile
    public void UpdateSelection(Tile tile)
    {
        // Clear the selection and selection list
        PrimaryMatch = null;
        ActiveMatches.Clear();
        
        if (tile == null)
        {
            // Clear Ghosts
            BoardVisual.ClearGhosts();
        }
        else
        {
            // Update the selection and selection list
            ActiveMatches = GetActiveMatches(tile);

            if (ActiveMatches.Count > 0)
            {
                PrimaryMatch = ActiveMatches[0];
                
                // Clear Ghosts First
                BoardVisual.ClearGhosts(true);
                
                // Draw new ghosts
                BoardVisual.DrawGhosts(tile, ActiveMatches);
            }
            else
            {
                BoardVisual.ClearGhosts();
            }
        }
    }
}
