//202322158 이준상

//Define Lobby state to use firebase realtime database.
public enum LobbyState
{
    LOBBY_WAITING,
    LOBBY_START_COUNTDOWN,
    GAME_ATK_CHOICE,
    GAME_DEF_CHOICE,
    GAME_ROUND_END_PLAYER_KO,
    //퍽이랑 아이템 추가했습니다
    GAME_PERK_CHOICE,
    GAME_ITEM_ANIMATION,
    END_RESULT,
    END_PLAYER_DISCONNECTED
}