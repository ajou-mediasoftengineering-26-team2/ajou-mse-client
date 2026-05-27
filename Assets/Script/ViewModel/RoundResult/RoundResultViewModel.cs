using System;
using System.Threading.Tasks;
using UnityEngine;

// 202422170 주형준
public class RoundResultViewModel : ViewModelBase
{
    private string _playerId;
    private string _lobbyId;
    private string _matchSubId;
    private string _playerSubId;

    private int  _myWins          = 0;
    private int  _prevMyWins      = 0;
    private bool _wasInResultState = false;

    public Observable<bool>   IsVisible    { get; } = new Observable<bool>();
    public Observable<string> CurrentRound { get; } = new Observable<string>();
    public Observable<bool>   IsWin        { get; } = new Observable<bool>();
    public Observable<int>    GetMoney     { get; } = new Observable<int>();

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
                if (!Enum.TryParse<LobbyState>(match.state, true, out var state)) return;

                bool isResultState = state == LobbyState.GAME_ROUND_END_PLAYER_KO;

                if (isResultState && !_wasInResultState)
                {
                    CurrentRound.Value = $"Round {match.currentRound}";
                    IsWin.Value        = _myWins > _prevMyWins;
                    IsVisible.Value    = true;
                }
                else if (!isResultState && _wasInResultState)
                {
                    _prevMyWins     = _myWins;
                    IsVisible.Value = false;
                }

                _wasInResultState = isResultState;
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
                _myWins = player.wins;

                // timing issue prepare
                if (_wasInResultState)
                    IsWin.Value = _myWins > _prevMyWins;

                // GetMoney.Value = player.money; // money 추가 후 연결
            },
            onError: err => Debug.LogError(err)
        );
    }

    public override void Dispose()
    {
        if (!string.IsNullOrEmpty(_matchSubId))
            FirebaseClient.Instance.Unsubscribe(_matchSubId);
        if (!string.IsNullOrEmpty(_playerSubId))
            FirebaseClient.Instance.Unsubscribe(_playerSubId);
        base.Dispose();
    }
}