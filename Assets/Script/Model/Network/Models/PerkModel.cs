using System;

//202422170 주형준
[Serializable]
public class PerkOption
{
    public int    id;
    public string title;
    public string description;
}

// GET perk/choices - 퍽 선택지 3개
[Serializable]
public class GetPerkChoicesResponse
{
    public PerkOption perk1;
    public PerkOption perk2;
    public PerkOption perk3;
}

// POST perk/select - 퍽 선택 요청
[Serializable]
public class PostSelectPerkRequest
{
    public string id;    // playerId
    public int    perkId;
}

// POST perk/select - 퍽 선택 응답
[Serializable]
public class SelectPerkResponse { }