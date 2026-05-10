using System;

//202422170 주형준
// 서버팀 API 확정 후 필드 수정
// GET shop/info - 상점 초기 정보
[Serializable]
public class GetShopInfoResponse
{
    public int    handLevel;
    public int    money;
    public int    upgradeCost;
    public string currentStat;
    public string nextStat;
}

// POST shop/upgrade - 업그레이드 요청
[Serializable]
public class PostUpgradeRequest
{
    public string id; // playerId
}

// POST shop/upgrade - 업그레이드 응답
[Serializable]
public class UpgradeResponse
{
    public int    handLevel;
    public int    money;
    public int    upgradeCost;
    public string currentStat;
    public string nextStat;
}