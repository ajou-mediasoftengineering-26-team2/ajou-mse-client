using System;
using System.Collections.Generic;

[Serializable]
public class Damage
{
    public string attackType;
    
    public int coin;
    public int damage;
    public int damageIndex;
    public bool ko;
    public int recoveredHp;
    public List<string> statusEffects;
    public List<string> usedItems;
    public List<string> usedPerks;
    public Damage() { }


    public Damage(string attackType, int coin, int damage, int damageIndex, bool ko, int recoveredHp, List<string> statusEffects, List<string> usedItems, List<string> usedPerks)
    {
        this.attackType = attackType;
        this.coin = coin;
        this.damage = damage;
        this.damageIndex = damageIndex;
        this.ko = ko;
        this.recoveredHp = recoveredHp;
        this.statusEffects = statusEffects;
        this.usedItems = usedItems;
        this.usedPerks = usedPerks;
    }
    
    public override string ToString()
    {
        string effects = statusEffects != null && statusEffects.Count > 0 ? string.Join(", ", statusEffects) : "None";
        string items = usedItems != null && usedItems.Count > 0 ? string.Join(", ", usedItems) : "None";
        string perks = usedPerks != null && usedPerks.Count > 0 ? string.Join(", ", usedPerks) : "None";

        return $"[Damage] Index: {damageIndex} | Type: {attackType} | Dmg: {damage} (KO: {ko}) | Coin: {coin} | RecHP: {recoveredHp} | Effects: [{effects}] | Items: [{items}] | Perks: [{perks}]";
    }
}