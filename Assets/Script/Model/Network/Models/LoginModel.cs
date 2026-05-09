using System;

//202422170 주형준
[Serializable]
public class PostLoginRequest
{
    public string playerName;
}

[Serializable]
public class PostLoginResponse
{
    public string playerId;
    public string matchId;
}