using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BoardVisual : MonoBehaviour
{
    public static BoardVisual Instance;

    public Board Board;

    [SerializeField] private Transform _camParent;
    [SerializeField] private Transform _placedParent;
    [SerializeField] private Transform _ghostParent;
    [SerializeField] private Transform _effectsParent;
    [SerializeField] private GameObject _ghostPrefab;
    [SerializeField] private GameObject _tileGlowEffect;
    [SerializeField] private List<TileGhostVisual> _ghostVisuals = new List<TileGhostVisual>();


    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    public void ClearGhosts(bool forceInstantClear = false)
    {
        foreach (TileGhostVisual ghostVisual in _ghostVisuals)
        {
            if (!forceInstantClear)
            {
                ghostVisual.SetVisibility(0f);
            }
            Destroy(ghostVisual.gameObject, forceInstantClear ? 0f : 0.2f);
        }
        
        _ghostVisuals.Clear();
    }

    public void DrawGhosts(Tile tile, List<Interface> matchedInterfaces)
    {
        matchedInterfaces = matchedInterfaces.Distinct().ToList();
        
        // Spawn ghosts at their matched interfaces
        foreach (Interface matchedInterface in matchedInterfaces)
        {
            Vector3 placementPosition = matchedInterface.GetPlacementPosition();
            TileGhostVisual tileGhostVisual = Instantiate(_ghostPrefab, placementPosition, Quaternion.identity, _ghostParent).GetComponent<TileGhostVisual>();
            tileGhostVisual.SetTile(tile, matchedInterface);
            
            // Align the ghosts properly
            placementPosition = matchedInterface.GetPlacementPosition();
            Interface placedInterface = tile.GetMatchingInterfaces(matchedInterface)[0];
            Quaternion placementRotation = placedInterface.GetOrientationTowards(placementPosition, matchedInterface);
            tileGhostVisual.transform.rotation = placementRotation;

            // Add the ghost visual to the visuals list
            _ghostVisuals.Add(tileGhostVisual);
        }
    }

    public void SetPlacementParent(Transform transform)
    {
        transform.parent = _placedParent;
    }

    public void SpawnGlow(Vector3 position, Quaternion rotation, TileGlowProperties glowProps, Interface spawnInterface)
    {
        Instantiate(_tileGlowEffect, position, rotation, _effectsParent).GetComponent<TileGlowEffect>().DoGlowAnimation(spawnInterface.Side, glowProps.BaseColor, glowProps.FadeColor);
    }
}
