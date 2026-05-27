//202322158
// Firebase: matches/<lobbyId>/players/<playerId>

using System;
using System.Collections.Generic;

[Serializable]
public class PlayerInfoModel
{
    public int ready;
    public int wins;
    public int hp;
    public string username;
    public bool attacking;
    public bool selecting;
    
    //형준
    //퍽에 대해서 추가했습니다 + 아이템
    public List<string> perkChoiceList;
    public List<string> perkList;
    public List<string> itemList;
    public string receviedItem;//서버 오타 -> receivedItem

    public PlayerInfoModel(string username)
    {
        this.username = username;
    }

    public PlayerInfoModel()
    {
    }

    public PlayerInfoModel(string username, bool attacking)
    {
        this.username = username;
        this.attacking = attacking;
    }
}