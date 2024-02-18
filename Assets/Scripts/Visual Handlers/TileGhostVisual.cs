using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TileGhostVisual : MonoBehaviour
{
    public Tile Tile;

    [SerializeField] private Material _material;
    [SerializeField] private Color32 _ghostSelectedColor;

    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
    }

    public void SetTile(Tile tile, bool placementGhost = false)
    {
        Tile = tile;
        Vector2Int tileValues = Tile.ValuesAsVector();
        _material.SetVector("_Values", new Vector4(tileValues.x, tileValues.y, 0, 0));
        if (placementGhost)
        {
            _material.SetColor("_Fill", _ghostSelectedColor);
        }
        gameObject.name = "Ghost " + Tile.ValuesAsString();
    }

    public void SetVisibility(float visibility, bool forceInstant = false)
    {
        _material.DOFloat(visibility, "_Visibility", forceInstant ? 0f : 0.2f);
    }
}
