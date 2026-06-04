// 202422170 주형준
public readonly struct GameEndEvent
{
    public readonly PlayerInfoModel player1;
    public readonly PlayerInfoModel player2;
    public readonly bool isPlayer1Winner;

    public GameEndEvent(PlayerInfoModel player1, PlayerInfoModel player2, bool isPlayer1Winner)
    {
        this.player1 = player1;
        this.player2 = player2;
        this.isPlayer1Winner = isPlayer1Winner;
    }
}