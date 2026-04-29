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

    public IDViewModel()
    {
        RepositoryFactory.Instance.Register<IIDRepository, IDRepository>();
        _repository = RepositoryFactory.Instance.Get<IIDRepository>();
    }

    public async void OnSubmitID(string playerName)
    {
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
                ErrorMsg.Value = response.error.message;
                ErrorCode.Value = response.error.code;
            }
        }
        catch (NetworkException e)
        {
            ErrorMsg.Value = e.ErrorMessage;
            ErrorCode.Value = (int)e.ResponseCode;
            Debug.LogException(e);
        }
        catch (Exception e)
        {
            ErrorMsg.Value = e.Message;
            ErrorCode.Value = -1;
            Debug.LogException(e);
        }
    }
}