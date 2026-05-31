//202322158 이준상

//Define Lobby state to use firebase realtime database.
public enum LobbyState
{
    // ==========================================
    // 1. Lobby & Match Start
    // ==========================================
    LOBBY_WAITING,                  // 로비 대기 중
    LOBBY_START_COUNTDOWN,          // 로비 시작 카운트다운
    MATCH_START,                    // [추가] 매치 시작 애니메이션 출력 (이번 역은~~ 역입니다 등)

    GAME_ITEM_ANIMATION,
    // ==========================================
    // 2. Round Start & Turn Loop
    // ==========================================
    GAME_ROUND_START_ANIMATION,     // [추가] 라운드 시작 애니메이션 출력 (동전 던지기 등)
    GAME_PLAYER_CHOICE,             // 클라이언트가 행동 선택 (기존 에넘 유지)
    GAME_ATK_CHOICE,                // 공격 선택 (기존 에넘 유지)
    GAME_DEF_CHOICE,                // 방어 선택 (기존 에넘 유지)
    GAME_CHOICE_FINISHED,           // 클라이언트가 선택한 행동을 서버로 보냄
    GAME_TURN_ANIMATION,            // 클라이언트가 공격/방어 애니메이션 출력

    // ==========================================
    // 3. Round End & Reward (Perk / Elemental / Item)
    // ==========================================
    GAME_ROUND_END_PLAYER_KO,       // 클라이언트가 KO 애니메이션을 출력함
    GAME_PERK_CHOICE,               // 클라이언트가 Perk(또는 Elemental) 선택함
    GAME_PERK_ITEM_RECEIVING,       // [추가] 5초 후 데이터 업데이트 및 수령 애니메이션 출력 (노션 명칭 반영)
    GAME_ELEMENTAL_CHOICE,
    // ※ 만약 기획 상 기재된 (GAME_ELEMENTAL_CHOICE / RECEIVING)을 
    // Perk과 별개의 State 코드로 분리해 관리해야 한다면 아래 주석을 해제하고 사용하세요.
    // GAME_ELEMENTAL_CHOICE,       
    // GAME_ELEMENTAL_RECEIVING,    

    // ==========================================
    // 4. Match End
    // ==========================================
    END_RESULT,                     // 게임 끝 애니메이션 및 결과 창 출력
    END_PLAYER_DISCONNECTED          // 플레이어 탈주/연결 끊김 처리
    ,
    GAME_ELEMENTAL_RECEIVING
}