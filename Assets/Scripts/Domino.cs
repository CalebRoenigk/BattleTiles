using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class Domino : MonoBehaviour
{
    public ValuePair Values;
    public PlayerHand ParentHand = null;

    public List<DominoInterface> Interfaces = new List<DominoInterface>();

    public bool MouseOn;
    public bool CanInteract;

    [SerializeField] private Material _material;
    [SerializeField] private Vector3 _topCenter;
    [SerializeField] private Vector3 _bottomCenter;
    [SerializeField] private Vector3 _leftCenter;
    [SerializeField] private Vector3 _rightCenter;

    [SerializeField] private Vector3 _topPlacement;
    [SerializeField] private Vector3 _bottomPlacement;
    [SerializeField] private Vector3 _leftPlacement;
    [SerializeField] private Vector3 _rightPlacement;
    
    [Header("Gizmos")]
    public bool ToggleGizmos = false;

    private void Awake()
    {
        _material = this.GetComponent<MeshRenderer>().material;
    }

    private void OnDrawGizmos()
    {
        if (ToggleGizmos)
        {
            UpdatePoints();
            Gizmos.color = Interfaces.Find(i => i.Side == DominoSide.Top).Open ? Color.green : Color.red;
            Gizmos.DrawSphere(_topPlacement, 0.25f);
            Gizmos.color = Interfaces.Find(i => i.Side == DominoSide.Bottom).Open ? Color.green : Color.red;
            Gizmos.DrawSphere(_bottomPlacement, 0.25f);
            if (Values.IsDouble)
            {
                Gizmos.color = Interfaces.Find(i => i.Side == DominoSide.Left).Open ? Color.green : Color.red;
                Gizmos.DrawSphere(_leftPlacement, 0.25f);
                Gizmos.color = Interfaces.Find(i => i.Side == DominoSide.Right).Open ? Color.green : Color.red;
                Gizmos.DrawSphere(_rightPlacement, 0.25f);
            }
        }
        
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, ParentHand != null ? ParentHand.transform.position : Vector3.zero);
    }

    private void Update()
    {
        if (MouseOn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameManager.Instance.RequestPlaceDomino(this);
            }
        }
    }

    void OnMouseEnter()
    {
        if (CanInteract)
        {
            MouseOn = true;
            transform.DOKill(false);
            transform.DOLocalMoveY(1f, 0.175f).SetEase(Ease.OutCubic);
        }
    }

    void OnMouseExit()
    {
        if (CanInteract)
        {
            MouseOn = false;
            transform.DOKill(false);
            transform.DOLocalMoveY(0f, 0.175f).SetEase(Ease.OutCubic);
        }
    }

    public void ResetMouseState()
    {
        transform.DOKill(false);
        transform.DOLocalMoveY(0f, 0.175f).SetEase(Ease.OutCubic);
        MouseOn = false;
    }

    public void SetDomino(ValuePair pair)
    {
        this.Values = pair;

        _material.SetInt("_Top_Value", Values.ValueA);
        _material.SetInt("_Bottom_Value", Values.ValueB);
        
        this.Interfaces.Add(new DominoInterface(this, DominoSide.Top, Values.ValueA));
        this.Interfaces.Add(new DominoInterface(this, DominoSide.Bottom, Values.ValueB));
        if (Values.IsDouble)
        {
            this.Interfaces.Add(new DominoInterface(this, DominoSide.Left, Values.ValueA));
            this.Interfaces.Add(new DominoInterface(this, DominoSide.Right, Values.ValueB));
        }
        
        this.gameObject.name = Values.ToString();
    }

    public List<Vector3> GetPossibleConnections(int value)
    {
        List<Vector3> positions = new List<Vector3>();
        if (value != Values.ValueA && value != Values.ValueB)
        {
            return positions;
        }
        
        List<DominoInterface> openInterfaces = Interfaces.Where(i => i.Open).ToList();
        
        foreach (var openInterface in openInterfaces)
        {
            switch (openInterface.Side)
            {
                case DominoSide.Top:
                default:
                    positions.Add(_topPlacement);
                    break;
                case DominoSide.Left:
                    positions.Add(_leftPlacement);
                    break;
                case DominoSide.Bottom:
                    positions.Add(_bottomPlacement);
                    break;
                case DominoSide.Right:
                    positions.Add(_rightPlacement);
                    break;
            }
        }

        return positions;
    }

    public List<DominoInterface> GetAllOpenInterfaces()
    {
        return Interfaces.Where(i => i.Open).ToList();
    }

    public bool HasOpenConnections()
    {
        return Interfaces.Where(i => i.Open).ToList().Count > 0;
    }

    public void ConnectSide(DominoSide side)
    {
        Interfaces.Find(i => i.Side == side).SetOpen(false);
    }

    public void UpdatePoints()
    {
        _topPlacement = transform.TransformPoint(_topCenter - new Vector3(0f, 0f, 2f) + new Vector3(0f, 0f, -2.35294f/2f-0.1f));
        _bottomPlacement = transform.TransformPoint(_bottomCenter + new Vector3(0f, 0f, 2f) + new Vector3(0f, 0f, 2.35294f/2f+0.1f));
        _leftPlacement = transform.TransformPoint(_leftCenter - new Vector3(2.35294f/2f-0.1f, 0f, 0f));
        _rightPlacement = transform.TransformPoint(_rightCenter + new Vector3(2.35294f/2f-0.1f, 0f, 0f));
    }

    public void RemoveFromPlayerHand()
    {
        ParentHand.RemoveDomino(this);
    }

    public Vector3 GetInterfacePosition(DominoSide side)
    {
        UpdatePoints();
        
        switch (side)
        {
            case DominoSide.Top:
            default:
                return _topPlacement;
                break;
            case DominoSide.Left:
                return _leftPlacement;
                break;
            case DominoSide.Bottom:
                return _bottomPlacement;
                break;
            case DominoSide.Right:
                return _rightPlacement;
                break;
        }
    }

    public DominoInterface GetMatchingInterface(DominoInterface dominoInterface)
    {
        return Interfaces.Where(i => i.Open && i.Value == dominoInterface.Value).ToList()[0];
    }
}
