using System;

//202322158 이준상
//Room information DTO
[Serializable]
public class RoomInfoModel
{
    public PlayerInfoModel player1Info;
    public PlayerInfoModel player2Info;
    public string attackingPlayer;
    public long timeEnd;
}