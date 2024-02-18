using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTally
{
    public int Damage;
    public int OwnerIndex;
    public List<int> PlayersEffected;
    public List<Interface> InterfaceSources = new List<Interface>();

    public DamageTally(int damage, int ownerIndex, List<int> playersEffected, List<Interface> interfaceSources)
    {
        Damage = damage;
        OwnerIndex = ownerIndex;
        PlayersEffected = playersEffected;
        InterfaceSources = interfaceSources;
    }
}
