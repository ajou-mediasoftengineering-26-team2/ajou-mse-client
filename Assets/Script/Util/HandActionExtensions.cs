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

    public static string GetName(this HandActionType action)
    {
        return action switch
        {
            HandActionType.None => "None",
            HandActionType.SingleHandFlipLeft  => "Left",
            HandActionType.SingleHandFlipRight => "Right",
            HandActionType.BothHandsFlip      => "Both",
            HandActionType.InsertBetweenHands  => "Stab",
            HandActionType.ShakeOverHands      => "Wave",
            _ => "Waiting"
        };
    }
}