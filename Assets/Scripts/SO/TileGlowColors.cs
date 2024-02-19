using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileGlowColors", menuName = "ScriptableObjects/Tile/Glow Colors", order = 1)]
public class TileGlowColors : ScriptableObject
{
    [SerializeField]private List<TileGlowProperties> _tileGlowProperties = new List<TileGlowProperties>();

    public TileGlowProperties GetGlowColors(int value)
    {
        return _tileGlowProperties.Find(p => p.Value == value);
    }
}
