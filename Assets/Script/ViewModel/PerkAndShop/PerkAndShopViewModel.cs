using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

// 202422170 주형준
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

    private void RefreshUpgradePanel()
    {
        BeforeInfo.Value = _currentLevel == 0
            ? HandInfoProvider.GetDescription(_currentHand)
            : HandUpgradeInfoProvider.GetEffectDescription(_currentHand, _currentLevel);

        if (_currentLevel >= 5)
        {
            AfterInfo.Value        = "MAX LEVEL";
            UpgradeCostLabel.Value = "-";
            CanUpgrade.Value       = false;
            return;
        }

        int nextLevel = _currentLevel == 0 ? 1 : _currentLevel + 1;
        AfterInfo.Value        = HandUpgradeInfoProvider.GetEffectDescription(_currentHand, nextLevel);
        UpgradeCostLabel.Value = _currentUpgradeCost.ToString();
        CanUpgrade.Value       = _displayCoin >= _currentUpgradeCost && !_upgradeInProgress;
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

    private async void StartTimer(string startTimeStr, int durationSec)
    {
        _timerCts?.Dispose();
        _timerCts = new CancellationTokenSource();
        var token = _timerCts.Token;

        string format = "yyyy-MM-dd'T'HH:mm:ss.fff";
        if (!DateTime.TryParseExact(startTimeStr, format,
            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startTime)) return;

        DateTime endTime = startTime.AddSeconds(durationSec);
        try
        {
            while (!token.IsCancellationRequested)
            {
                double remaining = (endTime - DateTime.Now).TotalSeconds;
                if (remaining <= 0)
                {
                    TimerRatio.Value = 0f;
                    await FlushPerkSelectionAsync();
                    break;
                }
                TimerRatio.Value = (float)(remaining / durationSec);
                await Task.Delay(50, token);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception e) { Debug.LogException(e); }
    }

    public void OnSelectPerk(int slot)
    {
        if (!CanSelect.Value) return;
        if (_perkChoices.Count <= slot - 1) return;

        _perkSelected         = true;
        _pendingPerkSelection = _perkChoices[slot - 1];
        CanSelect.Value       = false;
        EventBus.Publish(new PlaySfxEvent(SfxType.ButtonClick));
    }

    private async Task FlushPerkSelectionAsync()
    {
        if (_perkChoices.Count == 0) return;
        if (string.IsNullOrEmpty(_pendingPerkSelection))
            _pendingPerkSelection = _perkChoices[UnityEngine.Random.Range(0, _perkChoices.Count)];
        try
        {
            var res = await _perkAndShopRepo.PutChoice(_playerId, _pendingPerkSelection);
            if (!res.isSuccess)
                ErrorMsg.Value = res.error.message;
        }
        catch (Exception e) { Debug.LogException(e); }
    }

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