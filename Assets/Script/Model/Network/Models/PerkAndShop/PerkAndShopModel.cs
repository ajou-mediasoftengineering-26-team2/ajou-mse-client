using System;

// 202422170 주형준
//서버팀 API 확정 후 필드 수정 , 예비로 구조 잡아놓기


[Serializable]
public class PerkOption
{
    public int    id;
    public string title;
    public string description;
}

// GET shop/info  response
[Serializable]
public class GetPerkAndShopInfoResponse
{
    // Shop
    public int    money;
    public int    handLevel;
    public int    upgradeCost;
    public string currentStat;
    public string nextStat;

    // Perk
    public PerkOption perk1;
    public PerkOption perk2;
    public PerkOption perk3;
}

// POST shop/upgrade request
[Serializable]
public class PostPerkAndShopUpgradeRequest
{
    public string id; // playerId
}

// POST shop/upgrade response
[Serializable]
public class PerkAndShopUpgradeResponse
{
    public int    money;
    public int    handLevel;
    public int    upgradeCost;
    public string currentStat;
    public string nextStat;
}

// POST shop/perk/select request
[Serializable]
public class PostPerkAndShopSelectRequest
{
    public string id;    // playerId
    public int    perkId;
}

// POST shop/perk/select response
[Serializable]
public class PerkAndShopSelectResponse { }