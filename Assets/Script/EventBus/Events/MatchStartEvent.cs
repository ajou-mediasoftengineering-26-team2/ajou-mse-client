public readonly struct MatchStartEvent
{
    public readonly PlayerInfoModel player1;
    public readonly PlayerInfoModel player2;

    public MatchStartEvent(PlayerInfoModel player1, PlayerInfoModel player2)
    {
        this.player1 = player1;
        this.player2 = player2;
    }
}