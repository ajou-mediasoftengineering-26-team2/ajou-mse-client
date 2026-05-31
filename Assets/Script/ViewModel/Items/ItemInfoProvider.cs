using System;
// 202422170 주형준
public static class ItemInfoProvider
{
    public static string GetDisplayName(ItemType item) => item switch
    {
        ItemType.EMERGENCY_TREATMENT => "Emergency Treatment",
        ItemType.SHIELD              => "Shield",
        ItemType.RAGE                => "Rage",
        ItemType.ESCAPE              => "Escape",
        ItemType.REVERSAL            => "Reversal",
        ItemType.HEALING_POTION      => "Healing Potion",
        ItemType.RESISTANCE          => "Resistance",
        ItemType.FINAL_BLOW          => "Final Blow",
        ItemType.PANACEA             => "Panacea",
        _                            => item.ToString()
    };
    
    public static string GetDescription(ItemType item) => item switch
    {
        ItemType.EMERGENCY_TREATMENT => "HP ≤ 15: Instantly recover HP +20",
        ItemType.SHIELD              => "HP ≤ 20: Next turn damage reduced to 0",
        ItemType.RAGE                => "HP ≤ 10: Next attack damage +5",
        ItemType.ESCAPE              => "HP ≤ 5: Instantly HP +10, Coin +5",
        ItemType.REVERSAL            => "HP ≤ 10: Instantly enemy HP -10",
        ItemType.HEALING_POTION      => "HP ≤ 25: Instantly recover HP +10",
        ItemType.RESISTANCE          => "HP ≤ 20: This round damage received -2",
        ItemType.FINAL_BLOW          => "HP ≤ 5: Next attack damage x2",
        ItemType.PANACEA             => "Remove all status effects",
        _                            => ""
    };  
}
