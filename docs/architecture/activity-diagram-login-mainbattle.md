# Activity Diagram (Login -> MainBattle, code-based)

```mermaid
flowchart TB
  A([Start]) --> B[앱 진입 / LoginView.OnEnable]
  B --> C[LoginViewModel Initialize\n역 정보 Firebase 구독]
  C --> D[사용자 ID 입력]
  D --> E[Create 버튼 클릭]
  E --> F[OnSubmitID(playerName)]
  F --> G[LoginRepository.PostUserID\nPOST auth/player]

  G --> H{로그인 성공?}
  H -- No --> I[ErrorMsg 설정 + Toast 표시]
  I --> D

  H -- Yes --> J[PlayerId/LobbyId 저장\nIsSuccess=true]
  J --> K[로비 매치 구독 시작\nmatches/{lobbyId}, players]
  K --> L[대기 UI 표시 + SubwayDisplay 시작]

  L --> M{조건 충족?\nenemyId 존재 &&\nmatchState != LOBBY_WAITING}
  M -- No --> L
  M -- Yes --> N[SceneDataBridge 저장\nIsMatchStarted=true]
  N --> O[SceneManager.LoadScene\nMainBattle]

  O --> P[MainBattleView.OnEnable]
  P --> Q[SetPlayerAndMatchId]
  Q --> R[Firebase 실시간 구독\nmatch + me + enemy]
  R --> S[UI 상태 갱신\nHP/턴/카운트다운/라벨]

  S --> T{내 턴 선택 가능?}
  T -- No --> S
  T -- Yes --> U[액션 카드 클릭]
  U --> V[OnHandAction(choice)]
  V --> W[MainBattleRepository.PutChoice\nPUT turn/choice]
  W --> X[EventBus Publish\nAttackStartedEvent]
  X --> S

  S --> Y{매치 종료 상태?\nEND_RESULT or END_PLAYER_DISCONNECTED}
  Y -- No --> S
  Y -- Yes --> Z([End])
```

