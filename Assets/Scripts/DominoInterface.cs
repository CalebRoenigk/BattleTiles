using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DominoInterface
{
    public Domino Parent;
    public DominoSide Side;
    public bool Open = true;
    public int Value;

    public DominoInterface(Domino parent, DominoSide side, int value)
    {
        this.Parent = parent;
        this.Side = side;
        this.Value = value;
    }

    public void SetOpen(bool state)
    {
        Open = state;
    }
}
