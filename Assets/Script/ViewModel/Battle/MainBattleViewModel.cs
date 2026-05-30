using System;
using System.Collections;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

//202322158 이준상


/// <summary>
/// ViewModel for the main battle scene.
/// Maintains HP, rounds, timer, turn and selection state via observables.
/// Subscribes to Firebase match and player nodes to reflect real-time game state.
/// Sends player actions to server and exposes UI-friendly labels and countdown.
/// </summary>
public class MainBattleViewModel : ViewModelBase
{
    // Repository and runtime state (player/lobby ids, timers, subscriptions)
    private readonly IMainBattleRepository _repository;
    private readonly IRoundRepository _roundRepository;
    private readonly IElementalRepository _elementalRepository;
    private string _playerId;
    private string _lobbyId;
    private string _enemyId;
    private bool _isTimerRunning = false;
    private bool _firebaseSubscribed;
    private CancellationTokenSource _timerCts;
    private PlayerInfoModel player1;
    private PlayerInfoModel player2;


    // ── HP ──────────────────────────────────────────────────────────
    // Player HP observables (Left = local player, Right = remote player)
    public Observable<int> LeftHp { get; } = new Observable<int>();
    public Observable<int> RightHp { get; } = new Observable<int>();

    // ── round win maker ─────────────────────────────────────────────

    // Per-round win counters (used to display round wins)
    public Observable<int> LeftRoundWin { get; } = new Observable<int>();
    public Observable<int> RightRoundWin { get; } = new Observable<int>();


    // ── timer ──────────────────────────────────────────────────────
    // Countdown timer observable (used by UI for remaining seconds)
    public Observable<int> RemainingSeconds { get; } = new Observable<int>();

    // ── attacker? ─────────────────────────────────────────────────
    // Whether local player currently has attacking priority
    public Observable<bool> IsAttacker { get; } = new Observable<bool>();

    // ── station name ─────────────────────────────────────────────────────
    // Display name of the subway station for the match
    public Observable<string> StationName { get; } = new Observable<string>();

    // ──  game state 1 ───────────────────────────────────────────────────
    // Match-level state exposed to the UI
    public Observable<LobbyState> MatchState { get; } = new Observable<LobbyState>();
    public Observable<int> CurrentRound { get; } = new Observable<int>();
    public Observable<int> WinnerPlayerIdx { get; } = new Observable<int>(-1);


    //  ── game state 2 ───────────────────────────────────────────────────
    // Selection flags indicating if each player is currently selecting
    public Observable<bool> MySelecting { get; } = new Observable<bool>();
    public ObservableEvent<bool> MySelectingE { get; } = new ObservableEvent<bool>();
    public Observable<bool> EnemySelecting { get; } = new Observable<bool>();


    // ── money ──────────────────────────────────────────────────────────
    // Player currency
    public Observable<int> Money { get; } = new Observable<int>();

    // Status label displayed in UI (e.g., YOUR TURN, ENEMY TURN, GAME OVER)
    public Observable<string> LabelState { get; } = new Observable<string>();

    // ── name ──────────────────────────────────────────────────────────
    public Observable<String> MyName { get; } = new Observable<String>();

    public Observable<String> EnemyName { get; } = new Observable<string>();

    // current Turn
    // Current turn index
    public Observable<int> CurrentTurn { get; } = new Observable<int>();

    // Formatted countdown string (ss.ff)
    public Observable<string> CountDown { get; } = new Observable<string>();

    public Observable<string> HoverTest { get; } = new Observable<string>();

    // ── camera ──────────────────────────────────────────────────────────
    public Observable<CameraType> CameraPoint { get; } = new Observable<CameraType>();

    // ── Current action ──────────────────────────────────────────────────────────
    public Observable<HandActionType> CurrentHandAction { get; } =
        new Observable<HandActionType>(HandActionType.SINGLE_HAND_FLIP_LEFT);

    public Observable<string> CurrentHandActionText { get; } = new Observable<string>("Left");


