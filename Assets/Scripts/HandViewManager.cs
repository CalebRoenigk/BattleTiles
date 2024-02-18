using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandViewManager : MonoBehaviour
{
    public static HandViewManager Instance;
    
    public float SphereSize = 0.1f;
    public float Height = 0.3f; // In Viewport size

    [Header("Reference")]
    [SerializeField] private List<PlayerHandVisual> _playerHandVisuals = new List<PlayerHandVisual>(); 
    
    [Header("Hand UI Settings")]
    public float HandTargetWidth = 8f;
    [Space(12)]
    [SerializeField] private float _bottomPadding = 0.05f;
    [SerializeField] private float _handViewHeight = 0.3f;
    [SerializeField] private float _handViewWidth = 0.7f;
    [SerializeField] private float _handDistanceFromCamera = 4f;

    [Header("Hand Runtime Settings")]
    [SerializeField] private Vector3 _handTop;
    [SerializeField] private Vector3 _handBottom;
    [SerializeField] private Vector3 _handMiddle;
    [SerializeField] private Vector3 _handLeft;
    [SerializeField] private Vector3 _handRight;
    [Space(12)]
    [SerializeField] private float _handScale;
    
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
    
    private void Update()
    {
        // Update the top, bottom, middle, left, and right points
        UpdatePointPositions();

        // Update the transform of the hand
        UpdateHandTransform();
    }

    private void UpdatePointPositions()
    {
        Vector3 handViewBottom = new Vector3(0.5f, _bottomPadding, _handDistanceFromCamera);
        _handBottom = CameraManager.Instance.MainCamera.ViewportToWorldPoint(handViewBottom);
        Vector3 handViewTop = handViewBottom + new Vector3(0f, _handViewHeight, 0f);
        _handTop = CameraManager.Instance.MainCamera.ViewportToWorldPoint(handViewTop);
        _handMiddle = Vector3.Lerp(_handTop, _handBottom, 0.5f);
        Vector3 handViewLeft = new Vector3(0.5f - (_handViewWidth/2f), _bottomPadding + (_handViewHeight / 2f), _handDistanceFromCamera);
        _handLeft = CameraManager.Instance.MainCamera.ViewportToWorldPoint(handViewLeft);
        Vector3 handViewRight = new Vector3(0.5f + (_handViewWidth/2f), _bottomPadding + (_handViewHeight / 2f), _handDistanceFromCamera);
        _handRight = CameraManager.Instance.MainCamera.ViewportToWorldPoint(handViewRight);
    }

    private void UpdateHandTransform()
    {
        float worldWidth = Vector3.Distance(_handLeft, _handRight);
        _handScale = worldWidth / HandTargetWidth;
        
        // Apply the transforms to the hand
        foreach (PlayerHandVisual playerHandVisual in _playerHandVisuals)
        {
            // Only modify the transforms of the current and surrounding player hands
            if (GameManager.Instance.GetPlayerTurnsReference().Contains(playerHandVisual.Hand.Owner.Index))
            {
                Transform playerHandTransform = playerHandVisual.transform;
                playerHandTransform.localScale = _handScale * Vector3.one;
                playerHandTransform.localPosition = new Vector3(0f, -Vector3.Distance(_handTop, _handBottom), 0f);
                playerHandTransform.rotation = CameraManager.Instance.MainCamera.transform.rotation;
            }
        }
    }

    public void AddHand(PlayerHandVisual playerHandVisual)
    {
        _playerHandVisuals.Add(playerHandVisual);
    }
}
