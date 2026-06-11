using System;
using System.Collections.Generic;
using NUnit.Framework;

[Serializable]
public class DefendData {
    public int recoveredHp;
    int coin;

    public List<string> usedPerks = new List<string>();
    public List<string> usedItemCodes = new List<string>();
    public string usedElemental;
    public int counterDamage;
    
    public override string ToString()
    {
        string perksStr = usedPerks != null && usedPerks.Count > 0 ? string.Join(", ", usedPerks) : "None";
        string itemsStr = usedItemCodes != null && usedItemCodes.Count > 0 ? string.Join(", ", usedItemCodes) : "None";
        string elementalStr = !string.IsNullOrEmpty(usedElemental) ? usedElemental : "None";

        return $"[DefendData] RecoveredHp: {recoveredHp}, Coin: {coin}, " +
               $"UsedElemental: {elementalStr}, " +
               $"UsedPerks: [{perksStr}], UsedItemCodes: [{itemsStr}]";
    }
}