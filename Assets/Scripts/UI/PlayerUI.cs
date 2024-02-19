using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public Player Player;

    [SerializeField] private PlayerHealthBarUI _playerHealthBarUI;
    [SerializeField] private TextMeshProUGUI _playerName;

    public void SetPlayer(Player player)
    {
        Player = player;
        _playerName.text = "Player " + Player.Index;
        gameObject.name = "Player " + Player.Index + " UI";
        _playerHealthBarUI.SetPlayer(this);
    }

    public void UpdatePlayerHealth()
    {
        _playerHealthBarUI.UpdateHealth();
    }
}
