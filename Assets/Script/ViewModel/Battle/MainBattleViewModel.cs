using System;
using System.Collections;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

//202322158 이준상
public class MainBattleViewModel : ViewModelBase
{
    private readonly IMainBattleRepository _repository;
    private string _playerId;
    private string _lobbyId;
    private string _enemyId;
    private bool _isTimerRunning = false;
    private bool _firebaseSubscribed;
    private CancellationTokenSource _timerCts;

    // ── HP ──────────────────────────────────────────────────────────
    public Observable<int> LeftHp  { get; } = new Observable<int>();
    public Observable<int> RightHp { get; } = new Observable<int>();

    // ── 라운드 승리 마커 ─────────────────────────────────────────────
    
    public Observable<int> LeftRoundWin { get; } = new Observable<int>();
    public Observable<int> RightRoundWin { get; } = new Observable<int>();
    

    // ── 타이머 ──────────────────────────────────────────────────────
    public Observable<int> RemainingSeconds { get; } = new Observable<int>();

    // ── 공격자 여부 ─────────────────────────────────────────────────
    public Observable<bool> IsAttacker { get; } = new Observable<bool>();

    // ── 역 이름 ─────────────────────────────────────────────────────
    public Observable<string> StationName { get; } = new Observable<string>();

    // ── 게임 상태 ───────────────────────────────────────────────────
    public Observable<LobbyState> MatchState      { get; } = new Observable<LobbyState>();
    public Observable<int>    CurrentRound    { get; } = new Observable<int>();
    public Observable<int>    WinnerPlayerIdx { get; } = new Observable<int>(-1);
    
    
    //  ── 게임 상태 ───────────────────────────────────────────────────
    public Observable<bool> MySelecting { get; } = new Observable<bool>();
    public Observable<bool> EnemySelecting { get; } = new Observable<bool>();
    

    // ── 돈 ──────────────────────────────────────────────────────────
    public Observable<int> Money { get; } = new Observable<int>();
    
    // 현재 라벨 상태
    public Observable<string> LabelState { get; } = new Observable<string>();
    
    // current Turn
    public Observable<int> CurrentTurn { get; } = new Observable<int>();
    public Observable<string> CountDown { get; } = new Observable<string>();
    public MainBattleViewModel()
    {
        // _playerId = playerId;
        // _lobbyId = lobbyId;
        _repository = RepositoryFactory.Instance.Get<IMainBattleRepository>();
    }

    public override void Initialize()
    {
        if (IsInitialized) return;
        base.Initialize();
        TryStartFirebaseSubscriptions();
    }
    public async void OnHandAction(HandActionType choice)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_playerId))
            {
                Debug.LogError("PutChoice skipped: playerId is empty.");
                return;
            }

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

    public void SetPlayerAndMatchId(string playerId, string matchId, string enemyId)
    {
        _playerId = playerId;
        _lobbyId = matchId;
        _enemyId = enemyId;
        TryStartFirebaseSubscriptions();
    }
    
    

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

            // matches/{lobbyId} 구독
            await FirebaseClient.Instance.SubscribeAsync<MatchInfoModel>(
                $"matches/{_lobbyId}",
                onValueChanged: (match) =>
                {
                    if (match == null) return;
                    StationName.Value     = match.station;
                    if (Enum.TryParse(match.state, true, out LobbyState result))
                    {
                        MatchState.Value = result;
                    }

                    GetStatusText();
                    CurrentRound.Value    = match.currentRound;
                    WinnerPlayerIdx.Value = match.winnerPlayerIdx;
                    CurrentTurn.Value = match.currentTurn;

                    StartTimer(match.countdownStartTime, match.countdownSec);
                },
                onError: (error) => Debug.LogError(error)
            );

            // 내 플레이어 구독 → Left
            await FirebaseClient.Instance.SubscribeAsync<PlayerInfoModel>(
                $"matches/{_lobbyId}/players/{_playerId}",
                onValueChanged: (player) =>
                {
                    if (player == null) return;

                    LeftHp.Value     = player.hp;
                    IsAttacker.Value = player.attacking;
                    MySelecting.Value = player.selecting;
                    Debug.Log(player.hp + " " + player.username  + player.hp+ "Player(ME)");
                },
                onError: (error) => Debug.LogError(error)
            );

            // 상대방 구독 → Right
            // TODO: 상대방 playerId 확정 후 주석 해제
            await FirebaseClient.Instance.SubscribeAsync<PlayerInfoModel>(
                $"matches/{_lobbyId}/players/{_enemyId}",
                onValueChanged: (player) =>
                {
                    if (player == null) return;
                    RightHp.Value   = player.hp;
                    EnemySelecting.Value = player.selecting;
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

    private async void StartTimer(string startTimeStr, int durationSec)
    {
        _timerCts?.Cancel();
        _timerCts?.Dispose();
        _timerCts = new CancellationTokenSource();
        var token = _timerCts.Token;

        string format = "yyyy-MM-dd'T'HH:mm:ss.fff";
        if (!DateTime.TryParseExact(startTimeStr, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startTime))
        {
            return;
        }

        DateTime endTime = startTime.AddSeconds(durationSec);
    
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
    
    // ViewModel 안에서
    private void GetStatusText()
    {
        Debug.Log(MatchState.Value + " : Match STate");
        // 1순위: 대기 중일 때
        if (MatchState.Value == LobbyState.LOBBY_START_COUNTDOWN)
        {
            LabelState.Value = "START SOON..";
        }
        else if (MatchState.Value == LobbyState.END_RESULT)
        {
            LabelState.Value = "GAME OVER!";
        }
        // 2순위: 내 턴일 때
        else if (MySelecting.Value)
        {
            LabelState.Value = "YOUR TURN";
        }
        // 3순위: 그 외 (적 턴일 때)
        else
        {
            LabelState.Value = "ENEMY TURN";
        }
    }
    
    public override void Dispose()
    {
        _timerCts?.Cancel();
        _timerCts?.Dispose();
        _firebaseSubscribed = false;
        base.Dispose();
    }
    
}