    public MainBattleViewModel()
    {
        // _playerId = playerId;
        // _lobbyId = lobbyId;
        _repository = RepositoryFactory.Instance.Get<IMainBattleRepository>();
        _roundRepository = RepositoryFactory.Instance.Get<IRoundRepository>();
        _elementalRepository = RepositoryFactory.Instance.Get<IElementalRepository>();
    }

    public override void Initialize()
    {
        if (IsInitialized) return;
        base.Initialize();
        CurrentHandActionText.Value = "Left";
        TryStartFirebaseSubscriptions();

        eventJunsang();
    }

    private void eventJunsang()
    {
        EventBus.Subscribe<GameRoundStartAnimationAckEvent>(GRSAAckEvent);
    }

    private void GRSAAckEvent(GameRoundStartAnimationAckEvent obj)
    {
    }

    /// <summary>
    /// Sends player's chosen action to the server and publishes local AttackStartedEvent.
    /// Validates presence of playerId before sending.
    /// </summary>
    /// <param name="choice"></param>
    public async void OnHandAction(HandActionType choice)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_playerId))
            {
                Debug.LogError("PutChoice skipped: playerId is empty.");
                return;
            }
            //network communication to server(spring)
            string choiceValue = choice.ToString();
            Debug.Log($"PutChoice request -> id={_playerId}, choice={choiceValue}");
            await _repository.PutChoice(_playerId, choiceValue);
            EventBus.Publish(new AttackStartedEvent(isPlayer: true));
        }
        catch (NetworkException e)
        {
            Debug.LogError($"PutChoice failed: http={e.ResponseCode}, apiCode={e.ApiErrorCode}, msg={e.ErrorMessage}");
            Debug.LogException(e);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public void ChangeValue()
    {
        LeftRoundWin.Value = 2;
        Debug.Log(LeftRoundWin.Value + "Teststest");
    }

    /// <summary>
    /// Configure this ViewModel with the local player id, match id, and enemy id.
    /// Triggers starting of Firebase subscriptions when all ids are provided.
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="matchId"></param>
    /// <param name="enemyId"></param>
    public void HoverTesttest(string test)
    {
        HoverTest.Value = test;
    }

    /// <summary>
    /// Ensure Firebase is initialized then subscribe to match and player nodes to keep
    /// observables up-to-date with server state (station, hp, round, countdown, etc.).
    /// </summary>
    private async Task FirebaseSetting()
    {
        try
        {
            bool initialized = await FirebaseInitializer.EnsureInitializedAsync();
            if (!initialized)
            {
                _firebaseSubscribed = false;
                return;
            }


            // matches/{lobbyId} subscribe
            await FirebaseClient.Instance.SubscribeAsync<MatchInfoModel>(
                $"matches/{_lobbyId}",
                onValueChanged: (match) =>
                {
                    if (match == null) return;
                    StationName.Value = match.station;
                    if (Enum.TryParse(match.state, true, out LobbyState result))
                    {
                        MatchState.Value = result;
                    }

                    Debug.Log("current state = " + result.ToString());
                    ChangePlayByState(match);
                    GetStatusText();
                    CurrentRound.Value = match.currentRound;
                    WinnerPlayerIdx.Value = match.winnerPlayerIdx;
                    CurrentTurn.Value = match.currentTurn;
                    //lobby data changing mean timer start again.
                },
                onError: (error) => Debug.LogError(error)
            );

            // my player subscribe -> left
            await FirebaseClient.Instance.SubscribeAsync<PlayerInfoModel>(
                $"matches/{_lobbyId}/players/{_playerId}",
                onValueChanged: (player) =>
                {
                    if (player == null) return;
                    LeftHp.Value = player.hp;
                    IsAttacker.Value = player.attacking;
                    MySelecting.Value = player.selecting;
                    MySelectingE.Value = player.selecting;
                    MyName.Value = player.username;
                    LeftRoundWin.Value = player.wins;
                    player1 = player;
                    Debug.Log(player.hp + " " + player.username + player.hp + "Player(ME)");
                },
                onError: (error) => Debug.LogError(error)
            );

            // enemy player subscribe -> right
            await FirebaseClient.Instance.SubscribeAsync<PlayerInfoModel>(
                $"matches/{_lobbyId}/players/{_enemyId}",
                onValueChanged: (player) =>
                {
                    if (player == null) return;
                    RightHp.Value = player.hp;
                    EnemySelecting.Value = player.selecting;
                    EnemyName.Value = player.username;
                    RightRoundWin.Value = player.wins;
                    player2 = player;
                    Debug.Log(player.hp + " " + player.username + player.hp + "Enemy");
                },
                onError: (error) => Debug.LogError(error)
            );
        }
        catch (Exception e)
        {
            _firebaseSubscribed = false;
            Debug.LogException(e);
        }
    }

    private async Task ChangePlayByState(MatchInfoModel match)
    {
        Func<Task> action = MatchState.Value switch
        {
            LobbyState.GAME_PLAYER_CHOICE => () =>
            {
                StartTimer(match.countdownStartTime, match.countdownSec);
                return Task.CompletedTask;
            },

            LobbyState.GAME_CHOICE_FINISHED => async () =>
            {
                await Task.Delay(GameSetting.DELAY_MAP[SceneDataBridge.playerCamera]);
                await _repository.PutChoice(_playerId, CurrentHandAction.Value.ToString());
            },

            LobbyState.GAME_TURN_ANIMATION => async () =>
            {
                EventBus.Publish(new ChoiceAnimation(player1, player2));
                if (player1 == null || player2 == null)
                {
                    Debug.LogWarning("[MainBattleViewModel] ChoiceAnimation: player info not ready.");
                }
                await Task.Delay(5000);
                if (player1 != null && player2 != null)
                {
                    EventBus.Publish(new ActionSelectedEvent(player1, player2));
                }
                else
                {
                    Debug.LogWarning("[MainBattleViewModel] ActionSelectedEvent skipped: player info not ready.");
                }
                EventBus.Publish(new CameraAction(CameraType.Action));
                EventBus.Publish(new HitAnimation(
                    IsAttacker.Value ? BattleRole.Attack : BattleRole.Defense,
                    SceneDataBridge.playerCamera == CameraType.Camera1 ? Player.First : Player.Second,
                    HitActionType.Both5,
                    null));
                await Task.Delay(GameSetting.DELAY_MAP[SceneDataBridge.playerCamera] + 6000);
                await _repository.PutAck(_playerId);
            },

            LobbyState.END_RESULT => () =>
            {
                EventBus.Publish(new RoundOver(true));
                return Task.CompletedTask;
            },

            LobbyState.LOBBY_START_COUNTDOWN or LobbyState.GAME_ROUND_START_ANIMATION => () =>
            {
                // C# 9.0+ 부터 'or' 패턴으로 switch 조건 결합 가능 (구버전은 case 2개로 분리)
                EventBus.Publish(new IntroduceStationEvent(station: match.station, player1, player2));
                return Task.CompletedTask;
            },

            LobbyState.GAME_ROUND_END_PLAYER_KO => async () =>
            {
                EventBus.Publish(new RoundOver(true));
                await Task.Delay(GameSetting.DELAY_MAP[SceneDataBridge.playerCamera] + 5000);
                _roundRepository.endAck(SceneDataBridge.playerId);
            },

            LobbyState.GAME_ELEMENTAL_CHOICE => async () =>
            {
                EventBus.Publish(new HandElementalChoice());
                await Task.Delay(GameSetting.DELAY_MAP[SceneDataBridge.playerCamera] + 5000);
                _elementalRepository.PutChoice(_playerId, ElementalHand.FIRE.ToString());
            },

            LobbyState.GAME_ELEMENTAL_RECEIVING => async () =>
            {
                EventBus.Publish(new HandElementalChoiceResult());
                await Task.Delay(GameSetting.DELAY_MAP[SceneDataBridge.playerCamera] + 5000);
                _elementalRepository.PutAck(_playerId);
            },
            LobbyState.GAME_PERK_ITEM_RECEIVING => () =>
            {
                EventBus.Publish(new PerksAndItemReceiveEvent());
                return Task.CompletedTask;
            },

            _ => () => Task.CompletedTask // 매칭되는 상태가 없을 때 기본 동작 (예외 처리 필요 시 throw 가능)
        };

        // 매칭된 비동기/동기 액션 실행
        await action();
    }

    /// <summary>
    /// Runs a high-frequency countdown that updates CountDown observable until end time.
    /// Uses CancellationToken to stop the timer when needed.
    /// Some logics were written with the help of ai.
    /// </summary>
    /// <param name="startTimeStr"></param>
    /// <param name="durationSec"></param>
    private async void StartTimer(string startTimeStr, int durationSec)
    {
        _timerCts?.Cancel();
        _timerCts?.Dispose();
        _timerCts = new CancellationTokenSource();
        var token = _timerCts.Token;

        //setting format and convert to DataTime
        string format = "yyyy-MM-dd'T'HH:mm:ss.fff";
        if (!DateTime.TryParseExact(startTimeStr, format, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out DateTime startTime))
        {
            return;
        }

        DateTime endTime = startTime.AddSeconds(durationSec);

        Debug.Log(startTime.ToString() + "*****************" + durationSec);
        //Show CountDown Value
        try
        {
            while (!token.IsCancellationRequested)
            {
                TimeSpan remaining = endTime - DateTime.Now;
                double totalSeconds = remaining.TotalSeconds;

                if (totalSeconds <= 0)
                {
                    CountDown.Value = "00.00";
                    break;
                }

                int sec = (int)totalSeconds;
                int ms = (int)((totalSeconds - sec) * 100);
                CountDown.Value = string.Format("{0:D2}.{1:D2}", sec, ms);

                await Task.Delay(10, token);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    /// <summary>
    /// Starts Firebase subscriptions if ViewModel initialized and ids are set.
    /// Guards against duplicate subscription setup.
    /// </summary>
    private void TryStartFirebaseSubscriptions()
    {
        if (!IsInitialized || _firebaseSubscribed) return;
        if (string.IsNullOrWhiteSpace(_playerId) ||
            string.IsNullOrWhiteSpace(_lobbyId) ||
            string.IsNullOrWhiteSpace(_enemyId))
            return;

        _firebaseSubscribed = true;
        _ = FirebaseSetting();
    }

    /// <summary>
    /// Compute human-friendly LabelState based on MatchState and selecting flags.
    /// </summary>
    private void GetStatusText()
    {
        // waiting
        if (MatchState.Value == LobbyState.LOBBY_START_COUNTDOWN)
        {
            LabelState.Value = "START SOON..";
        }
        // game over
        else if (MatchState.Value == LobbyState.END_RESULT)
        {
            LabelState.Value = "GAME OVER!";
        }
        // my turn or enemy turn
        else if (MySelecting.Value)
        {
            LabelState.Value = "YOUR TURN";
        }
        else
        {
            LabelState.Value = "ENEMY TURN";
        }
    }


    /// <summary>
    /// Cleanup timers and unsubscribe flags when ViewModel is disposed.
    /// </summary>
    public override void Dispose()
    {
        _timerCts?.Cancel();
        _timerCts?.Dispose();
        _firebaseSubscribed = false;
        EventBus.Unsubscribe<GameRoundStartAnimationAckEvent>(GRSAAckEvent);
        base.Dispose();
    }

    public void SetPlayerAndMatchId(string playerId, string matchId, string enemyId, CameraType playerCamera,
        CameraType enemyCamera)
    {
        _playerId = playerId;
        _lobbyId = matchId;
        _enemyId = enemyId;
        CameraPoint.Value = playerCamera;
        TryStartFirebaseSubscriptions();
    }

    public void OnChangeActionIndex(HandActionType actionIndex, string actionText)
    {
        CurrentHandAction.Value = actionIndex;
        CurrentHandActionText.Value = actionText;
    }


    public async void PutRoundStartAck()
    {
        Debug.Log("put round start ack 보내고 있음!");
        await Task.Delay(GameSetting.DELAY_MAP[SceneDataBridge.playerCamera]);
        await _roundRepository.startAck(SceneDataBridge.playerId);
    }
}