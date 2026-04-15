using System;
using System.Threading.Tasks;
using UnityEngine;

public class TestViewModel : ViewModelBase
{
    private readonly ITestRepository _repository;

    public Observable<string> FirebaseMessage { get; } = new Observable<string>("");
    public Observable<string> RepoData { get; } = new Observable<string>("");

    public TestViewModel()
    {
        RepositoryFactory.Instance.Register<ITestRepository, TestRepository>();
        _repository = RepositoryFactory.Instance.Get<ITestRepository>();
    }

    //상속 받은 함수들
    public override async void Initialize()
    {
        try 
        {
            bool initialized = await FirebaseInitializer.EnsureInitializedAsync();
            if (initialized)
            {
                await FirebaseClient.Instance.SubscribeAsync<TestModel>(
                    "test/",
                    onValueChanged: (playerData) => FirebaseMessage.Value = playerData.message,
                    onError: (error) => Debug.LogError(error)
                );
            }
            await LoadTestData();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async Task LoadTestData()
    {
        var data = await _repository.getTest();
        RepoData.Value = $"{data.response} {data.time}";
    }
}