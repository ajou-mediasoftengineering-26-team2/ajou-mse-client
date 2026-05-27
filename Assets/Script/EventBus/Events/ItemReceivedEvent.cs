// 202422170 주형준
public readonly struct ItemReceivedEvent
{
    public readonly string ItemCode;
    public ItemReceivedEvent(string itemCode) => ItemCode = itemCode;
}