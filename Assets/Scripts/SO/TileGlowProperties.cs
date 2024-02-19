using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileGlowProperties
{
    public int Value;
    [ColorUsageAttribute(true, true)]public Color BaseColor;
    [ColorUsageAttribute(true, true)]public Color FadeColor;
}
