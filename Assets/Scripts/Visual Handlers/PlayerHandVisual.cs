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
    [SerializeField] private float _openMaxLength = 8f;
    [SerializeField] private float _closeMaxLength = 5f;
    [SerializeField] private float _staggerDepth = 0.4f;

    [Header("State")]
    [SerializeField] private bool _viewingState = false;

    public void SetHand(Hand hand)
    {
        Hand = hand;
        Hand.HandVisual = this;
    }

    public void AddTile(Tile tile)
    {
        TileVisual visualTile = Instantiate(_visualTilePrefab, transform.position, Quaternion.identity, transform).GetComponent<TileVisual>();
        visualTile.SetTile(tile);
        UpdateHandVisuals();
    }

    public void SetViewingState(bool state)
    {
        if (state != _viewingState)
        {
            UpdateHandVisuals();
        }

        _viewingState = state;
    }
    
    public List<Vector3> GetHandPositions()
    {
        List<Vector3> handPositions = new List<Vector3>();
        float maxLength = _viewingState ? _openMaxLength : _closeMaxLength;
        float maxLengthHalf = maxLength / 2f;
        Vector3 startPoint = new Vector3(-maxLengthHalf, 0f, 0f);
        Vector3 endPoint = new Vector3(maxLengthHalf, 0f, 0f);
        int tileCount = Hand.Tiles.Count;
        float stepSize = Mathf.Min(1f + _perferedSpacing, Vector3.Distance(startPoint, endPoint) / (tileCount - 1));
        bool mustStagger = stepSize < 1f + _perferedSpacing;
        Vector3 centeringOffset = Vector3.zero;
        if (stepSize * tileCount < (maxLength))
        {
            centeringOffset.x = (maxLength - (stepSize * tileCount))/2f;
        }
        
        for (int i = 0; i < tileCount; i++)
        {
            float t = i / (float)(tileCount - 1);
            Vector3 point = Vector3.Lerp(startPoint, endPoint, t) + centeringOffset;
    
            if (mustStagger && i % 2 != 0)
            {
                point.z += 0.8f;
            }
            
            handPositions.Add(transform.TransformPoint(point));
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
            visualTile.transform.DOMove(handPositions[i], 0.25f).SetEase(Ease.OutCubic).SetDelay(delay);
            visualTile.transform.DOLocalRotate(new Vector3(90f, 0f, 0f), 0.25f).SetEase(Ease.OutCubic).SetDelay(delay);
        }
    }

    public void PlaceTile(Tile tile, Vector3 position, Quaternion rotation)
    {
        Hand.RemoveTile(tile);

        tile.TileVisual.transform.DOKill();
        tile.TileVisual.transform.DOMove(position, 0.375f).SetEase(Ease.InOutQuad);
        tile.TileVisual.transform.DORotate(rotation.eulerAngles, 0.375f).SetEase(Ease.InOutQuad);

        GameManager.Instance.Board.UpdateTileParent(tile);
    }
}
