//202322158
// Firebase: matches/<lobbyId>/players/<playerId>

using System;
using System.Collections.Generic;

[Serializable]
public class PlayerInfoModel
{
    // JSON 데이터와 이름 및 타입을 일치시켰습니다.
    public bool ready;
    public int wins;
    public int hp;
    public string username;
    public bool attacking;
    public bool selecting;
    public bool finalWinner;
    public string handChoice;
    
    // 리스트 타입 매칭
    public List<string> itemList;
    public List<string> receivedItemList;
    
    public PlayerInfoModel()
    {
    }

    public PlayerInfoModel(string username)
    {
        this.username = username;
    }

    public PlayerInfoModel(string username, bool attacking)
    {
        this.username = username;
        this.attacking = attacking;
    }
}