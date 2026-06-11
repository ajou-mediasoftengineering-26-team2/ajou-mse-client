using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

// 202422170 주형준
/// <summary>
/// ViewModel for the perk selection and hand elemental upgrade phase.
/// Subscribes to Firebase match and player nodes to manage perk card data,
/// countdown timer, upgrade state, and deferred server submission.
/// </summary>
public class PerkAndShopViewModel : ViewModelBase
{
    private readonly IPerkAndShopRepository _perkAndShopRepo;
    private readonly IElementalRepository   _elementalRepo;

    private string _playerId;
    private string _lobbyId;

    private string       _matchSubId;
    private string       _playerSubId;
    private List<string> _perkChoices = new List<string>();
    private CancellationTokenSource _timerCts;

    private bool   _inPerkPhase          = false;
    private bool   _perkSelected         = false;
    private string _pendingPerkSelection  = null;

    private bool              _upgradeInProgress  = false;
    private HandElementalType _currentHand        = HandElementalType.FIRE;
    private int               _displayCoin        = 0;
    private int               _currentLevel       = 0;
    private int               _currentUpgradeCost = 0;

    public Observable<bool>   IsVisible        { get; } = new Observable<bool>();
    public Observable<float>  TimerRatio       { get; } = new Observable<float>(1f);
    public Observable<string> Perk1Title       { get; } = new Observable<string>();
    public Observable<string> Perk1Desc        { get; } = new Observable<string>();
    public Observable<string> Perk1Raw         { get; } = new Observable<string>();
    public Observable<string> Perk2Title       { get; } = new Observable<string>();
    public Observable<string> Perk2Desc        { get; } = new Observable<string>();
    public Observable<string> Perk2Raw         { get; } = new Observable<string>();
    public Observable<string> Perk3Title       { get; } = new Observable<string>();
    public Observable<string> Perk3Desc        { get; } = new Observable<string>();
    public Observable<string> Perk3Raw         { get; } = new Observable<string>();
    public Observable<bool>   CanSelect        { get; } = new Observable<bool>(false);
    public Observable<string> ErrorMsg         { get; } = new Observable<string>();
    public Observable<string> HandElementalName { get; } = new Observable<string>();
    public Observable<string> BeforeInfo       { get; } = new Observable<string>();
    public Observable<string> AfterInfo        { get; } = new Observable<string>();
    public Observable<string> UpgradeCostLabel { get; } = new Observable<string>();
    public Observable<bool>   CanUpgrade       { get; } = new Observable<bool>(false);
    public Observable<string> CoinLabel        { get; } = new Observable<string>("0");
    public Observable<string> RoundLabel       { get; } = new Observable<string>("Round: 1");

    public PerkAndShopViewModel()
    {
        _perkAndShopRepo = RepositoryFactory.Instance.Get<IPerkAndShopRepository>();
        _elementalRepo   = RepositoryFactory.Instance.Get<IElementalRepository>();
    }

    public void SetPlayerInfo(string playerId, string lobbyId)
    {
        _playerId = playerId;
        _lobbyId  = lobbyId;
    }

    public override async void Initialize()
    {
        base.Initialize();
        try
        {
            await FirebaseInitializer.EnsureInitializedAsync();
            await SubscribeMatchStateAsync();
            await SubscribePlayerInfoAsync();
        }
        catch (Exception e) { Debug.LogException(e); }
    }

    private int _lastPerkRound = -1;
    
    /// <summary>
    /// Subscribes to the Firebase match state to detect phase transitions.
    /// Uses _lastPerkRound to distinguish a genuinely new GAME_PERK_CHOICE phase
    /// from repeated Firebase callbacks within the same round, preventing redundant resets.
    /// Triggers server submission when state transitions to GAME_PERK_ITEM_RECEIVING.
    /// </summary>

    private async Task SubscribeMatchStateAsync()
    {
        _matchSubId = await FirebaseClient.Instance.SubscribeAsync<MatchInfoModel>(
            $"matches/{_lobbyId}",
            onValueChanged: match =>
            {
                if (match == null) return;
                Debug.Log($"[PerkVM] state={match.state} round={match.currentRound} inPerk={_inPerkPhase} selected={_perkSelected}");
                
                bool isPerkPhase = match.state == "GAME_PERK_CHOICE";
                IsVisible.Value  = isPerkPhase;
                RoundLabel.Value = $"Round: {match.currentRound}";

                if (isPerkPhase)
                {
                    bool isNewRound = match.currentRound != _lastPerkRound;
                    if (isNewRound)
                    {
                        // 진짜 새 라운드일 때만 초기화
                        _lastPerkRound        = match.currentRound;
                        _inPerkPhase          = false;
                        _perkSelected         = false;
                        _pendingPerkSelection = null;
                        _timerCts?.Cancel();
                    }
                    if (!_inPerkPhase)
                    {
                        _inPerkPhase = true;
                        StartTimer(match.countdownStartTime, match.countdownSec);
                    }
                    if (_perkChoices.Count > 0 && !CanSelect.Value && !_perkSelected)
                        CanSelect.Value = true;
                }
                else if(match.state == "GAME_PERK_ITEM_RECEIVING")
                {
                    PutPerk();
                }
                else
                {
                    // 상태가 잠깐 바뀌는 거일 수 있으니 _inPerkPhase/_perkSelected 건드리지 않음
                    CanSelect.Value  = false;
                    CanUpgrade.Value = false;
                }
            },
            onError: err => Debug.LogError(err)
        );
    }

