// 202422170 주형준
public static class HandInfoProvider
{
    public static string GetDisplayName(HandElementalType hand) => hand switch
    {
        HandElementalType.FIRE      => "Fire",
        HandElementalType.WATER     => "Water",
        HandElementalType.WIND      => "Wind",
        HandElementalType.LIGHTNING => "Lightning",
        HandElementalType.POISON    => "Poison",
        HandElementalType.PLANT     => "Plant",
        _                           => hand.ToString()
    };

    public static string GetDescription(HandElementalType hand) => hand switch
    {
        HandElementalType.FIRE      => "Deal 2 dmg/turn for 2 turns on attack",
        HandElementalType.WATER     => "Recover HP +5 on defense success",
        HandElementalType.WIND      => "Dodge 1 attack per round",
        HandElementalType.LIGHTNING => "+2 bonus damage on attack success",
        HandElementalType.POISON    => "Enemy max HP -5 on attack success",
        HandElementalType.PLANT     => "Max HP +15",
        _                           => ""
    };

    // 파일명이 enum 이름과 동일하므로 ToString() 그대로 사용
    public static string GetImagePath(HandElementalType hand) => $"Hands/{hand}";
}