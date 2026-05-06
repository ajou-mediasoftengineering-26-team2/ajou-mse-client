// PUT turn/choice 응답 data (서버가 null로 내려보냄)

using System;

[Serializable]
public class PostHandActionRequest
{
    public string playerId;
    public string moveType;
}