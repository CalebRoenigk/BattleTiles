using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [Header("Camera")]
    public Camera MainCamera;
    
    [Header("Cinemachine")]
    [SerializeField] private CinemachineTargetGroup _boardTargetGroup;
    
    [Header("Hand Settings")]
    [SerializeField] private float _distFromCamera = 4f;
    [SerializeField] private float _bottomPad = 0.05f;
    [SerializeField] private float _objectViewSize = 0.3f;
    [SerializeField] private float _objectViewLength = 0.6f;

    [Header("Runtime")]
    [SerializeField] private float _handWidth;
    [SerializeField] private float _handScaleFactor;

    [Header("Gizmos")]
    [SerializeField] private bool _drawGizmos = true;

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

    private void Start()
    {
        MainCamera = Camera.main;
        CalculateHandSettings();
    }

    private void OnDrawGizmos()
    {
        if (_drawGizmos)
        {
            if (MainCamera != null)
            {
                Vector3 bottomObjectViewPoint = new Vector3(0.5f, 0f + _bottomPad, _distFromCamera);
                Vector3 topObjectViewPoint = new Vector3(0.5f, 0f + _bottomPad + _objectViewSize, _distFromCamera);
                Vector3 bottomObjectWorldPoint = MainCamera.ViewportToWorldPoint(bottomObjectViewPoint);
                Vector3 topObjectWorldPoint = MainCamera.ViewportToWorldPoint(topObjectViewPoint);
                Vector3 worldPos = Vector3.Lerp(bottomObjectWorldPoint, topObjectWorldPoint, 0.5f);
                Vector3 middleObjectViewPoint = MainCamera.WorldToViewportPoint(worldPos);
        
                Vector3 leftObjectLengthViewPoint = new Vector3(0.5f - (_objectViewLength / 2f), middleObjectViewPoint.y, _distFromCamera);
                Vector3 rightObjectLengthViewPoint = new Vector3(0.5f + (_objectViewLength / 2f), middleObjectViewPoint.y, _distFromCamera);
                Vector3 leftObjectLengthWorldPoint = MainCamera.ViewportToWorldPoint(leftObjectLengthViewPoint);
                Vector3 rightObjectLengthWorldPoint = MainCamera.ViewportToWorldPoint(rightObjectLengthViewPoint);

                List<Vector3> pointsToDraw = new List<Vector3>();
                pointsToDraw.Add(worldPos);
                pointsToDraw.Add(bottomObjectWorldPoint);
                pointsToDraw.Add(topObjectWorldPoint);
                pointsToDraw.Add(leftObjectLengthWorldPoint);
                pointsToDraw.Add(rightObjectLengthWorldPoint);
            
                Gizmos.color = Color.cyan;
                foreach (Vector3 pt in pointsToDraw)
                {
                    Gizmos.DrawSphere(pt, 0.01f);
                }
                
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 4f;
                Vector3 mouse = MainCamera.ScreenToWorldPoint(mousePos);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(mouse, 0.01f);
            }
        }
    }

    public void AddTileToBoardTargets(Tile tile)
    {
        _boardTargetGroup.AddMember(tile.TileVisual.transform, 1f, 1f);
    }
    
    // Returns the width of a player's visual hand
    public void GetHandSettings(out float handWidth, out float handScale)
    {
        CalculateHandSettings();
        handWidth = _handWidth;
        handScale = _handScaleFactor;
    }
    
    // Calculates the width of a player's hand
    public void CalculateHandSettings()
    {
        Vector3 bottomObjectViewPoint = new Vector3(0.5f, 0f + _bottomPad, _distFromCamera);
        Vector3 topObjectViewPoint = new Vector3(0.5f, 0f + _bottomPad + _objectViewSize, _distFromCamera);
        Vector3 bottomObjectWorldPoint = MainCamera.ViewportToWorldPoint(bottomObjectViewPoint);
        Vector3 topObjectWorldPoint = MainCamera.ViewportToWorldPoint(topObjectViewPoint);
        Vector3 worldPos = Vector3.Lerp(bottomObjectWorldPoint, topObjectWorldPoint, 0.5f);
        Vector3 middleObjectViewPoint = MainCamera.WorldToViewportPoint(worldPos);
        
        Vector3 leftObjectLengthViewPoint = new Vector3(0.5f - (_objectViewLength / 2f), middleObjectViewPoint.y, _distFromCamera);
        Vector3 rightObjectLengthViewPoint = new Vector3(0.5f + (_objectViewLength / 2f), middleObjectViewPoint.y, _distFromCamera);
        Vector3 leftObjectLengthWorldPoint = MainCamera.ViewportToWorldPoint(leftObjectLengthViewPoint);
        Vector3 rightObjectLengthWorldPoint = MainCamera.ViewportToWorldPoint(rightObjectLengthViewPoint);
        
        _handWidth = Vector3.Distance(leftObjectLengthWorldPoint, rightObjectLengthWorldPoint);
        _handScaleFactor = Vector3.Distance(bottomObjectWorldPoint, topObjectWorldPoint);
    }

    public void GetHandTransforms(out Vector3 position, out Quaternion rotation)
    {
        Vector3 bottomObjectViewPoint = new Vector3(0.5f, 0f + _bottomPad, _distFromCamera);
        Vector3 topObjectViewPoint = new Vector3(0.5f, 0f + _bottomPad + _objectViewSize, _distFromCamera);
        Vector3 bottomObjectWorldPoint = MainCamera.ViewportToWorldPoint(bottomObjectViewPoint);
        Vector3 topObjectWorldPoint = MainCamera.ViewportToWorldPoint(topObjectViewPoint);
        position = Vector3.Lerp(bottomObjectWorldPoint, topObjectWorldPoint, 0.5f);
        position.x = 0f;
        
        rotation = Quaternion.LookRotation(MainCamera.transform.forward, MainCamera.transform.up);
    }
}
