public readonly struct HandElementalChoiceResult
{
    public readonly PlayerInfoModel player1;
    public readonly PlayerInfoModel player2;

    public HandElementalChoiceResult(PlayerInfoModel p1, PlayerInfoModel p2)
    {
        player1 = p1;
        player2 = p2;
    }
}