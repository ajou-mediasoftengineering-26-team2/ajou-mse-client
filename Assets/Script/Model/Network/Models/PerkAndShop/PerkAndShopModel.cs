using System;

// 202422170 주형준

// PUT /perk/choice request
[Serializable]
public class PutPerkAndShopChoiceRequest
{
    public string playerId;
    public string perk;
}

// PUT /perk/ack request
[Serializable]
public class PutPerkAndShopAckRequest
{
    public string playerId;
}