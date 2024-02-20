using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class TileVisual : MonoBehaviour
{
    public Tile Tile;

    public bool Selected = false;
    
    [Header("Settings")]
    [SerializeField] private float _placementDuration = 0.4f;
    [SerializeField] private LayerMask _ghostLayerMask;
    
    [Header("References")]
    [SerializeField] private Material _material;
    [SerializeField] private BoxCollider _collider;
    
    [Header("Runtime")]
    [SerializeField] private bool _drawGizmos = false;
    [SerializeField] private bool _isDragging = false;
    [SerializeField] private bool _isOnGhost = false;
    [SerializeField] private TileGhostVisual _hoveredGhost;

    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        if (_isDragging)
        {
            TileGhostVisual ghost = CheckForGhost();
            
            if (ghost == null && _hoveredGhost != null)
            {
                _hoveredGhost.SetGhostSelected(false);
            }

            if (GhostChange(ghost))
            {
                _isOnGhost = ghost != null;

                if (_isOnGhost)
                {
                    Quaternion facing = Tile.GetMatchingInterfaces(BoardVisual.Instance.Board.PrimaryMatch, true)[0].GetOrientationTowards(ghost.transform.position, BoardVisual.Instance.Board.PrimaryMatch);
                    transform.DORotate(facing.eulerAngles, 0.25f).SetEase(Ease.OutSine);
                    transform.DOScale(Vector3.one * (1f / Tile.Owner.Hand.HandVisual.transform.localScale.x), 0.25f).SetEase(Ease.OutSine);
                }

                if (!_isOnGhost)
                {
                    transform.DOLocalRotate(new Vector3(90f,0f,0f), 0.25f).SetEase(Ease.OutSine);
                    transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutSine);
                }
            }
            _hoveredGhost = ghost;
            
            transform.position = Vector3.Lerp(transform.position, _isOnGhost ? (ghost.transform.position + Vector3.up*0.75f) : CameraManager.Instance.MouseWorldPosition, 12f*Time.deltaTime);

            if (_isOnGhost)
            {
                _hoveredGhost.SetGhostSelected(true);
            }
        }
    }

    private void OnMouseEnter()
    {
        if (Tile.IsInHand() && Tile.Owner.Hand.HandVisual.Interactable && !_isDragging)
        {
            transform.DOLocalMoveY(0.5f, 0.15f).SetEase(Ease.OutCubic);
            Selected = true;
            GameManager.Instance.UpdateSelection(Tile);
        }
    }

    private void OnMouseExit()
    {
        if (Tile.IsInHand() && Tile.Owner.Hand.HandVisual.Interactable && !_isDragging)
        {
            transform.DOLocalMoveY(0f, 0.15f).SetEase(Ease.OutCubic);
            Selected = false;
            GameManager.Instance.UpdateSelection(null);
        }
    }

    private void OnMouseDrag()
    {
        _isDragging = true;
    }

    private void OnMouseUp()
    {
        _isDragging = false;
        if (_isOnGhost)
        {
            // Place the tile on the ghost
            GameManager.Instance.TryPlaceTile(Tile);
        }
        else
        {
            Tile.Owner.Hand.HandVisual.UpdateHandVisuals();
        }
        Tile.Owner.Hand.HandVisual.SetTileDragging(null);
    }

    private void OnMouseDown()
    {
        // Drag has started
        _isDragging = true;
        Tile.Owner.Hand.HandVisual.SetTileDragging(this);
    }

    private void OnDrawGizmos()
    {
        // if (_isDragging)
        // {
        //     Gizmos.color = Color.yellow;
        //     Vector3 rayDirection = CameraManager.Instance.MouseWorldPosition - CameraManager.Instance.MainCamera.transform.position;
        //     Gizmos.DrawRay(CameraManager.Instance.MainCamera.transform.position, rayDirection.normalized*1000f);
        //     
        //     Plane hPlane = new Plane(Vector3.up, Vector3.zero);
        //     Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);
        //     float distance = 0; 
        //     // if the ray hits the plane...
        //     if (hPlane.Raycast(ray, out distance)){
        //         // get the hit point:
        //         Gizmos.DrawSphere(ray.GetPoint(distance), 0.1f);
        //     }
        // }
        
        if (_drawGizmos)
        {
            Handles.color = Color.white;
            foreach (Interface tileInterface in Tile.Interfaces)
            {
                Handles.Label(tileInterface.GetPosition(), tileInterface.Side.ToString());
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_drawGizmos)
        {
            // Draw all Interfaces and their states
            foreach (Interface tileInterface in Tile.Interfaces)
            {
                Gizmos.color = tileInterface.Open ? Color.green : Color.red;
                Gizmos.matrix = Tile.TileVisual.transform.localToWorldMatrix;
                Vector3 interfacePosition = tileInterface.Center*0.5f;
                Vector3 gizmoSize = new Vector3(0.5f, 0.1875f, 1f);
                if (tileInterface.Side == TileSide.Left || tileInterface.Side == TileSide.Right)
                {
                    gizmoSize = new Vector3(1f, 0.1875f, 0.5f);
                }
            
                if (tileInterface.Open)
                {
                    Gizmos.DrawWireCube(interfacePosition, gizmoSize);
                }
                else
                {
                    Gizmos.DrawCube(interfacePosition, gizmoSize);
                }
                
                Gizmos.matrix = Matrix4x4.identity;

                // Draw connection between interfaces
                if (!tileInterface.Open)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawLine(tileInterface.GetPosition(), tileInterface.Connected.GetPosition());
                }
            }
            
        }
    }

    public void SetTile(Tile tile)
    {
        Tile = tile;
        Tile.TileVisual = this;
        Vector2Int tileValues = Tile.ValuesAsVector();
        _material.SetVector("_Values", new Vector4(tileValues.x, tileValues.y, 0, 0));
        gameObject.name = Tile.ValuesAsString();
    }

    public void SetVisibility(float visibility, bool forceInstant = false)
    {
        _material.DOFloat(visibility, "_Visibility", forceInstant ? 0f : 0.2f);
    }
    
    // When the tile is placed reformat the collider so that it matches the tile
    public void ConfigurePlacedTile()
    {
        _collider.center = Vector3.zero;
        _collider.size = new Vector3(1f, 0.1875f, 1f);
        _collider.isTrigger = false;
        _drawGizmos = true;
    }

    // Place the tile on the board
    public void RelocateTile(Vector3 position, Quaternion rotation)
    {
        SetVisibility(1f);
        ConfigurePlacedTile();
        gameObject.layer = GameManager.Instance.Board.TileMask;
        gameObject.tag = "Board Tile";
        
        transform.DOKill();

        Sequence placementSequence = DOTween.Sequence();
        placementSequence.Append(transform.DOMove(position, _placementDuration).SetEase(Ease.InOutQuad));
        placementSequence.Insert(0f, transform.DORotate(rotation.eulerAngles, _placementDuration).SetEase(Ease.InOutQuad));
        placementSequence.Insert(0f, transform.DOScale(1f, _placementDuration).SetEase(Ease.InOutQuad));
        placementSequence.AppendCallback(GameManager.Instance.EndTurn);
    }

    private TileGhostVisual CheckForGhost()
    {
        // Get the position of the mouse intersection with the ground plane
        Vector3 groundCast = GetMouseRayOnGround();

        float ghostRadiusCheck = 0.6f;
        
        Collider[] hitColliders = Physics.OverlapSphere(groundCast, ghostRadiusCheck, _ghostLayerMask);
        TileGhostVisual closestGhost = null;
        float closestDistance = ghostRadiusCheck;
        foreach (var hitCollider in hitColliders)
        {
            // Check if the collider is a ghost
            if (hitCollider.transform.gameObject.CompareTag("Ghost"))
            {
                // Hit a ghost
                float ghostDist = Vector3.Distance(hitCollider.transform.position, groundCast);
                if (ghostDist < closestDistance)
                {
                    closestGhost = hitCollider.transform.GetComponent<TileGhostVisual>();
                    closestDistance = ghostDist;
                }
            }
        }

        return closestGhost;
    }

    // Determines if the collider should be enabled or not
    public void SetEnabled(bool state)
    {
        _collider.enabled = state;
    }

    private Vector3 GetMouseRayOnGround()
    {
        Vector3 rayDirection = CameraManager.Instance.MouseWorldPosition - CameraManager.Instance.MainCamera.transform.position;
        Plane hPlane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);
        float distance = 0;
        if (hPlane.Raycast(ray, out distance)){
            // get the hit point:
            return ray.GetPoint(distance);
        }

        return CameraManager.Instance.MainCamera.transform.position;
    }

    private bool GhostChange(TileGhostVisual newGhost)
    {
        // If no ghost to ghost
        if (!_isOnGhost && newGhost != null)
        {
            GameManager.Instance.Board.SetPrimaryMatch(newGhost.MatchedInterface);
            return true;
        }
        
        // If ghost to no ghost
        if (_isOnGhost && newGhost == null)
        {
            GameManager.Instance.Board.SetPrimaryMatch(null);
            return true;
        }
        
        // If on ghost to different ghost
        if (_isOnGhost && newGhost != _hoveredGhost && _hoveredGhost != null)
        {
            GameManager.Instance.Board.SetPrimaryMatch(newGhost.MatchedInterface);
            return true;
        }

        return false;
    }
}
