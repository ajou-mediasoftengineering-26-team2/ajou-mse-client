public readonly struct AttackStartedEvent
{
    public readonly bool IsPlayer;
    public AttackStartedEvent(bool isPlayer) => IsPlayer = isPlayer;
}
