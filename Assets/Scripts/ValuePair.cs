using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ValuePair
{
    public int ValueA;
    public int ValueB;

    public bool IsDouble;
    
    public ValuePair(int a, int b)
    {
        this.ValueA = a;
        this.ValueB = b;
        this.IsDouble = a == b;
    }

    public bool IsEqual(ValuePair compare)
    {
        if (compare.ValueA == ValueA && compare.ValueB == ValueB)
        {
            return true;
        }
        if (compare.ValueB == ValueA && compare.ValueA == ValueB)
        {
            return true;
        }

        return false;
    }

    public bool IsGreater(ValuePair compare)
    {
        return TotalValue() > compare.TotalValue();
    }

    public int TotalValue()
    {
        return ValueA + ValueB;
    }

    public override string ToString()
    {
        return "[" + ValueA + ":" + ValueB + "]";
    }

    public bool HasMatchingValue(List<int> values)
    {
        return values.Contains(ValueA) || values.Contains(ValueB);
    }
}
