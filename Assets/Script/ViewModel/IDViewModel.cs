using System;
using System.Threading.Tasks;
using UnityEngine;
using Object = System.Object;

public class IDViewModel : ViewModelBase
{
    private readonly IIDRepository _repository;
    
    public Observable<String> UserUID { get; } = new Observable<String>();
    public Observable<bool> IsSuccess { get; } = new Observable<bool>();
    public Observable<String> ErrorMsg { get; } =  new Observable<string>();

    public IDViewModel()
    {
        RepositoryFactory.Instance.Register<IIDRepository, IDRepository>();
        _repository = RepositoryFactory.Instance.Get<IIDRepository>();
    }

    public async void OnSubminID(String userID)
    {
        try
        {
            var response = await _repository.PostUserID(userID);

            if (response.isSuccess)
            {
                UserUID.Value = userID;
                IsSuccess.Value = true;
            }
            else
            {
                ErrorMsg.Value = response.error;
            }
        }
        catch (Exception e)
        {
            ErrorMsg.Value = e.Message;
            Debug.LogException(e);
        }
    }
    
}

