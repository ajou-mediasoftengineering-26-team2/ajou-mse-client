using System;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

public class IDViewModel : ViewModelBase
{
    private readonly IIDRepository _repository;
    
    public Observable<string> PlayerId { get; } = new Observable<string>();
    public Observable<string> LobbyId { get; } = new Observable<string>();
    public Observable<bool> IsSuccess { get; } = new Observable<bool>();
    public Observable<string> ErrorMsg { get; } =  new Observable<string>();
    public Observable<int> ErrorCode { get; } =  new Observable<int>();
    
    public Observable<string> SubwayStation { get; } =  new Observable<string>();

    public IDViewModel()
    {
        RepositoryFactory.Instance.Register<IIDRepository, IDRepository>();
        _repository = RepositoryFactory.Instance.Get<IIDRepository>();
    }

    public override async void Initialize()
    {
        await FirebaseClient.Instance.SubscribeAsync<FBStationModel>(
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
                LobbyId.Value = response.data.lobbyId;
                IsSuccess.Value = true;
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
}
