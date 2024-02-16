using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interface
{
    public Tile Parent;
    public int Value;
    public Vector3 Center; // Local
    public TileSide Side;
    public Interface Connected;

    public Interface(Tile parent, int value, TileSide side)
    {
        Parent = parent;
        Value = value;
        Center = SideToCenter(side);
        Side = side;
        Connected = null;
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
        return Connected == null;
    }

    public void ConnectInterface(Interface interfaceToConnect)
    {
        Connected = interfaceToConnect;
        interfaceToConnect.Connected = this;
    }

    public Vector3 GetPosition(bool localPosition = false)
    {
        return localPosition ? Center : Parent.TileVisual.transform.TransformPoint(Center);
    }

    public Vector3 GetPlacementPosition()
    {
        return Parent.TileVisual.transform.TransformPoint(Center*3f);
    }
}
