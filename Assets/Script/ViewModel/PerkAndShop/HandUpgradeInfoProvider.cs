// 202422170 주형준
public static class HandUpgradeInfoProvider
{
    public static int GetUpgradeCost(int targetLevel) => targetLevel switch
    {
        1 => 10,   // 서버 0→1 (표시: Lv.1→2)
        2 => 25,   // 서버 1→2 (표시: Lv.2→3)
        3 => 45,   // 서버 2→3 (표시: Lv.3→4)
        4 => 75,   // 서버 3→4 (표시: Lv.4→5)
        _ => 0
    };
    
    public static string GetEffectDescription(HandElementalType hand, int level) => hand switch
    {
        HandElementalType.FIRE => level switch
        {
            1 => "Deal 2 dmg/turn for 2 turns",
            2 => "Deal 2 dmg/turn for 3 turns",
            3 => "Deal 3 dmg/turn for 3 turns",
            4 => "Deal 3 dmg/turn for 4 turns",
            5 => "Deal 4 dmg/turn for 4 turns",
            _ => ""
        },
        HandElementalType.LIGHTNING => level switch
        {
            1 => "+2 bonus damage on hit",
            2 => "+3 bonus damage on hit",
            3 => "+4 bonus damage on hit",
            4 => "+5 bonus damage on hit",
            5 => "+6 bonus damage on hit",
            _ => ""
        },
        HandElementalType.WATER => level switch
        {
            1 => "HP +5 on defense success",
            2 => "HP +7 on defense success",
            3 => "HP +9 on defense success",
            4 => "HP +12 on defense success",
            5 => "HP +15 on defense success",
            _ => ""
        },
        HandElementalType.PLANT => level switch
        {
            1 => "Dmg 4+: reduce by 1",
            2 => "Dmg 4+: reduce by 2",
            3 => "Dmg 3+: reduce by 2",
            4 => "Dmg 3+: reduce by 3",
            5 => "Dmg 3+: reduce by 4",
            _ => ""
        },
        HandElementalType.WIND => level switch
        {
            1 => "Dodge 1 attack per round",
            2 => "Dodge 2 attacks per round",
            3 => "Dodge 2x + coin +5 on dodge",
            4 => "Dodge 3x + coin +5 on dodge",
            5 => "Dodge 4x + coin +5 on dodge",
            _ => ""
        },
        HandElementalType.POISON => level switch
        {
            1 => "Enemy damage -1 (stacking)",
            2 => "Enemy damage -2 (stacking)",
            3 => "Enemy damage -3 (stacking)",
            4 => "Enemy damage -4 (stacking)",
            5 => "Enemy damage -5 (stacking)",
            _ => ""
        },
        _ => ""
    };
}