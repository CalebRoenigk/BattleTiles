using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using DG.Tweening;

public class PlayerHandVisual : MonoBehaviour
{
    public Hand Hand;
    
    [SerializeField] private List<TileVisual> _tileVisuals = new List<TileVisual>();
    [SerializeField] private GameObject _visualTilePrefab;

    [Header("Settings")]
    [SerializeField] private float _perferedSpacing = 0.2f;
    [SerializeField] private float _handScale;
    [SerializeField] private float _staggerDepth = 0.4f;

    [Header("State")]
    public bool Interactable = false;
    [SerializeField] private bool _viewingState = false;

    public void SetHand(Hand hand)
    {
        Hand = hand;
        Hand.HandVisual = this;
        transform.localScale = Vector3.one * _handScale;
        gameObject.name = "Player " + Hand.Owner.Index;
    }

    public void AddTile(Tile tile)
    {
        TileVisual visualTile = Instantiate(_visualTilePrefab, transform.position, Quaternion.identity, transform).GetComponent<TileVisual>();
        visualTile.transform.localPosition = new Vector3(0f, -2f, 0f);
        visualTile.SetTile(tile);
        if (!_viewingState)
        {
            // The tile was added to a hand who is currently not viewed. Force the tile to be invisible
            visualTile.SetVisibility(0f, true);
        }
        RefreshTileVisualList();
        UpdateHandVisuals();
    }

    private void SetViewingState(bool state)
    {
        if (state)
        {
            UpdateHandVisuals();
        }

        _viewingState = state;
    }
    
    public List<Vector3> GetHandPositions()
    {
        int tileCount = Hand.Tiles.Count;
        float stepSize = HandViewManager.Instance.HandTargetWidth / (tileCount - 1);
        stepSize = Mathf.Min(stepSize, 1f + _perferedSpacing);
        bool mustStagger = stepSize < 1f + _perferedSpacing;
        float sizeDifference = HandViewManager.Instance.HandTargetWidth - (stepSize * tileCount);
        Vector3 centeringOffset = new Vector3(-(stepSize * tileCount) / 2f, 0f, 0f);
        
        List<Vector3> handPositions = new List<Vector3>();
        for (int i = 0; i < tileCount; i++)
        {
            float t = i / (float)(tileCount-1);
            Vector3 point = Vector3.Lerp(Vector3.zero, new Vector3(stepSize * tileCount, 0f, 0f), t) + centeringOffset;
        
            if (mustStagger && i % 2 != 0)
            {
                point.z += _staggerDepth;
            }
            else
            {
                point.z = 0;
            }
            
            handPositions.Add(point);
        }

        return handPositions;
    }

    public void UpdateHandVisuals()
    {
        List<Vector3> handPositions = GetHandPositions();
        transform.localScale = Vector3.one * _handScale;
        for (int i = 0; i < Hand.Tiles.Count; i++)
        {
            TileVisual visualTile = Hand.Tiles[i].TileVisual;
            float delay = 0.05f * i;
            Vector3 localPos = visualTile.transform.localPosition;
            localPos.z = handPositions[i].z;
            visualTile.transform.localPosition = localPos;
            visualTile.transform.DOKill();
            visualTile.transform.DOLocalMoveX( handPositions[i].x, 0.25f).SetEase(Ease.OutCubic).SetDelay(delay);
            visualTile.transform.DOLocalRotate(new Vector3(90f, 0f, 0f), 0.25f).SetEase(Ease.OutCubic).SetDelay(delay);
        }
    }

    // Shows the player hand in-front of the camera
    public void ShowHand()
    {
        EnableTiles();
        SetViewingState(true);
        // Delay this by 0.25f
        // Move the transform of each tile down by _handScale * 2f
        float staggerDelay = 0.035f;
        int i = 0;
        foreach (var tileVisual in _tileVisuals)
        {
            tileVisual.transform.DOLocalMoveY(0f, 0.175f).SetEase(Ease.OutQuad).SetDelay(0.25f + (staggerDelay * i));
            i++;
        }
        Sequence animateOnSequence = DOTween.Sequence();
        animateOnSequence.InsertCallback(0.175f + (staggerDelay * _tileVisuals.Count), () => SetInteractableState(true));
    }

    // Hides the player hand in-front of the camera
    public void HideHand()
    {
        SetInteractableState(false);
        // Move the transform of each tile down by _handScale * 2f
        foreach (var tileVisual in _tileVisuals)
        {
            tileVisual.transform.DOLocalMoveY(-2f, 0.175f).SetEase(Ease.InQuad);
        }
        Sequence animateOffSequence = DOTween.Sequence();
        animateOffSequence.InsertCallback(0.175f, () => SetViewingState(false));
        animateOffSequence.InsertCallback(0.175f, DisableTiles);
    }

    // Enables all tiles in the hand
    private void EnableTiles()
    {
        foreach (var tileVisual in _tileVisuals)
        {
            tileVisual.SetVisibility(1f);
        }
    }
    
    // Disable all tiles in the hand
    private void DisableTiles()
    {
        foreach (var tileVisual in _tileVisuals)
        {
            tileVisual.SetVisibility(0f);
        }
    }

    // Updates the tile visual list
    public void RefreshTileVisualList()
    {
        _tileVisuals.Clear();

        foreach (Tile tile in Hand.Tiles)
        {
            _tileVisuals.Add(tile.TileVisual);
        }
    }
    
    // Sets the interaction state
    private void SetInteractableState(bool state)
    {
        Interactable = state;
        if (!Interactable)
        {
            foreach (TileVisual tileVisual in _tileVisuals)
            {
                tileVisual.Selected = false;
            }
        }

        GameManager.Instance.UpdateSelection(null);
    }
}
