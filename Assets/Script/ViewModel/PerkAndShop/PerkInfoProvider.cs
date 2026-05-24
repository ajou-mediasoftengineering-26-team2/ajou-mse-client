// 202422170 주형준
public static class PerkInfoProvider
{
    public static string GetDisplayName(PerkType perk) => perk switch
    {
        PerkType.IRON_FIST      => "Iron Fist",
        PerkType.VAMPIRISM      => "Vampirism",
        PerkType.THRIFTY        => "Thrifty",
        PerkType.GRIT           => "Grit",
        PerkType.TAUNT          => "Taunt",
        PerkType.FOCUS          => "Focus",
        PerkType.LUCK           => "Luck",
        PerkType.UNYIELDING     => "Unyielding",
        PerkType.INSERT_MASTER  => "Insert Master",
        PerkType.FLEXIBLE_HANDS => "Flexible Hands",
        PerkType.FLEXIBLE_MIND  => "Flexible Mind",
        PerkType.RICH           => "Rich",
        PerkType.WISE_INVESTOR  => "Wise Investor",
        _ => perk.ToString()
    };

    public static string GetDescription(PerkType perk) => perk switch
    {
        PerkType.IRON_FIST      => "All attack damage +2",
        PerkType.VAMPIRISM      => "Recover HP +3 on attack success",
        PerkType.THRIFTY        => "All coin gain +2",
        PerkType.GRIT           => "Halve damage received when HP is below 15",
        PerkType.TAUNT          => "Enemy HP -5 at round start",
        PerkType.FOCUS          => "Shake damage +3",
        PerkType.LUCK           => "Bonus coin +3 on defense success",
        PerkType.UNYIELDING     => "Max damage received per hit capped at 5",
        PerkType.INSERT_MASTER  => "Insert damage 5 → 7",
        PerkType.FLEXIBLE_HANDS => "Left ↔ Right hand group defense available",
        PerkType.FLEXIBLE_MIND  => "Insert ↔ Shake group defense available",
        PerkType.RICH           => "Instantly gain +30 coins",
        PerkType.WISE_INVESTOR  => "Attack damage +1 per 10 coins held",
        _ => ""
    };
}