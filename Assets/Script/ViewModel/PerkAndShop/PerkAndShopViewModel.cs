using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

//202422170 주형준
public class PerkAndShopViewModel : ViewModelBase
{
    private readonly IPerkAndShopRepository _perkAndShopRepo;
    private string _playerId;
    private string _lobbyId;

    private string _matchSubId;
    private string _playerSubId;
    private List<string> _perkChoices = new List<string>();
    private CancellationTokenSource _timerCts;

    public Observable<bool>   IsVisible   { get; } = new Observable<bool>();
    public Observable<float>  TimerRatio  { get; } = new Observable<float>(1f);
    public Observable<string> Perk1Title  { get; } = new Observable<string>();
    public Observable<string> Perk1Desc   { get; } = new Observable<string>();
    public Observable<string> Perk1Raw    { get; } = new Observable<string>();
    public Observable<string> Perk2Title  { get; } = new Observable<string>();
    public Observable<string> Perk2Desc   { get; } = new Observable<string>();
    public Observable<string> Perk2Raw    { get; } = new Observable<string>();
    public Observable<string> Perk3Title  { get; } = new Observable<string>();
    public Observable<string> Perk3Desc   { get; } = new Observable<string>();
    public Observable<string> Perk3Raw    { get; } = new Observable<string>();
    public Observable<bool>   CanSelect   { get; } = new Observable<bool>(false);
    public Observable<string> ErrorMsg    { get; } = new Observable<string>();

    public PerkAndShopViewModel()
    {
        _perkAndShopRepo = RepositoryFactory.Instance.Get<IPerkAndShopRepository>();
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
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async Task SubscribeMatchStateAsync()
    {
        _matchSubId = await FirebaseClient.Instance.SubscribeAsync<MatchInfoModel>(
            $"matches/{_lobbyId}",
            onValueChanged: match =>
            {
                if (match == null) return;
                bool isPerkPhase = match.state == "GAME_PERK_CHOICE";
                IsVisible.Value = isPerkPhase;
                CanSelect.Value = isPerkPhase && _perkChoices.Count > 0;

                if (isPerkPhase)
                    StartTimer(match.countdownStartTime, match.countdownSec);
                else
                    _timerCts?.Cancel();
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
                if (player?.perkChoiceList == null || player.perkChoiceList.Count == 0) return;
                _perkChoices = player.perkChoiceList;
                RefreshPerkCards();
                CanSelect.Value = true;
            },
            onError: err => Debug.LogError(err)
        );
    }

    private void RefreshPerkCards()
    {
        SetPerkCard(_perkChoices, 1, Perk1Title, Perk1Desc, Perk1Raw);
        SetPerkCard(_perkChoices, 2, Perk2Title, Perk2Desc, Perk2Raw);
        SetPerkCard(_perkChoices, 3, Perk3Title, Perk3Desc, Perk3Raw);
    }

    private void SetPerkCard(List<string> choices, int index,
        Observable<string> title, Observable<string> desc, Observable<string> raw)
    {
        if (choices.Count <= index) return;
        if (!Enum.TryParse<PerkType>(choices[index], out var perkType)) return;

        title.Value = PerkInfoProvider.GetDisplayName(perkType);
        desc.Value  = PerkInfoProvider.GetDescription(perkType);
        raw.Value   = choices[index];
    }

    private async void StartTimer(string startTimeStr, int durationSec)
    {
        _timerCts?.Cancel();
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
                if (remaining <= 0) { TimerRatio.Value = 0f; break; }
                TimerRatio.Value = (float)(remaining / durationSec);
                await Task.Delay(50, token);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception e) { Debug.LogException(e); }
    }

    public async void OnSelectPerk(int slot)
    {
        if (!CanSelect.Value) return;
        if (_perkChoices.Count <= slot) return;

        string selectedPerk = _perkChoices[slot];
        CanSelect.Value = false;

        try
        {
            var res = await _perkAndShopRepo.PutChoice(_playerId, selectedPerk);
            if (!res.isSuccess)
            {
                ErrorMsg.Value  = res.error.message;
                CanSelect.Value = true;
                return;
            }
            _ = _perkAndShopRepo.PutAck(_playerId);
            EventBus.Publish(new PlaySfxEvent(SfxType.ButtonClick));
        }
        catch (Exception e)
        {
            ErrorMsg.Value  = e.Message;
            CanSelect.Value = true;
            Debug.LogException(e);
        }
    }

    public override void Dispose()
    {
        _timerCts?.Cancel();
        _timerCts?.Dispose();
        if (!string.IsNullOrEmpty(_matchSubId))
            FirebaseClient.Instance.Unsubscribe(_matchSubId);
        if (!string.IsNullOrEmpty(_playerSubId))
            FirebaseClient.Instance.Unsubscribe(_playerSubId);
        base.Dispose();
    }
}