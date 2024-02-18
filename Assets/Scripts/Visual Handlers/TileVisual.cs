using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TileVisual : MonoBehaviour
{
    public Tile Tile;

    public bool Selected = false;
    [SerializeField] private Material _material;
    [SerializeField] private BoxCollider _collider;
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
            Tile.Owner.Hand.HandVisual.UpdateSelection();
        }
    }

    private void OnMouseExit()
    {
        if (Tile.IsInHand() && Tile.Owner.Hand.HandVisual.Interactable)
        {
            transform.DOLocalMoveY(0f, 0.15f).SetEase(Ease.OutCubic);
            Selected = false;
            Tile.Owner.Hand.HandVisual.UpdateSelection();
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
            // Draw all Interfaces and their states
            Gizmos.matrix = Tile.TileVisual.transform.localToWorldMatrix;
            foreach (Interface tileInterface in Tile.Interfaces)
            {
                Gizmos.color = tileInterface.Open ? Color.green : Color.red;
                Vector3 interfacePosition = tileInterface.GetPosition(true)*0.5f;
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
}
