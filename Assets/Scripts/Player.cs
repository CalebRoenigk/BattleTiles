using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int Index;
    public Hand Hand;
    public int Health;
    public int Score;

    public Player(int index, int health = 100)
    {
        Index = index;
        Hand = new Hand(this);
        Health = health;
        Score = 0;
    }

    public Tile GetHighestInHand()
    {
        return Hand.GetHighestInHand();
    }
    
    public void AddToHand(Tile tile, float delay = 0f)
    {
        Hand.AddTile(tile);
    }

    public void StartTurn()
    {
        // Trigger turn start for the player hand visual
        Hand.HandVisual.ShowHand();
    }

    public void EndTurn()
    {
        // Trigger turn end for the player hand visual
        Hand.HandVisual.HideHand();
    }

    public void DoDamage(int damage)
    {
        Health -= damage;
        Health = Mathf.RoundToInt(Mathf.Max(Health, 0)); // Clamp the health to not fall below zero
    }
}
