using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MainBattleViewModel : ViewModelBase
{
    private readonly IMainBattleRepository _repository;
    private string _playerId;
    private string _lobbyId;
    private CancellationTokenSource _countdownCts;

    // ── HP ──────────────────────────────────────────────────────────
    public Observable<int> LeftHp  { get; } = new Observable<int>();
    public Observable<int> RightHp { get; } = new Observable<int>();

    // ── 라운드 승리 마커 ─────────────────────────────────────────────
    public Observable<bool> LeftWin1  { get; } = new Observable<bool>();
    public Observable<bool> LeftWin2  { get; } = new Observable<bool>();
    public Observable<bool> LeftWin3  { get; } = new Observable<bool>();
    public Observable<bool> RightWin1 { get; } = new Observable<bool>();
    public Observable<bool> RightWin2 { get; } = new Observable<bool>();
    public Observable<bool> RightWin3 { get; } = new Observable<bool>();

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

    //절대로! 커스텀 생성자를 만드시면 안됩니다
    // public MainBattleViewModel(string playerId, string lobbyId)
    // {
    //     // _playerId = playerId;
    //     // _lobbyId = lobbyId;
    //     
    //     RepositoryFactory.Instance.Register<IMainBattleRepository, MainBattleRepository>();
    //     _repository = RepositoryFactory.Instance.Get<IMainBattleRepository>();
    // }
    
    public MainBattleViewModel()
    {
        // _playerId = playerId;
        // _lobbyId = lobbyId;
        
        RepositoryFactory.Instance.Register<IMainBattleRepository, MainBattleRepository>();
        _repository = RepositoryFactory.Instance.Get<IMainBattleRepository>();
    }

    public override async void Initialize()
    {
        base.Initialize();
        try
        {
            bool initialized = await FirebaseInitializer.EnsureInitializedAsync();
            if (!initialized) return;

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
                    StartCountdown(match.countdownStartTime, match.countdownSec);
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

                    LeftWin1.Value = player.wins >= 1;
                    LeftWin2.Value = player.wins >= 2;
                    LeftWin3.Value = player.wins >= 3;

                    // TODO: Firebase에 items/perks/statusEffects/money 추가되면 연결
                    // Item1Active.Value = player.items.Length > 0 && player.items[0] != 0;
                    // Item2Active.Value = player.items.Length > 1 && player.items[1] != 0;
                    // Item3Active.Value = player.items.Length > 2 && player.items[2] != 0;
                    // Perk1Active.Value = player.perks.Length > 0 && player.perks[0] != 0;
                    // Perk2Active.Value = player.perks.Length > 1 && player.perks[1] != 0;
                    // Perk3Active.Value = player.perks.Length > 2 && player.perks[2] != 0;
                    // Effect1Active.Value = player.statusEffects.Length > 0 && player.statusEffects[0] != 0;
                    // Effect2Active.Value = player.statusEffects.Length > 1 && player.statusEffects[1] != 0;
                    // Effect3Active.Value = player.statusEffects.Length > 2 && player.statusEffects[2] != 0;
                    // Effect4Active.Value = player.statusEffects.Length > 3 && player.statusEffects[3] != 0;
                    // Money.Value = player.money;
                },
                onError: (error) => Debug.LogError(error)
            );

            // 상대방 구독 → Right
            // TODO: 상대방 playerId 확정 후 주석 해제
            // await FirebaseClient.Instance.SubscribeAsync<PlayerInfoModel>(
            //     $"matches/{_lobbyId}/players/{opponentId}",
            //     onValueChanged: (player) =>
            //     {
            //         if (player == null) return;
            //         RightHp.Value   = player.hp;
            //         RightWin1.Value = player.wins >= 1;
            //         RightWin2.Value = player.wins >= 2;
            //         RightWin3.Value = player.wins >= 3;
            //     },
            //     onError: (error) => Debug.LogError(error)
            // );
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async void StartCountdown(string startTime, int countdownSec)
    {
        _countdownCts?.Cancel();
        _countdownCts = new CancellationTokenSource();
        var token = _countdownCts.Token;

        DateTime deadline = DateTime.Parse(startTime).AddSeconds(countdownSec);

        try
        {
            while (!token.IsCancellationRequested)
            {
                int remainSec = Math.Max(0, (int)(deadline - DateTime.UtcNow).TotalSeconds);
                RemainingSeconds.Value = remainSec;

                if (remainSec <= 0) break;
                await Task.Delay(1000, token);
            }
        }
        catch (OperationCanceledException) { }
    }

    public async void OnHandAction(string choice)
    {
        try
        {
            await _repository.PostHandAction(_playerId, moveType);
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
    
    public override void Dispose()
    {
        _countdownCts?.Cancel();
        _countdownCts?.Dispose();
        base.Dispose();
    }
}
