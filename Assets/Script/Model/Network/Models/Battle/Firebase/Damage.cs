using System;

[Serializable]
public class Damage
{
    public string attackType;
    
    public int coin;
    public int damage;
    public int damageIndex;
    public bool ko;
    public int recoveredHp;

    public Damage() { }

    public Damage(string attackType, int coin, int damage, int damageIndex, bool ko, int recoveredHp)
    {
        this.attackType = attackType;
        this.coin = coin;
        this.damage = damage;
        this.damageIndex = damageIndex;
        this.ko = ko;
        this.recoveredHp = recoveredHp;
    }
}