    /// Sends the buffered perk selection to the server.
    /// If no explicit selection was made, falls back to a random choice from the available options.
    private async Task PutPerk()
    {
        if (string.IsNullOrEmpty(_pendingPerkSelection))
        {
            _pendingPerkSelection = _perkChoices[UnityEngine.Random.Range(0, _perkChoices.Count)];
        }
        
        var res = await _perkAndShopRepo.PutChoice(_playerId, _pendingPerkSelection);
        if (!res.isSuccess)
            ErrorMsg.Value = res.error.message;
    }

    /// <summary>
    /// Subscribes to the player node to receive the server-assigned perk choices
    /// and to keep coin and elemental level information up to date.
    /// Skips coin/level updates while an upgrade is in progress to protect
    /// the optimistic UI update from being overwritten prematurely.
    /// </summary>
    private async Task SubscribePlayerInfoAsync()
    {
        _playerSubId = await FirebaseClient.Instance.SubscribeAsync<PlayerInfoModel>(
            $"matches/{_lobbyId}/players/{_playerId}",
            onValueChanged: player =>
            {
                if (player == null) return;

                if (player.perkChoiceList != null && player.perkChoiceList.Count > 0)
                {
                    _perkChoices = player.perkChoiceList;
                    RefreshPerkCards();
                    if (_inPerkPhase && !_perkSelected)
                        CanSelect.Value = true;
                }

                if (_upgradeInProgress && player.elementalLevel > _currentLevel)
                    _upgradeInProgress = false;

                if (!_upgradeInProgress)
                {
                    _displayCoin    = player.coin;
                    CoinLabel.Value = _displayCoin.ToString();
                }

                if (!string.IsNullOrEmpty(player.handElemental) &&
                    player.handElemental != "NONE" &&
                    Enum.TryParse<HandElementalType>(player.handElemental, out var hand))
                {
                    _currentHand            = hand;
                    _currentLevel           = player.elementalLevel;
                    _currentUpgradeCost     = player.upgradeCost;
                    HandElementalName.Value = hand.ToString();
                    RefreshUpgradePanel();
                }
            },
            onError: err => Debug.LogError(err)
        );
    }
    
    /// <summary>
    /// Refreshes the upgrade panel labels based on the current elemental level.
    /// Server level is 0-indexed; display level is 1-indexed (server 0 = display Lv.1).
    /// Disables the upgrade button at maximum level (server level 4 = display Lv.5).
    /// </summary>
    private void RefreshUpgradePanel()
    {
        int displayCurrent = _currentLevel + 1;  // 서버 0 → 표시 Lv.1

        if (_currentLevel >= 4)  // 서버 4 = 표시 Lv.5 = MAX
        {
            BeforeInfo.Value       = $"[Lv.5]\n{HandUpgradeInfoProvider.GetEffectDescription(_currentHand, 5)}";
            AfterInfo.Value        = "MAX LEVEL";
            UpgradeCostLabel.Value = "-";
            CanUpgrade.Value       = false;
            return;
        }

        int nextServerLevel  = _currentLevel + 1;
        int displayNext      = _currentLevel + 2;
        int cost             = HandUpgradeInfoProvider.GetUpgradeCost(nextServerLevel);

        BeforeInfo.Value       = $"{HandUpgradeInfoProvider.GetEffectDescription(_currentHand, displayCurrent)}";
        AfterInfo.Value        = $"{HandUpgradeInfoProvider.GetEffectDescription(_currentHand, displayNext)}";
        UpgradeCostLabel.Value = $"{cost}";
        CanUpgrade.Value       = _displayCoin >= cost && !_upgradeInProgress;
    }

    
    
