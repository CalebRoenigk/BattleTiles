using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUIHandler : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Transform _damageCountParent;
    [SerializeField] private Transform _damageTallyParent;
    [SerializeField] private GameObject _damageCountPrefab;
    [SerializeField] private GameObject _damageTallyPrefab;

    [Header("Settings")]
    [SerializeField] private Vector3 _damageTallyStartPoint = new Vector3(0.5f, 0.5f, 0f); // Viewport Coordinates
    

    public void DisplayDamageTally(DamageTally damageTally)
    {
        // Spawn the "hidden" tally
        Vector3 tallyPosition = new Vector3(0f, 0f, 0f);
        DamageTallyUI tallyUIMain = Instantiate(_damageTallyPrefab, Vector3.zero, Quaternion.identity, _damageTallyParent).GetComponent<DamageTallyUI>();
        tallyUIMain.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        Vector2 sourceScreenDestination = Vector2.zero;
        // First spawn all the damage counters
        int delayIndex = 0;
        float delayCountOffset = 0.075f;
        float delayCountBase = 0.5f;
        int sourcesCount = damageTally.InterfaceSources.Count;
        foreach (Interface sourceInterface in damageTally.InterfaceSources)
        {
            Vector3 sourceScreenPosition = CameraManager.Instance.MainCamera.WorldToScreenPoint(sourceInterface.GetPosition());
            
            // Force them towards the damage center point
            Instantiate(_damageCountPrefab, sourceScreenPosition, Quaternion.identity, _damageCountParent).GetComponent<DamageCountUI>().AnimateDamage(-GameManager.Instance.DamageCardValue, sourceScreenDestination, tallyUIMain, (float)delayIndex * delayCountOffset + delayCountBase, delayIndex == sourcesCount-1);
            delayIndex++;
        }
    }
}
