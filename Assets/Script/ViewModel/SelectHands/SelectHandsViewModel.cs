using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


// 202422170 주형준

/// <summary>
/// ViewModel for the hand elemental selection phase.
/// Subscribes to the Firebase match state and manages selection timing,
/// countdown display, and deferred server submission.
/// </summary>
public class SelectHandsViewModel : ViewModelBase
{
    private readonly ISelectHandsRepository _repo;
    private string _playerId;
    private string _lobbyId;
    private string _matchSubId;
    private CancellationTokenSource _timerCts;
    private string _selectedHandType; // 추가: 로컬 선택값 저장
    private bool _inChoicePhase = false;
    private bool _selectionSent = false;//추가
    
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

    /// <summary>
    /// Subscribes to the Firebase match state node to detect phase transitions.
    /// On entering GAME_ELEMENTAL_CHOICE, pre-selects a random hand as the default
    /// and starts the countdown timer.
    /// On entering GAME_ELEMENTAL_RECEIVING, sends the final selection to the server.
    /// </summary>
    private async Task SubscribeMatchStateAsync()
    {
        _matchSubId = await FirebaseClient.Instance.SubscribeAsync<MatchInfoModel>(
            $"matches/{_lobbyId}",
            onValueChanged: match =>
            {
                if (match == null) return;

                bool isChoice    = match.state == "GAME_ELEMENTAL_CHOICE";
                bool isReceiving = match.state == "GAME_ELEMENTAL_RECEIVING";

                if (isChoice)
                {
                    IsVisible.Value = true;
                    if (!_inChoicePhase)
                    {
                        _inChoicePhase  = true;
                        _selectionSent  = false;
                        CanSelect.Value = true;

                        // 미리 랜덤 세팅
                        var hands = new[] {
                            HandElementalType.FIRE,   HandElementalType.WATER,
                            HandElementalType.WIND,   HandElementalType.LIGHTNING,
                            HandElementalType.POISON, HandElementalType.PLANT
                        };
                        _selectedHandType = hands[UnityEngine.Random.Range(0, hands.Length)].ToString();

                        StartTimer(match.countdownStartTime, match.countdownSec);
                    }
                }
                else
                {//추가
                    if (isReceiving && !_selectionSent)
                    {
                        _selectionSent = true;
                        _timerCts?.Cancel();
                        _ = SendSelection();
                    }
                    _inChoicePhase = false;
                }
            },
            onError: err => Debug.LogError(err)
        );
    }

    /// <summary>
    /// Sends the locally buffered hand selection to the server.
    /// Adds a random delay (0–1000 ms) to reduce request collisions
    /// when both clients submit simultaneously.
    /// </summary>
    private async Task SendSelection()
    {
        var handType = _selectedHandType;
        _selectedHandType = null;
        try
        {
            await Task.Delay(UnityEngine.Random.Range(0, 1000));
            //await Task.Delay(GameSetting.DELAY_MAP[SceneDataBridge.playerCamera] );
            var res = await _repo.PostSelectHand(_playerId, handType);
            if (!res.isSuccess)
                Debug.LogError($"[SelectHands] PostSelectHand 실패: {res.error.message}");
        }
        catch (Exception e) { Debug.LogException(e); }
    }

    /// <summary>
    /// Runs a countdown loop that updates TimerRatio every 50 ms.
    /// On expiry, hides the panel and sends the selection if not already sent.
    /// A CancellationTokenSource is used to cancel any previous timer before starting a new one.
    /// </summary>
    private async void StartTimer(string startTimeStr, int durationSec)
    {
        _timerCts?.Cancel();
        _timerCts?.Dispose();
        _timerCts = new CancellationTokenSource();
        var token = _timerCts.Token;

        string format = "yyyy-MM-dd'T'HH:mm:ss.fff";
        if (!DateTime.TryParseExact(startTimeStr, format,
            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startTime)) {
            startTime   = DateTime.Now;
            durationSec = durationSec > 0 ? durationSec : 5;
            // return 제거
        }

        DateTime endTime = startTime.AddSeconds(durationSec);

        try
        {
            while (!token.IsCancellationRequested)
            {
                double remaining = (endTime - DateTime.Now).TotalSeconds;
                if (remaining <= 0)
                {
                    TimerRatio.Value = 0f;
                    CanSelect.Value  = false;
                    IsVisible.Value  = false;
                    if (_inChoicePhase && !_selectionSent)
                    {
                        _selectionSent = true;
                        _ = SendSelection();
                    }
                    break;  // FlushHandSelectionAsync 없이 break만
                }
                TimerRatio.Value = (float)(remaining / durationSec);
                await Task.Delay(50, token);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception e) { Debug.LogException(e); }

        TimerRatio.Value = 0f;
        CanSelect.Value  = false;
        IsVisible.Value  = false;
        //await FlushHandSelectionAsync();
    }

    /// <summary>
    /// Records the player's card choice locally (slot 1–6 mapped to HandElementalType enum).
    /// The selection is intentionally not sent immediately; it will be submitted when
    /// the match state transitions to GAME_ELEMENTAL_RECEIVING.
    /// </summary>
    // async 제거, 로컬 저장만
    public void OnSelectHand(int slot)
    {
        if (!CanSelect.Value) return;
        if (slot < 1 || slot > 6) return;

        _selectedHandType = ((HandElementalType)(slot - 1)).ToString();
        //CanSelect.Value   = false;
        EventBus.Publish(new PlaySfxEvent(SfxType.ButtonClick));
    }

    /// <summary>
    /// Cancels the countdown timer and unsubscribes from Firebase on teardown.
    /// </summary>
    public override void Dispose()
    {
        _timerCts?.Cancel();
        _timerCts?.Dispose();
        if (!string.IsNullOrEmpty(_matchSubId))
            FirebaseClient.Instance.Unsubscribe(_matchSubId);
        base.Dispose();
    }
}