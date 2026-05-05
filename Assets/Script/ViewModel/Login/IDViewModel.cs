using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class IDViewModel : ViewModelBase
{
    private readonly IIDRepository _repository;
    private string _stationSubscriptionId;
    private string _matchSubscriptionId;
    private string _playersSubscriptionId;
    private string _subscribedLobbyId;
    private LobbyState _matchState = LobbyState.LOBBY_WAITING;
    
    public Observable<string> PlayerId { get; } = new Observable<string>();
    public Observable<string> LobbyId { get; } = new Observable<string>();
    
    public Observable<string> EnemyId { get; } =  new Observable<string>();
    public Observable<bool> IsSuccess { get; } = new Observable<bool>();
    public Observable<string> ErrorMsg { get; } =  new Observable<string>();
    public Observable<int> ErrorCode { get; } =  new Observable<int>();
    
    public Observable<string> SubwayStation { get; } =  new Observable<string>();
    public Observable<bool> IsMatchStarted { get; } = new Observable<bool>();

    public IDViewModel()
    {
        RepositoryFactory.Instance.Register<IIDRepository, IDRepository>();
        _repository = RepositoryFactory.Instance.Get<IIDRepository>();
    }

    public override async void Initialize()
    {
        if (IsInitialized) return;
        base.Initialize();

        _stationSubscriptionId = await FirebaseClient.Instance.SubscribeAsync<FBStationModel>(
            "/testGame",
            onValueChanged: (station) =>
            {
                if (station == null) return;
                StationType type = StationConverter.GetType(station.currentStation);
                
                SubwayStation.Value = StationConverter.GetDisplayName(type);;
            },
            onError: (error) => Debug.LogError(error)
        );
    }

    public async void OnSubmitID(string playerName)
    {
        // 같은 에러 메시지가 연속으로 와도 구독자가 다시 반응하도록 제출 시점에 초기화

        try
        {
            var response = await _repository.PostUserID(playerName);

            if (response.isSuccess)
            {
                PlayerId.Value = response.data.playerId;
                LobbyId.Value = response.data.matchId;
                IsSuccess.Value = true;
                IsMatchStarted.Value = false;

                Debug.Log(response.data.matchId + "????????");
                await EnsureMatchSubscriptionAsync(response.data.matchId);
            }
            else
            {
                ErrorMsg.Value = null;
                ErrorMsg.Value = response.error.message;
                ErrorCode.Value = response.error.code;
            }
        }
        catch (NetworkException e)
        {
            ErrorMsg.Value = null;
            ErrorMsg.Value = e.ErrorMessage;
            ErrorCode.Value = e.ApiErrorCode ?? (int)e.ResponseCode;
            Debug.LogException(e);
        }
        catch (Exception e)
        {
            ErrorMsg.Value = null;
            ErrorMsg.Value = e.Message;
            ErrorCode.Value = -1;
            Debug.LogException(e);
        }
    }

    public override void Dispose()
    {
        if (!string.IsNullOrEmpty(_stationSubscriptionId))
        {
            FirebaseClient.Instance.Unsubscribe(_stationSubscriptionId);
            _stationSubscriptionId = null;
        }

        if (!string.IsNullOrEmpty(_matchSubscriptionId))
        {
            FirebaseClient.Instance.Unsubscribe(_matchSubscriptionId);
            _matchSubscriptionId = null;
        }

        if (!string.IsNullOrEmpty(_playersSubscriptionId))
        {
            FirebaseClient.Instance.Unsubscribe(_playersSubscriptionId);
            _playersSubscriptionId = null;
        }

        _subscribedLobbyId = null;
        _matchState = LobbyState.LOBBY_WAITING;
        base.Dispose();
    }

    private async Task EnsureMatchSubscriptionAsync(string lobbyId)
    {
        if (string.IsNullOrWhiteSpace(lobbyId)) return;

        if (_subscribedLobbyId == lobbyId &&
            !string.IsNullOrEmpty(_matchSubscriptionId) &&
            !string.IsNullOrEmpty(_playersSubscriptionId))
            return;

        if (!string.IsNullOrEmpty(_matchSubscriptionId))
        {
            FirebaseClient.Instance.Unsubscribe(_matchSubscriptionId);
            _matchSubscriptionId = null;
        }

        if (!string.IsNullOrEmpty(_playersSubscriptionId))
        {
            FirebaseClient.Instance.Unsubscribe(_playersSubscriptionId);
            _playersSubscriptionId = null;
        }

        _subscribedLobbyId = lobbyId;
        _matchState = LobbyState.LOBBY_WAITING;
        EnemyId.Value = null;
        
        _matchSubscriptionId = await FirebaseClient.Instance.SubscribeAsync<MatchInfoModel>(
            $"matches/{lobbyId}",
            onValueChanged: match =>
            {
                if (match == null) return;

                if (Enum.TryParse(match.state, true, out LobbyState result))
                {
                    _matchState = result;
                }
                TryMoveToBattleIfReady();
            },
            onError: error => Debug.LogError(error)
        );

        _playersSubscriptionId = await FirebaseClient.Instance.SubscribeChildKeysAsync(
            $"matches/{lobbyId}/players",
            onKeysChanged: playerIds =>
            {
                if (playerIds == null || playerIds.Count == 0) return;

                foreach (string candidate in playerIds)
                {
                    if (candidate == PlayerId.Value) continue;
                    EnemyId.Value = candidate;
                    Debug.Log(candidate + "EnemyID");
                    break;
                }

                TryMoveToBattleIfReady();
            },
            onError: error => Debug.LogError(error)
        );
    }

    private void TryMoveToBattleIfReady()
    {
        if (string.IsNullOrWhiteSpace(EnemyId.Value)) return;
        if (_matchState == null || _matchState == LobbyState.LOBBY_WAITING) return;
        if (IsMatchStarted.Value) return;

        SceneDataBridge.MatchId = LobbyId.Value;
        SceneDataBridge.playerId = PlayerId.Value;
        SceneDataBridge.enemyId = EnemyId.Value;
        IsMatchStarted.Value = true;
    }
}
