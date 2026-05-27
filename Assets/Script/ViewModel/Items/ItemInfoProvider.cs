// 202422170 주형준

public static class ItemInfoProvider
{
    public static string GetDisplayName(ItemType item) => item switch
    {
        ItemType.HP_POTION => "HP Potion",
        ItemType.GUKBAP    => "Gukbap",
        _                  => item.ToString()
    };

    public static string GetDescription(ItemType item) => item switch
    {
        //ex
        ItemType.HP_POTION => "Recover HP +10",
        ItemType.GUKBAP    => "Recover HP +15",
        _                  => ""
    };
}