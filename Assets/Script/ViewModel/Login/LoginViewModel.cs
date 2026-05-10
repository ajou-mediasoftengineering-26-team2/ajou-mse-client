using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
//202322158 이준상


/// <summary>
/// ViewModel that handles player login and matchmaking.
/// Submits player ID to server, exposes observables for UI binding, and manages Firebase
/// subscriptions for station, match info, and players. When two players are ready and match state
/// indicates start, prepares SceneDataBridge and signals match start.
/// </summary>
public class LoginViewModel : ViewModelBase
{
    // Private state and Firebase subscription ids
    private readonly ILoginRepository _repository;
    private string _stationSubscriptionId;
    private string _matchSubscriptionId;
    private string _playersSubscriptionId;
    private string _subscribedLobbyId;
    private LobbyState _matchState = LobbyState.LOBBY_WAITING;
    
    // Public observables exposed to Views for data-binding
    public Observable<string> PlayerId { get; } = new Observable<string>();
    public Observable<string> LobbyId { get; } = new Observable<string>();
    
    public Observable<string> EnemyId { get; } =  new Observable<string>();
    public Observable<bool> IsSuccess { get; } = new Observable<bool>();
    public Observable<string> ErrorMsg { get; } =  new Observable<string>();
    public Observable<int> ErrorCode { get; } =  new Observable<int>();
    
    public Observable<string> SubwayStation { get; } =  new Observable<string>();
    public Observable<bool> IsMatchStarted { get; } = new Observable<bool>();

    public LoginViewModel()
    {
        _repository = RepositoryFactory.Instance.Get<ILoginRepository>();
    }

    /// <summary>
    /// Initialize this ViewModel and subscribe to station updates.
    /// Subscriptions are started only once via IsInitialized guard.
    /// </summary>
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

    /// <summary>
    /// Sends a login/join request to server with the provided playerName.
    /// On success updates PlayerId/LobbyId and ensures subscriptions to the match are active.
    /// Errors are reported via ErrorMsg/ErrorCode observables.
    /// </summary>
    /// <param name="playerName"></param>
    public async void OnSubmitID(string playerName)
    {

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
                //if submit id success, firebase setting
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

    /// <summary>
    /// Unsubscribe from any active Firebase subscriptions and clear local state.
    /// Ensures resources are released and base.Dispose() is called.
    /// </summary>
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

    /// <summary>
    /// Ensure subscriptions to the match node and its players are active for the given lobbyId.
    /// If already subscribed to the same lobby, this is a match making! Updates EnemyId and match state.
    /// </summary>
    /// <param name="lobbyId"></param>
    private async Task EnsureMatchSubscriptionAsync(string lobbyId)
    {
        //Null or Empty Check. AI give me advice for this.
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

        //Default Value
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

                //Get enemy id. Because we will be using at MainBattleScene. 
                foreach (string candidate in playerIds)
                {
                    if (candidate == PlayerId.Value) continue;
                    EnemyId.Value = candidate;
                    //Debug.Log(candidate + "EnemyID");
                    break;
                }

                TryMoveToBattleIfReady();
            },
            onError: error => Debug.LogError(error)
        );
    }

    /// <summary>
    /// When both players are present and match state indicates the game should start,
    /// populate SceneDataBridge with match/player IDs and mark IsMatchStarted to prevent reruns.
    /// </summary>
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
