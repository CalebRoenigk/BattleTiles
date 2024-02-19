using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarSegment : MonoBehaviour
{
    [SerializeField] private Material _material;

    private void Awake()
    {
        Material instanceMaterial = new Material(_material = GetComponent<Image>().material);
        _material = instanceMaterial;
        GetComponent<Image>().material = _material;
    }

    private void Start()
    {
        SetState(2f);
    }

    public void SetState(float state, float delay = 0f)
    {

        _material.DOFloat(state, "_State", 0.8f).SetEase(Ease.InOutSine).SetDelay(delay);
    }
}
