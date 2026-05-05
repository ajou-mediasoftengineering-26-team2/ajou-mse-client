// Firebase: matches/<lobbyId>
// players 필드는 Dictionary라 JsonUtility 미지원 → 별도 경로로 각각 구독

using System;
using System.Collections.Generic;

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
    public Dictionary<string, PlayerInfoModel> players;
}