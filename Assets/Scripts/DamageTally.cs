using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DamageTally
{
    public int Damage;
    public int OwnerIndex;
    public int SourceValue;
    public List<int> PlayersEffected;
    public List<Interface> InterfaceSources = new List<Interface>();

    public DamageTally(int damage, int ownerIndex, List<int> playersEffected, List<Interface> interfaceSources)
    {
        Damage = damage;
        OwnerIndex = ownerIndex;
        PlayersEffected = playersEffected;
        InterfaceSources = interfaceSources;
        SourceValue = interfaceSources[0].Value;
    }

    // TODO: This is not gonna work quite right because when it condenses it doesnt allow for 2 tallies (1 for healing and 1 for damage), fix this later
    public static DamageTally CondenseIntoSingle(List<DamageTally> tallies)
    {
        int dmg = 0;
        List<Interface> sources = new List<Interface>();
        List<int> effected = new List<int>();

        foreach (DamageTally tally in tallies)
        {
            dmg += tally.Damage;
            sources.AddRange(tally.InterfaceSources);
            foreach (int effectedIndex in tally.PlayersEffected)
            {
                if (!effected.Contains(effectedIndex))
                {
                    effected.Add(effectedIndex);
                }
            }
        }
        
        DamageTally condensedTally = new DamageTally(dmg, GameManager.Instance.PlayerTurn, effected, sources.Distinct().ToList());

        return condensedTally;
    }
}
