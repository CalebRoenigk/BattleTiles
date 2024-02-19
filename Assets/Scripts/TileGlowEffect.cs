using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TileGlowEffect : MonoBehaviour
{
    [SerializeField] private Material _material;

    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
    }

    public void DoGlowAnimation(TileSide side, Color baseColor, Color fadeColor)
    {
        gameObject.name = "Effect " + side.ToString();
        
        _material.SetFloat("_X_Fade", 0f);
        _material.SetFloat("_Y_Fade", 1f);
        _material.SetColor("_Bottom_Color", baseColor);
        _material.SetColor("_Top_Color", fadeColor);
        
        Sequence glowSequence = DOTween.Sequence();
        glowSequence.Append(_material.DOFloat(0f, "_Y_Fade",  0.1f).SetEase(Ease.OutSine));
        glowSequence.Insert(0.1f, _material.DOFloat(1f, "_X_Fade",  0.65f).SetEase(Ease.InQuad));
        glowSequence.Insert(0.15f, _material.DOFloat(1f, "_Y_Fade",  0.875f).SetEase(Ease.InSine));
        // glowSequence.AppendCallback(() => Destroy(gameObject, 0f));
    }
}
