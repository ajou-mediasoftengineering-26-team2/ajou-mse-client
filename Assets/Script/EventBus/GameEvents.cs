//202322158 이준상

/// <summary>
/// Define Game Events
/// </summary>


public enum SfxType
{
    ButtonClick,
    PlayerAttack,
    EnemyAttack,
    PlayerRoundWin,
    EnemyRoundWin
}

public enum HitActionType
{
    Left, Right, Both1, Both5, Both10
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


public enum BattleRole { Attack, Defense}
public enum Player {First, Second}

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

public readonly struct RoundOver
{
    public readonly bool isWin;
    
    public RoundOver(bool isWin) => this.isWin = isWin;
    
}

public readonly struct CameraAction
{
    public readonly CameraType ActionCode;
    public CameraAction(CameraType cameraType) => this.ActionCode = cameraType;
}

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

public readonly struct HitAnimationZoomIn
{
}

