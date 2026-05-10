//202322158 이준상
// Firebase: matches/<lobbyId>
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