//202322158
// Firebase: matches/<lobbyId>/players/<playerId>

using System;

[Serializable]
public class PlayerInfoModel
{
    public int ready;
    public int wins;
    public int hp;
    public string username;
    public bool attacking;
    public bool selecting;
}