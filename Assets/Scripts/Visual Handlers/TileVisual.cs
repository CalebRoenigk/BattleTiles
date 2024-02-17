using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileVisual : MonoBehaviour
{
    public Tile Tile;

    [SerializeField] private Material _material;

    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
    }

    public void SetTile(Tile tile)
    {
        Tile = tile;
        Tile.TileVisual = this;
        Vector2Int tileValues = Tile.ValuesAsVector();
        _material.SetVector("_Values", new Vector4(tileValues.x, tileValues.y, 0, 0));
        gameObject.name = Tile.ValuesAsString();
    }
}
