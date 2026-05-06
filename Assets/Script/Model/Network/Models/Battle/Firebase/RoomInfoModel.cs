using System;

[Serializable]
public class RoomInfoModel
{
    public PlayerInfoModel player1Info;
    public PlayerInfoModel player2Info;
    public string attackingPlayer;
    public long timeEnd;
}