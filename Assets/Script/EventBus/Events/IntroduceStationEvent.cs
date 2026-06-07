public readonly struct IntroduceStationEvent
{
    public readonly string station;
    public readonly string title;
    public readonly string description;
    public readonly PlayerInfoModel player1;
    public readonly PlayerInfoModel player2;
    
    // 기본값으로 null을 지정합니다.
    

    public IntroduceStationEvent(string station, string title, string description, PlayerInfoModel player1, PlayerInfoModel player2)
    {
        this.station = station;
        this.title = title;
        this.description = description;
        this.player1 = player1 ?? new PlayerInfoModel("1", player1.attacking);
        this.player2 = player2 ?? new PlayerInfoModel("1", player2.attacking);
    }
}