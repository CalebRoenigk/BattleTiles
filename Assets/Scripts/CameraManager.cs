using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] private Transform _cameraGroup;
    [SerializeField] private CinemachineTargetGroup _boardTargetGroup;
    
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

    public void NextTurn()
    {
        _cameraGroup.DORotate(new Vector3(0, -((360f / (float)GameManager.Instance.PlayerCount) * ((float)GameManager.Instance.PlayerTurn) + 90f) , 0f), 0.625f).SetEase(Ease.InOutCubic);
    }

    public void AddTileToBoardTargets(Tile tile)
    {
        _boardTargetGroup.AddMember(tile.TileVisual.transform, 1f, 1f);
    }
}
