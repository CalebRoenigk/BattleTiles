using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardVisual : MonoBehaviour
{
    public static BoardVisual Instance;
    
    [SerializeField] private Transform _boardParent;
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
    
    public void AddTile(Tile tile)
    {
        tile.TileVisual.transform.parent = _boardParent;
    }
}
