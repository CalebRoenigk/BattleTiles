using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BoardVisual : MonoBehaviour
{
    public static BoardVisual Instance;

    public Board Board;

    [SerializeField] private Transform _boardParent;
    [SerializeField] private Transform _placedParent;
    [SerializeField] private Transform _ghostParent;
    [SerializeField] private Transform _effectsParent;
    [SerializeField] private GameObject _ghostPrefab;
    [SerializeField] private GameObject _tileGlowEffect;
    [SerializeField] private List<TileGhostVisual> _ghostVisuals = new List<TileGhostVisual>();
    [SerializeField] private float _rotateBoard = 0f;
    [SerializeField] private float _rotationSpeedRamp = 0.05f;
    [SerializeField] private float _rotationSpeedMax = 10f;
    [SerializeField] private float _rotationSpeedSlow = 0.075f;
    
    
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
        bool rotationPressed = false;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            _rotateBoard += _rotationSpeedRamp * Time.deltaTime;
            rotationPressed = true;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            _rotateBoard -= _rotationSpeedRamp * Time.deltaTime;
            rotationPressed = true;
        }
        
        _rotateBoard = Mathf.Clamp(_rotateBoard, -_rotationSpeedMax, _rotationSpeedMax);
        
        if (!rotationPressed)
        {
            _rotateBoard = Mathf.Lerp(_rotateBoard, 0f, _rotationSpeedSlow);
            if (Mathf.Abs(_rotateBoard) <= 0.01f)
            {
                _rotateBoard = 0f;
            }
        }
        
        Vector3 euler = _boardParent.localRotation.eulerAngles;
        euler.y += _rotateBoard;
        _boardParent.localRotation = Quaternion.Euler(euler);
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
        // Spawn ghosts at their matched interfaces
        foreach (Interface matchedInterface in matchedInterfaces)
        {
            Vector3 placementPosition = matchedInterface.GetPlacementPosition();
            TileGhostVisual tileGhostVisual = Instantiate(_ghostPrefab, placementPosition, Quaternion.identity, _ghostParent).GetComponent<TileGhostVisual>();
            tileGhostVisual.SetTile(tile, Board.PrimaryMatch == matchedInterface);
            
            // Align the ghosts properly
            Quaternion alignedRotation = Quaternion.LookRotation(matchedInterface.GetPosition() - placementPosition, Vector3.up);
            tileGhostVisual.transform.rotation = alignedRotation;
            tileGhostVisual.transform.Rotate(new Vector3(0f, 90f, 0f), Space.Self);

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