    private void RefreshPerkCards()
    {
        SetPerkCard(_perkChoices, 1, Perk1Title, Perk1Desc, Perk1Raw);
        SetPerkCard(_perkChoices, 2, Perk2Title, Perk2Desc, Perk2Raw);
        SetPerkCard(_perkChoices, 3, Perk3Title, Perk3Desc, Perk3Raw);
    }

    private void SetPerkCard(List<string> choices, int slot,
        Observable<string> title, Observable<string> desc, Observable<string> raw)
    {
        int index = slot - 1;
        if (index >= choices.Count) return;
        if (!Enum.TryParse<PerkType>(choices[index], out var perkType)) return;
        title.Value = PerkInfoProvider.GetDisplayName(perkType);
        desc.Value  = PerkInfoProvider.GetDescription(perkType);
        raw.Value   = choices[index];
    }

    // 1. async Task로 변경하여 흐름 제어 가능하도록 수정
    private async Task StartTimer(string startTimeStr, int durationSec)
    {
        _timerCts?.Cancel(); // Dispose 보다는 Cancel을 명확히 호출
        _timerCts?.Dispose();
        _timerCts = new CancellationTokenSource();
        var token = _timerCts.Token;

        Debug.Log("startTimeStr + durationSec: " + durationSec + startTimeStr);
        string format = "yyyy-MM-dd'T'HH:mm:ss.fff";
        if (!DateTime.TryParseExact(startTimeStr, format,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startTime)) return;

        // DateTime serverNow = DateTime.Now + NetworkManager.ServerTimeOffset;
    
        DateTime endTime = startTime.AddSeconds(durationSec);

        try
        {
            while (!token.IsCancellationRequested)
            {
                // DateTime.UtcNow를 사용하는 것이 전 세계 시간대(Timezone) 문제 예방에 좋습니다.
                // 여기서는 기존 코드 흐름에 맞추되, 실시간 서버 시간 동기화를 권장합니다.
                double remaining = (endTime - DateTime.Now).TotalSeconds;

                if (remaining <= 0)
                {
                    // 유니티 메인 스레드에서 UI를 안전하게 바꾸기 위해 구조 점검 필요
                    TimerRatio.Value = 0f;
                    break;
                }

                TimerRatio.Value = (float)(remaining / durationSec);

                // 유니티 환경에서는 Task.Delay 대신 UniTask를 쓰거나, 
                // 50ms 대기 대신 매 프레임 체크하는 것이 부드럽고 안전합니다.
                await Task.Delay(50, token); 
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception e) { Debug.LogException(e); }
    }
    
    /// <summary>
    /// Stores the player's perk selection locally.
    /// The selection is not sent immediately; it is submitted when the match state
    /// transitions to GAME_PERK_ITEM_RECEIVING.
    /// </summary>

    public void OnSelectPerk(int slot)
    {
        if (!CanSelect.Value) return;
        if (_perkChoices.Count <= slot - 1) return;

        //_perkSelected         = true;
        _pendingPerkSelection = _perkChoices[slot - 1];
        //CanSelect.Value       = false;
        EventBus.Publish(new PlaySfxEvent(SfxType.ButtonClick));
    }

    
    
    /// <summary>
    /// Initiates a hand elemental upgrade using an optimistic update pattern:
    /// deducts the cost from the displayed coin count immediately, then sends the
    /// server request. If the request fails, the deducted amount is refunded and
    /// the panel is refreshed to reflect the restored state.
    /// </summary>
    public async void OnUpgrade()
    {
        if (!CanUpgrade.Value || _currentLevel >= 5) return;

        _upgradeInProgress = true;
        CanUpgrade.Value   = false;
        _displayCoin      -= _currentUpgradeCost;
        CoinLabel.Value    = _displayCoin.ToString();

        try
        {
            var res = await _elementalRepo.PutUpgrade(_playerId, _currentHand.ToString());
            if (!res.isSuccess)
            {
                _displayCoin      += _currentUpgradeCost;
                CoinLabel.Value    = _displayCoin.ToString();
                ErrorMsg.Value     = res.error?.message ?? "Upgrade failed";
                _upgradeInProgress = false;
                RefreshUpgradePanel();
            }
        }
        catch (Exception e)
        {
            _displayCoin      += _currentUpgradeCost;
            CoinLabel.Value    = _displayCoin.ToString();
            _upgradeInProgress = false;
            RefreshUpgradePanel();
            Debug.LogException(e);
        }
    }

    public override void Dispose()
    {
        _timerCts?.Dispose();
        if (!string.IsNullOrEmpty(_matchSubId))
            FirebaseClient.Instance.Unsubscribe(_matchSubId);
        if (!string.IsNullOrEmpty(_playerSubId))
            FirebaseClient.Instance.Unsubscribe(_playerSubId);
        base.Dispose();
    }
}