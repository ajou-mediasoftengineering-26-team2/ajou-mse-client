using System;

[Serializable]
public class PlayerInfoModel
{
    public int hp;
    public int[] items;
    public int[] perks;
    public int[] statusEffects;
    public int roundWin;
    public int money;
}

[Serializable]
public class RoomInfoModel
{
    public PlayerInfoModel player1Info;
    public PlayerInfoModel player2Info;
    public string attackingPlayer;
    public long timeEnd;
}

[Serializable]
public class StationModel
{
    public string stationName;
}

[Serializable]
public class PostHandActionRequest
{
    public string playerId;
    public int moveType;
}
