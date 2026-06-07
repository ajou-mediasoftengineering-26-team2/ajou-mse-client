public readonly struct StatusEvent
{
    public readonly string status;
    public readonly Player player;

    public StatusEvent(string status, Player player)
    {
        this.status = status;
        this.player = player;
    }
}