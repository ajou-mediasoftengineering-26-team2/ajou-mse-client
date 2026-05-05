
//202322158 이준상
public static class HandActionExtensions

{
    public static int GetDamage(this HandActionType action)
    {
        return action switch
        {
            HandActionType.None               => 0,
            HandActionType.SingleHandFlipLeft  => 1,
            HandActionType.SingleHandFlipRight => 1,
            HandActionType.BothHandsFlip      => 2,
            HandActionType.InsertBetweenHands  => 5,
            HandActionType.ShakeOverHands      => 10, 
            _ => 0
        };
    }

    public static string GetName(this HandActionType action, bool isAttack)
    {
        return isAttack switch
        {
            // 공격 중일 때 (기존 로직)
            true => action switch
            {
                HandActionType.None => "None",
                HandActionType.SingleHandFlipLeft  => "Left",
                HandActionType.SingleHandFlipRight => "Right",
                HandActionType.BothHandsFlip       => "Both",
                HandActionType.InsertBetweenHands  => "Stab",
                HandActionType.ShakeOverHands      => "Wave",
                _ => "Waiting"
            },

            // 공격 중이 아닐 때 (대기 혹은 방어 등 다른 상태)
            false => action switch
            {
                HandActionType.None => "None",
                HandActionType.SingleHandFlipLeft  => "Left",
                HandActionType.SingleHandFlipRight => "Right",
                HandActionType.BothHandsFlip       => "Both",
                HandActionType.InsertBetweenHands  => "Defense",
                HandActionType.ShakeOverHands      => "Pause",
                _ => "Waiting"
            },
        };
    }
}