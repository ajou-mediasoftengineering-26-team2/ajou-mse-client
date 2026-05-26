using UnityEngine.UIElements;

public readonly struct HitAnimation
{
    public readonly HitActionType hitAction;
    public readonly BattleRole Role;
    public readonly Player Player;
    public readonly VisualElement Element;

    public HitAnimation(BattleRole role, 
        Player player, 
        HitActionType type,
        VisualElement element)
    {
        Role = role;
        Player = player;
        hitAction = type;
        Element = element;
    }
}
