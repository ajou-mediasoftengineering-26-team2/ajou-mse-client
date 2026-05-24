public readonly struct RoundWonEvent
{
    public readonly bool IsPlayer;
    public RoundWonEvent(bool isPlayer) => IsPlayer = isPlayer;
}
