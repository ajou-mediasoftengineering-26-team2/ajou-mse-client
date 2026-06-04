// 202422170 주형준
public readonly struct RoundResultEvent
{
    public readonly bool isWin;
    public readonly int currentRound;

    public RoundResultEvent(bool isWin, int currentRound)
    {
        this.isWin = isWin;
        this.currentRound = currentRound;
    }
}