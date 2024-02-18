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
    
    [Header("References")]
    [SerializeField] private Material _material;
    [SerializeField] private BoxCollider _collider;
    
    [Header("Runtime")]
    [SerializeField] private bool _drawGizmos = false;

    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
    }

    private void OnMouseEnter()
    {
        if (Tile.IsInHand() && Tile.Owner.Hand.HandVisual.Interactable)
        {
            transform.DOLocalMoveY(0.5f, 0.15f).SetEase(Ease.OutCubic);
            Selected = true;
            GameManager.Instance.UpdateSelection(Tile);
        }
    }

    private void OnMouseExit()
    {
        if (Tile.IsInHand() && Tile.Owner.Hand.HandVisual.Interactable)
        {
            transform.DOLocalMoveY(0f, 0.15f).SetEase(Ease.OutCubic);
            Selected = false;
            GameManager.Instance.UpdateSelection(null);
        }
    }

    private void OnMouseDown()
    {
        // Try and place the tile
        GameManager.Instance.TryPlaceTile(Tile);
    }

    private void OnDrawGizmos()
    {
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
        gameObject.layer = GameManager.Instance.Board.BoardTileMask;
        gameObject.tag = "Board Tile";
        
        transform.DOKill();

        Sequence placementSequence = DOTween.Sequence();
        placementSequence.Append(transform.DOMove(position, _placementDuration).SetEase(Ease.InOutQuad));
        placementSequence.Insert(0f, transform.DORotate(rotation.eulerAngles, _placementDuration).SetEase(Ease.InOutQuad));
        placementSequence.Insert(0f, transform.DOScale(1f, _placementDuration).SetEase(Ease.InOutQuad));
        placementSequence.AppendCallback(GameManager.Instance.EndTurn);
    }
}
