using System;

[Serializable]
public class EmptyResponse{}

[Serializable]
public class PutPerkAckRequest
{
    public string playerId;
}

[Serializable]
public class PutPerkChoiceRequest
{
    public string playerId;
    public string perk;
}