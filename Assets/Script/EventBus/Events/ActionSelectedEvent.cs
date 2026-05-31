public readonly struct ActionSelectedEvent
{
    public readonly HandActionType ActionCode;
    public readonly BattleRole Role;
    public readonly Player Player;

    public ActionSelectedEvent(HandActionType actionCode, BattleRole role, Player player)
    {
        Role = role;
        ActionCode = actionCode;
        Player = player;
    }
}
