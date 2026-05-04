using System;

// Firebase: matches/<lobbyId>/players/<playerId>
[Serializable]
public class PlayerInfoModel
{
    public int ready;
    public int wins;
    public int hp;
    public string username;
    public bool attacking;
    public bool selecting;
}

// Firebase: matches/<lobbyId>
// players 필드는 Dictionary라 JsonUtility 미지원 → 별도 경로로 각각 구독
[Serializable]
public class MatchInfoModel
{
    public string station;
    public string countdownStartTime;
    public int countdownSec;
    public string state;
    public int winnerPlayerIdx;
    public int currentPlayerIdx;
    public int attackerPlayerIdx;
    public int currentTurn;
    public int currentRound;
}

// PUT turn/choice 요청 바디
[Serializable]
public class PutChoiceRequest
{
    public string id;
    public string choice;
}

// PUT turn/choice 응답 data (서버가 null로 내려보냄)
[Serializable]
public class PostHandActionRequest
{
    public string playerId;
    public int moveType;
}


[Serializable]
public class FBStationModel
{
    public string currentStation;
}
