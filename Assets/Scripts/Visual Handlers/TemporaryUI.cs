using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TemporaryUI : MonoBehaviour
{
    public static TemporaryUI Instance;
    
    [Header("Reference")]
    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private TextMeshProUGUI _playerHealth;
    [SerializeField] private TextMeshProUGUI _playerScore;
    
    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    public void SetPlayerStats(string name, int health, int score)
    {
        _playerName.text = name;
        _playerHealth.text = health.ToString();
        _playerScore.text = score.ToString();
    }
}
