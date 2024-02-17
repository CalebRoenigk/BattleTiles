using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TestCamPlacement : MonoBehaviour
{
    [SerializeField] private Transform _testObject;
    [SerializeField] private Camera _mainCam;
    [SerializeField] private float _distFromCamera = 2f;
    [SerializeField] private float _bottomPad = 0.05f;
    [SerializeField] private float _objectViewSize = 0.3f;
    [SerializeField] private float _objectViewLength = 0.6f;
    
    [SerializeField] private float _objectPlacementLength;

    private void Start()
    {
        _mainCam = Camera.main;
    }

    void Update()
    {
        // Vector3 worldPos = _mainCam.ViewportToWorldPoint(new Vector3(0.5f, 0f, _distFromCamera));
        
        
        // Calculate the scale of a 1f object at a distance from the camera
        // Vector3 bottomObjectViewPoint = new Vector3(0.5f, 0f + _bottomPad, _distFromCamera);
        // Vector3 bottomObjectWorldPoint = _mainCam.ViewportToWorldPoint(bottomObjectViewPoint);
        // Vector3 topObjectWorldPoint = bottomObjectWorldPoint + _mainCam.transform.up;
        // Vector3 topObjectViewPoint = _mainCam.WorldToViewportPoint(topObjectWorldPoint);

        // float objectScaleFactor = _objectViewSize / Vector3.Distance(bottomObjectViewPoint, topObjectViewPoint);

        // _testObject.position = worldPos;
        // _testObject.rotation = lookRotation;
        // _testObject.localScale = Vector3.one * objectScaleFactor;
        
        // Another way to do this would be get the top and bottom point in the viewport
        // Convert them to world space coords
        // Get the center of the world space coords
        // Get the size of the world space coords
        
        Vector3 bottomObjectViewPoint = new Vector3(0.5f, 0f + _bottomPad, _distFromCamera);
        Vector3 topObjectViewPoint = new Vector3(0.5f, 0f + _bottomPad + _objectViewSize, _distFromCamera);
        Vector3 bottomObjectWorldPoint = _mainCam.ViewportToWorldPoint(bottomObjectViewPoint);
        Vector3 topObjectWorldPoint = _mainCam.ViewportToWorldPoint(topObjectViewPoint);

        float objectScaleFactor = Vector3.Distance(bottomObjectWorldPoint, topObjectWorldPoint);
        Quaternion lookRotation = Quaternion.LookRotation(_mainCam.transform.forward, _mainCam.transform.up);
        Vector3 worldPos = Vector3.Lerp(bottomObjectWorldPoint, topObjectWorldPoint, 0.5f);
        
        _testObject.position = worldPos;
        _testObject.rotation = lookRotation;
        _testObject.localScale = Vector3.one * objectScaleFactor;
    }

    private void OnDrawGizmos()
    {
        // Calculate the scale of a 1f object at a distance from the camera
        Vector3 bottomObjectViewPoint = new Vector3(0.5f, 0f + _bottomPad, _distFromCamera);
        Vector3 topObjectViewPoint = new Vector3(0.5f, 0f + _bottomPad + _objectViewSize, _distFromCamera);
        Vector3 bottomObjectWorldPoint = _mainCam.ViewportToWorldPoint(bottomObjectViewPoint);
        Vector3 topObjectWorldPoint = _mainCam.ViewportToWorldPoint(topObjectViewPoint);
        Vector3 worldPos = Vector3.Lerp(bottomObjectWorldPoint, topObjectWorldPoint, 0.5f);
        Vector3 middleObjectViewPoint = _mainCam.WorldToViewportPoint(worldPos);

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(bottomObjectWorldPoint, 0.1f);
        Gizmos.DrawSphere(topObjectWorldPoint, 0.1f);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(worldPos, 0.1f);
        
        // Draw the left and right edges of the view length
        Vector3 leftObjectLengthViewPoint = new Vector3(0.5f - (_objectViewLength / 2f), middleObjectViewPoint.y, _distFromCamera);
        Vector3 rightObjectLengthViewPoint = new Vector3(0.5f + (_objectViewLength / 2f), middleObjectViewPoint.y, _distFromCamera);
        Vector3 leftObjectLengthWorldPoint = _mainCam.ViewportToWorldPoint(leftObjectLengthViewPoint);
        Vector3 rightObjectLengthWorldPoint = _mainCam.ViewportToWorldPoint(rightObjectLengthViewPoint);
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(leftObjectLengthWorldPoint, 0.1f);
        Gizmos.DrawSphere(rightObjectLengthWorldPoint, 0.1f);

        _objectPlacementLength = Vector3.Distance(leftObjectLengthWorldPoint, rightObjectLengthWorldPoint);
    }
    
    // My thoughts are:
    // When a turn starts the player hand is enabled and placed in the proper location from the cam
    // The player hand will need to be told how wide it can be by the camera manager (could do this every turn, but maybe better performance to do it once at the start?)
    // When the turn is finished, animate the player hand off
    // After animating off, trigger pre-turn checks on game manager
    // At the end of preturn checks, trigger turn start which will animate the player hand on
}
