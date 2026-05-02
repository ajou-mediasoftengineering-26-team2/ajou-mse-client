//202322158 이준상

public enum SfxType
{
    ButtonClick,
    PlayerAttack,
    EnemyAttack,
    PlayerRoundWin,
    EnemyRoundWin
}

public readonly struct PlaySfxEvent
{
    public readonly SfxType SfxType;
    public PlaySfxEvent(SfxType sfxType) => SfxType = sfxType;
}

public readonly struct ButtonEvent
{ }

public readonly struct AttackStartedEvent
{
    public readonly bool IsPlayer;
    public AttackStartedEvent(bool isPlayer) => IsPlayer = isPlayer;
}

public readonly struct RoundWonEvent
{
    public readonly bool IsPlayer;
    public RoundWonEvent(bool isPlayer) => IsPlayer = isPlayer;
}

public readonly struct LoginPanelHideEvent
{ }

public readonly struct ActionSelectedEvent
{
    public readonly HandActionType ActionCode;
    public ActionSelectedEvent(HandActionType actionCode)
    {
        ActionCode = actionCode;
    }
}