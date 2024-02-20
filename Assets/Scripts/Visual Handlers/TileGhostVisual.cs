using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TileGhostVisual : MonoBehaviour
{
    public Tile Tile;
    public Interface MatchedInterface;

    [SerializeField] private Material _material;
    [SerializeField] private bool _setState;
    
    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        if (_setState && _material.GetFloat("_Fill") == 0)
        {
            _material.SetFloat("_Fill", 1f);
        }
        if (!_setState && _material.GetFloat("_Fill") == 1)
        {
            _material.SetFloat("_Fill", 0f);
        }
    }

    public void SetTile(Tile tile, Interface matchedInterface)
    {
        Tile = tile;
        MatchedInterface = matchedInterface;
        Vector2Int tileValues = Tile.ValuesAsVector();
        _material.SetVector("_Values", new Vector4(tileValues.x, tileValues.y, 0, 0));
        gameObject.name = "Ghost " + Tile.ValuesAsString();
    }

    public void SetGhostSelected(bool state)
    {
        if (state != _setState)
        {
            if (state)
            {
                
            }
            else
            {
                _material.SetFloat("_Fill", 0f);
            }
            
            _setState = state;
        }
    }

    public void SetVisibility(float visibility, bool forceInstant = false)
    {
        _material.DOFloat(visibility, "_Visibility", forceInstant ? 0f : 0.2f);
    }

    private void OnDrawGizmos()
    {
        // Gizmos.color = _setState ? Color.green : Color.red;
        // Gizmos.DrawWireCube(transform.position, new Vector3(1f, 0.1f, 1f));
    }
}
