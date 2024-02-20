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

    [Header("Mouse")]
    public Vector3 MouseWorldPosition;
    
    [Header("Cam Rotate")]
    [SerializeField] private float _rotateBoard = 0f;
    [SerializeField] private float _rotationSpeedRamp = 0.05f;
    [SerializeField] private float _rotationSpeedMax = 10f;
    [SerializeField] private float _rotationSpeedSlow = 0.075f;
    [SerializeField] private Vector3 _cameraRotationPosition = new Vector3(0f, 15f, -20f);
    [SerializeField] private float _yRotation = 0f;
    
    [Header("Cinemachine")]
    [SerializeField] private CinemachineTargetGroup _boardTargetGroup;
    [SerializeField] private CinemachineVirtualCamera _boardCamera;
    [SerializeField] private CinemachineTransposer _boardCameraTransposer;
    
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
        _boardCameraTransposer = _boardCamera.GetCinemachineComponent<CinemachineTransposer>();
    }

    private void Update()
    {
        MouseWorldPosition = GetMouseWorldPosition();
        RotateCamera();
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
    
    // Returns the mouse world position
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 4f;
        return MainCamera.ScreenToWorldPoint(mousePos);
    }

    private void RotateCamera()
    {
        bool rotationPressed = false;
        _rotationSpeedMax = GetRotationSpeedMax();
        
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

        _yRotation += _rotateBoard;
        _yRotation += 360;
        _yRotation %= 360;

        Vector3 newOffset = RotatePointAroundY(_cameraRotationPosition, _yRotation);

        _boardCameraTransposer.m_FollowOffset = newOffset;
    }

    private Vector3 RotatePointAroundY(Vector3 point, float angle)
    {
        float rads = Mathf.Deg2Rad * angle;
        float cos = Mathf.Cos(rads);
        float sin = Mathf.Sin(rads);

        float nx = (cos * (point.x - 0f)) + (sin * (point.z - 0f)) + 0f;
        float nz = (cos * (point.z - 0f)) - (sin * (point.x - 0f)) + 0f;

        return new Vector3(nx, point.y, nz);
    }

    private float GetRotationSpeedMax()
    {
        return 0.02f * MainCamera.fieldOfView + 0.42f;
    }
}
