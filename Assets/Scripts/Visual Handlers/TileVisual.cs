using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TileVisual : MonoBehaviour
{
    public Tile Tile;

    [SerializeField] private Material _material;

    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
    }

    private void OnMouseEnter()
    {
        if (Tile.IsInHand() && Tile.Owner.Hand.HandVisual.Interactable)
        {
            transform.DOLocalMoveY(0.5f, 0.15f).SetEase(Ease.OutCubic);
        }
    }

    private void OnMouseExit()
    {
        if (Tile.IsInHand() && Tile.Owner.Hand.HandVisual.Interactable)
        {
            transform.DOLocalMoveY(0f, 0.15f).SetEase(Ease.OutCubic);
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
}
