using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public Hand Hand;
    public int Health;
    public int Score;

    public Player(int health = 100)
    {
        Hand = new Hand();
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
}
