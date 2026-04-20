using System;

[Serializable]
public class RoomInfoModel
{
    public string timeEnd;
    public int currentTurn;
    public int attackingPlayer;
    public int currentRound;
    public bool isDefenseSuccess;
    public PlayerInfoModel player1Info;
    public PlayerInfoModel player2Info;
}