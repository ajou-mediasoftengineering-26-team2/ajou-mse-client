public readonly struct ActionSelectedEvent
{
    public readonly PlayerInfoModel Player1;
    public readonly PlayerInfoModel Player2;

    public ActionSelectedEvent(Player player,PlayerInfoModel player1, PlayerInfoModel player2)
    {
        Player1 = player1;
        Player2 = player2;
    }
    
}
