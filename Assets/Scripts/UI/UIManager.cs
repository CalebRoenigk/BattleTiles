using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [SerializeField] private Transform _playerUIParent;
    [SerializeField] private GameObject _playerUIPrefab;

    [SerializeField] private List<PlayerUI> _playerUIs = new List<PlayerUI>();

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
    
    public void AddPlayer(Player player)
    {
        PlayerUI playerUI = Instantiate(_playerUIPrefab, _playerUIParent).GetComponent<PlayerUI>();
        playerUI.SetPlayer(player);
        _playerUIs.Add(playerUI);
    }

    public void UpdatePlayerHealth()
    {
        // TODO: Refactor this so the tallies are the ones that apply the health changes!
        foreach (PlayerUI playerUI in _playerUIs)
        {
            playerUI.UpdatePlayerHealth();
        }
    }
}
