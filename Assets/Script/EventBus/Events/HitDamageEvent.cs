public readonly struct HitDamageEvent
{
    public readonly Damage damage;
    public readonly bool isLeft; 

    public HitDamageEvent(Damage damage, bool isLeft)
    {
        this.damage = damage;
        this.isLeft = isLeft;
    }
}