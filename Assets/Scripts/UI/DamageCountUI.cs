using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamageCountUI : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private RectTransform _transform;

    public void AnimateDamage(int damage, Vector3 destination, DamageTallyUI tallyParent, float delay = 0f, bool isLast = false)
    {
        _countText.text = damage.ToString();
        Sequence animateSequence = DOTween.Sequence();
        animateSequence.Append(_transform.DOAnchorPosX(destination.x, 0.75f).SetEase(Ease.InCubic).SetDelay(delay));
        animateSequence.Insert(0f, _transform.DOAnchorPosY(destination.x, 0.75f).SetEase(Ease.InExpo).SetDelay(delay));
        animateSequence.AppendCallback(() => tallyParent.AddCount(damage, isLast));
        animateSequence.AppendCallback(() => Destroy(gameObject, 0f));
    }
}
