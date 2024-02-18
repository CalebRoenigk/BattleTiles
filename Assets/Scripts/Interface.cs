using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Interface
{
    public Tile Parent;
    public int Value;
    public Vector3 Center; // Local
    public TileSide Side;
    public Interface Connected;
    public bool Open;

    public Interface(Tile parent, int value, TileSide side)
    {
        Parent = parent;
        Value = value;
        Center = SideToCenter(side);
        Side = side;
        Connected = null;
        Open = true;
    }

    private static Vector3 SideToCenter(TileSide side)
    {
        switch (side)
        {
            case TileSide.Top:
            default:
                return new Vector3(0.5f, 0f, 0f);
            case TileSide.Right:
                return new Vector3(0f, 0f, 0.5f);
            case TileSide.Bottom:
                return new Vector3(-0.5f, 0f, 0f);
            case TileSide.Left:
                return new Vector3(0f, 0f, -0.5f);
        }
    }

    public Tile GetParent()
    {
        return Parent;
    }

    public bool IsOpen()
    {
        return Open;
    }

    // Connects this interface to the passed interface
    public void ConnectInterface(Interface interfaceToConnect)
    {
        Connected = interfaceToConnect;
        if (interfaceToConnect.Connected == null)
        {
            interfaceToConnect.ConnectInterface(this);
        }
        SetOpenState(false);
    }

    public Vector3 GetPosition(bool localPosition = false)
    {
        return localPosition ? Center : Parent.TileVisual.transform.TransformPoint(Center);
    }

    public Vector3 GetPlacementPosition()
    {
        return Parent.TileVisual.transform.TransformPoint(Center*2f);
    }

    public void SetOpenState(bool state)
    {
        Open = state;
    }
    
    // Returns a quaternion that points towards the passed interface given an origin location and the
    public Quaternion GetOrientationTowards(Vector3 origin, Interface targetInterface)
    {
        // Vector3 originLocalized = targetInterface.Parent.TileVisual.transform.InverseTransformPoint(origin);
        // Vector3 alignmentDirection = targetInterface.Center - originLocalized;
        //
        // Quaternion baseQuaternion = quaternion.LookRotation(alignmentDirection.normalized, Vector3.up);
        // Quaternion sideRotationOffset = GetSideQuaternion();
        // baseQuaternion *= sideRotationOffset;
        // return baseQuaternion;
        
        // Given this base quaternion we know that:
        // First Tile Above, Placed Tile Below
        // Example: Left to Right = 0
        // Example: Top to Bottom = 0
        // Example: Bottom to Top = 0
        // Example: Right to Left = 0
        // Example: Left to Top = -90
        // Example: Left to Left = -180
        // Example: Left to Bottom = 90
        
        // First Tile Right, Placed Tile Left
        // Example: Bottom to Top = 0
        // Example: Bottom to Bottom = -180
        // Example: Bottom to Left = -90
        // Example: Bottom to Right = 90;
        Vector3 targetSideDirection = targetInterface.Center.normalized;
        Vector3 placementSideDirection = Center.normalized;
        Quaternion sidesAngularDifferential = Quaternion.FromToRotation(placementSideDirection, targetSideDirection);
        float yDifferential = sidesAngularDifferential.eulerAngles.y;

        Quaternion baseRotation = targetInterface.Parent.TileVisual.transform.rotation;
        Quaternion alignedRotation = baseRotation * Quaternion.Euler(Vector3.up * yDifferential);
        alignedRotation *= Quaternion.Euler(Vector3.up * 180f);

        return alignedRotation;
    }
}
