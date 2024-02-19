using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerHealthBarUI : MonoBehaviour
{
    [SerializeField] private Transform _barParent;
    [SerializeField] private GameObject _segementPrefab;
    [SerializeField] private List<HealthBarSegment> _segments;
    [SerializeField] private int _healthSegments;
    [SerializeField] private PlayerUI _playerUI;
    [SerializeField] private int _currentHealth;
    [SerializeField] private int _segmentHealthValue = 10;
    [SerializeField] private float _segmentDelay = 0.075f;
    [SerializeField] private float _segmentChangeSpeed = 0.05f;

    private void GenerateHealthSegments()
    {
        _healthSegments = GameManager.Instance.StartingHealth / _segmentHealthValue;
        for (int i = 0; i < _healthSegments; i++)
        {
            HealthBarSegment barSegment = Instantiate(_segementPrefab, _barParent).GetComponent<HealthBarSegment>();
            _segments.Add(barSegment);
        }
    }

    public void SetPlayer(PlayerUI playerUI)
    {
        _playerUI = playerUI;
        _currentHealth = _playerUI.Player.Health;
        GenerateHealthSegments();
    }

    public void UpdateHealth()
    {
        if (_playerUI.Player.Health != _currentHealth)
        {
            // Going Up
            for (int i = 0; i < _segments.Count; i++)
            {
                int segmentHealth = i * _segmentHealthValue;
                float segmentVal = segmentHealth > _playerUI.Player.Health ? 0f : 2f;

                int delayIndex = _playerUI.Player.Health < _currentHealth ? (_segments.Count - 1) - i : i;
                
                _segments[i].SetState(segmentVal, delayIndex * 0.05f);
            }
        }

        _currentHealth = _playerUI.Player.Health;
    }

    private int HealthToSegmentValue(int health)
    {
        return Mathf.FloorToInt((float)(health) / (float)_segmentHealthValue);
    }
    
}
