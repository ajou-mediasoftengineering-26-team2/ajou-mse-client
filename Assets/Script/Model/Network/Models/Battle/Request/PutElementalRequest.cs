using System;

// 202422170 주형준
// PUT /elemental/choice, /elemental/ack request
[Serializable]
public class PutElementalChoiceRequest
{
    public string playerId;
    public string handElemental;
}

[Serializable]
public class PutElementalAckRequest
{
    public string playerId;
}
