public readonly struct IntroduceStationEvent
{
    public readonly string station;
    public readonly PlayerInfoModel player1;
    public readonly PlayerInfoModel player2;
    
    // 기본값으로 null을 지정합니다.
    public IntroduceStationEvent(string station,  
        PlayerInfoModel player1 = null, 
        PlayerInfoModel player2 = null)
    {
        this.station = station;
        
        // 읽기 전용 필드이므로 생성과 동시에 null 체크 후 바로 할당합니다.
        this.player1 = player1 ?? new PlayerInfoModel("Junsang", true);
        this.player2 = player2 ?? new PlayerInfoModel("asdf", false);
    }
}