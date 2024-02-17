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
    [SerializeField] private float _handWidth;
    [SerializeField] private float _handScale;
    [SerializeField] private float _staggerDepth = 0.4f;

    [Header("State")]
    public bool Interactable = false;
    [SerializeField] private bool _viewingState = false;

    private void Update()
    {
        if (_viewingState)
        {
            SetTransformFromView();
        }
    }

    public void SetHand(Hand hand)
    {
        Hand = hand;
        Hand.HandVisual = this;
        CameraManager.Instance.GetHandSettings(out _handWidth, out _handScale);
        transform.localScale = Vector3.one * _handScale;
        gameObject.name = "Player " + Hand.Owner.Index;
    }

    public void AddTile(Tile tile)
    {
        TileVisual visualTile = Instantiate(_visualTilePrefab, transform.position, Quaternion.identity, transform).GetComponent<TileVisual>();
        visualTile.transform.localPosition = new Vector3(0f, -2f, 0f);
        visualTile.SetTile(tile);
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
        CameraManager.Instance.GetHandSettings(out _handWidth, out _handScale);
        float unscaledWidth = _handWidth;
        int tileCount = Hand.Tiles.Count;
        float stepSize = Mathf.Min(1f + _perferedSpacing, unscaledWidth / (tileCount - 1));
        bool mustStagger = stepSize < 1f + _perferedSpacing;
        Vector3 centeringOffset = Vector3.zero;
        
        if (stepSize * tileCount < unscaledWidth)
        {
            centeringOffset.x = (unscaledWidth - (stepSize * tileCount))/2f;
        }
        
        List<Vector3> handPositions = new List<Vector3>();
        Vector3 startPoint = new Vector3(-unscaledWidth / 2f, 0f, 0f);
        Vector3 endPoint = new Vector3(unscaledWidth / 2f, 0f, 0f);
        for (int i = 0; i < tileCount; i++)
        {
            float t = i / (float)(tileCount - 1);
            Vector3 point = Vector3.Lerp(startPoint, endPoint, t) + centeringOffset;
        
            if (mustStagger && i % 2 != 0)
            {
                point.z += _staggerDepth;
            }
            
            handPositions.Add(point);
        }

        return handPositions;
    }

    public void UpdateHandVisuals()
    {
        List<Vector3> handPositions = GetHandPositions();
        for (int i = 0; i < Hand.Tiles.Count; i++)
        {
            TileVisual visualTile = Hand.Tiles[i].TileVisual;
            float delay = 0.05f * i;
            Vector3 localPos = visualTile.transform.localPosition;
            localPos.z = handPositions[i].z;
            visualTile.transform.localPosition = localPos;
            visualTile.transform.DOMoveX(handPositions[i].x, 0.25f).SetEase(Ease.OutCubic).SetDelay(delay);
            visualTile.transform.DOLocalRotate(new Vector3(90f, 0f, 0f), 0.25f).SetEase(Ease.OutCubic).SetDelay(delay);
        }
    }

    public void PlaceTile(Tile tile, Vector3 position, Quaternion rotation)
    {
        Hand.RemoveTile(tile);
        tile.TileVisual.SetVisibility(1f);

        tile.TileVisual.transform.DOKill();
        tile.TileVisual.transform.DOMove(position, 0.375f).SetEase(Ease.InOutQuad);
        tile.TileVisual.transform.DORotate(rotation.eulerAngles, 0.375f).SetEase(Ease.InOutQuad);
        tile.TileVisual.transform.DOScale(1f, 0.375f).SetEase(Ease.InOutQuad);

        GameManager.Instance.Board.UpdateTileParent(tile);
        
        RefreshTileVisualList();
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
    
    // Sets the transform of the rotation and position given the camera view
    private void SetTransformFromView()
    {
        Vector3 position;
        Quaternion rotation;
        CameraManager.Instance.GetHandTransforms(out position, out rotation);
        transform.position = position;
        transform.rotation = rotation;
    }
    
    // Updates the tile visual list
    private void RefreshTileVisualList()
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
    }
}
