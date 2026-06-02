using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

// 202422170 주형준
public class SelectHandsViewModel : ViewModelBase
{
    private readonly ISelectHandsRepository _repo;
    private string _playerId;
    private string _lobbyId;
    private string _matchSubId;
    private CancellationTokenSource _timerCts;

    public Observable<bool>   IsVisible   { get; } = new Observable<bool>(false);
    public Observable<bool>   CanSelect   { get; } = new Observable<bool>(false);
    public Observable<float>  TimerRatio  { get; } = new Observable<float>(1f);
    public Observable<string> ErrorMsg    { get; } = new Observable<string>();

    public SelectHandsViewModel()
    {
        _repo = RepositoryFactory.Instance.Get<ISelectHandsRepository>();
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
                bool isMyPhase = match.state == "GAME_ELEMENTAL_CHOICE";
                IsVisible.Value  = isMyPhase;
                CanSelect.Value  = isMyPhase;

                if (isMyPhase)
                    StartTimer(match.countdownStartTime, match.countdownSec);
                else
                    _timerCts?.Cancel();
            },
            onError: err => Debug.LogError(err)
        );
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

    public async void OnSelectHand(int slot)
    {
        if (!CanSelect.Value) return;
        if (slot < 1 || slot > 6) return;

        CanSelect.Value = false;
        try
        {
            var handType = ((HandElementalType)(slot - 1)).ToString();
            var res = await _repo.PostSelectHand(_playerId, handType);
            if (!res.isSuccess)
            {
                ErrorMsg.Value  = res.error.message;
                CanSelect.Value = true;
                return;
            }
            // PutAck 없음 — MainBattleViewModel이 GAME_ELEMENTAL_RECEIVING에서 처리
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
        base.Dispose();
    }
}