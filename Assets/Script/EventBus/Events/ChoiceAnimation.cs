public readonly struct ChoiceAnimation
{
    public readonly PlayerInfoModel Player1;
    public readonly PlayerInfoModel Player2;

    public ChoiceAnimation(PlayerInfoModel player1, PlayerInfoModel player2)
    {
        Player1 = player1;
        Player2 = player2;
    }
}