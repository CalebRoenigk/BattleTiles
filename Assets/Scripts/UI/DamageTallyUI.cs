using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamageTallyUI : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private TextMeshProUGUI _tallyText;
    [SerializeField] private RectTransform _transform;
    
    [Header("Runtime")]
    [SerializeField] private int _tallyCount = 0;

    private void Update()
    {
        if (_tallyCount < 0)
        {
            _tallyText.text = _tallyCount.ToString();
        }
    }

    public void AddCount(int addValue, bool isLast = false)
    {
        _tallyCount += addValue;
        ((Transform)_transform).DOScale(Vector3.one * GetScaleFactor(Mathf.Abs(_tallyCount)), 0.4f).SetEase(Ease.OutElastic);
        if (isLast)
        {
            // TODO: Do the second part of the animation
        }
    }

    private float GetScaleFactor(int value)
    {
        return Mathf.Log(((float)value + 1f), 5.5f) + 0f;
    }
}
