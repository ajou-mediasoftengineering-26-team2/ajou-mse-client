using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MainBattleViewModel : ViewModelBase
{
    private readonly IMainBattleRepository _repository;
    private string _playerId;
    private string _lobbyId;
    private string _enemyId;
    private CancellationTokenSource _countdownCts;
    private bool _firebaseSubscribed;

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
    public Observable<string> MatchState      { get; } = new Observable<string>();
    public Observable<int>    CurrentRound    { get; } = new Observable<int>();
    public Observable<int>    WinnerPlayerIdx { get; } = new Observable<int>(-1);
    
    
    //  ── 게임 상태 ───────────────────────────────────────────────────
    public Observable<bool> mySelecting { get; } = new Observable<bool>();
    public Observable<bool> enemySelecting { get; } = new Observable<bool>();

    // ── 아이템 슬롯 활성 여부 ────────────────────────────────────────
    public Observable<bool> Item1Active { get; } = new Observable<bool>();
    public Observable<bool> Item2Active { get; } = new Observable<bool>();
    public Observable<bool> Item3Active { get; } = new Observable<bool>();

    // ── 퍽 슬롯 활성 여부 ───────────────────────────────────────────
    public Observable<bool> Perk1Active { get; } = new Observable<bool>();
    public Observable<bool> Perk2Active { get; } = new Observable<bool>();
    public Observable<bool> Perk3Active { get; } = new Observable<bool>();

    // ── 상태이상 슬롯 활성 여부 ──────────────────────────────────────
    public Observable<bool> Effect1Active { get; } = new Observable<bool>();
    public Observable<bool> Effect2Active { get; } = new Observable<bool>();
    public Observable<bool> Effect3Active { get; } = new Observable<bool>();
    public Observable<bool> Effect4Active { get; } = new Observable<bool>();

    // ── 돈 ──────────────────────────────────────────────────────────
    public Observable<int> Money { get; } = new Observable<int>();
    
    public MainBattleViewModel()
    {
        // _playerId = playerId;
        // _lobbyId = lobbyId;
        
        RepositoryFactory.Instance.Register<IMainBattleRepository, MainBattleRepository>();
        _repository = RepositoryFactory.Instance.Get<IMainBattleRepository>();
    }

    public override void Initialize()
    {
        if (IsInitialized) return;
        base.Initialize();
        TryStartFirebaseSubscriptions();
    }
    public async void OnHandAction(string choice)
    {
        try
        {
            //await _repository.PostHandAction(_playerId, moveType);
            EventBus.Publish(new AttackStartedEvent(isPlayer: true));
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public void changeValue()
    {
        LeftRoundWin.Value = 2;
        Debug.Log(LeftRoundWin.Value + "Teststest");
    }

    public void setPlayerAndMatchId(string playerId, string matchId, string enemyId)
    {
        _playerId = playerId;
        _lobbyId = matchId;
        _enemyId = enemyId;
        TryStartFirebaseSubscriptions();
    }
    
    public override void Dispose()
    {
        _countdownCts?.Cancel();
        _countdownCts?.Dispose();
        _firebaseSubscribed = false;
        base.Dispose();
    }

    private async Task firebaseSetting()
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
                    MatchState.Value      = match.state;
                    CurrentRound.Value    = match.currentRound;
                    WinnerPlayerIdx.Value = match.winnerPlayerIdx;
                    //StartCountdown(match.countdownStartTime, match.countdownSec);
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
                    mySelecting.Value = player.selecting;
                    Debug.Log(player.hp + " " + player.username + "Player(ME)");
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
                    enemySelecting.Value = player.selecting;
                    Debug.Log(player.hp + " " + player.username + player.attacking + "Enemy");
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

    private void TryStartFirebaseSubscriptions()
    {
        if (!IsInitialized || _firebaseSubscribed) return;
        if (string.IsNullOrWhiteSpace(_playerId) ||
            string.IsNullOrWhiteSpace(_lobbyId) ||
            string.IsNullOrWhiteSpace(_enemyId))
            return;

        _firebaseSubscribed = true;
        _ = firebaseSetting();
    }
}
