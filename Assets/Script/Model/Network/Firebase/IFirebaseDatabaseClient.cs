using System;
using System.Threading.Tasks;


//202322158 이준상
public class FirebaseClient
{
    private static FirebaseClient _instance;
    public static FirebaseClient Instance => _instance ??= new FirebaseClient();

    public async Task<T> GetAsync<T>(string path) where T : class, new()
    {
        return await FirebaseDatabaseClient.Instance.GetOnceAsync<T>(path);
    }

    public async Task<string> SubscribeAsync<T>(string path, Action<T> onValueChanged, Action<string> onError = null) where T : class, new()
    {
        return await FirebaseDatabaseClient.Instance.SubscribeAsync(path, onValueChanged, onError);
    }

    public void Unsubscribe(string subscriptionId)
    {
        FirebaseDatabaseClient.Instance.Unsubscribe(subscriptionId);
    }

    public void UnsubscribeAll()
    {
        FirebaseDatabaseClient.Instance.UnsubscribeAll();
    }
}
