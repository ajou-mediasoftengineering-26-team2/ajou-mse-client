//202322158 이준상

//Define Lobby state to use firebase realtime database.
public enum LobbyState
{
    LOBBY_WAITING,
    LOBBY_START_COUNTDOWN,
    GAME_ATK_CHOICE,
    GAME_DEF_CHOICE,
    GAME_ROUND_END_PLAYER_KO,
    END_RESULT,
    END_PLAYER_DISCONNECTED
}