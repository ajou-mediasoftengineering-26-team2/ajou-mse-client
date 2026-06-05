// 202422170 주형준
public readonly struct RoundResultEvent
{
    public readonly bool isWin;
    public readonly int currentRound;
    public readonly int coin;

    public RoundResultEvent(bool isWin, int currentRound, int coin)
    {
        this.isWin        = isWin;
        this.currentRound = currentRound;
        this.coin         = coin;
    }
}