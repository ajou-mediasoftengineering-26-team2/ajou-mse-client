using System;

[Serializable]
public class PostLoginRequest
{
    public string playerName;
}

[Serializable]
public class PostLoginResponse
{
    public string playerId;
    public string lobbyId;
}