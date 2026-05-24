public readonly struct HitAnimation
{
    public readonly HitActionType hitAction;
    public readonly BattleRole Role;
    public readonly Player Player;

    public HitAnimation(BattleRole role, Player player, HitActionType type)
    {
        Role = role;
        Player = player;
        hitAction = type;
    }
}
