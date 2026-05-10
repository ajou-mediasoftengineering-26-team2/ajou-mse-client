using System;
using System.Threading.Tasks;


//202322158 이준상
public class FirebaseClient
{
    // Singleton instance to allow global access from anywhere in the game
    private static FirebaseClient _instance;
    public static FirebaseClient Instance => _instance ??= new FirebaseClient();

    /// <summary>
    /// Fetches data once from the specified database path.
    /// It passes the request to the actual database handler.
    /// It's a function that's not being used in the current situation, but it can be used someday.
    /// </summary>
    /// <param name="path"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<T> GetAsync<T>(string path) where T : class, new()
    {
        return await FirebaseDatabaseClient.Instance.GetOnceAsync<T>(path);
    }

    /// <summary>
    /// Subscribes to real-time changes at a specific path.
    /// It triggers 'onValueChanged' whenever the data on the server is updated.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="onValueChanged"></param>
    /// <param name="onError"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<string> SubscribeAsync<T>(string path, Action<T> onValueChanged, Action<string> onError = null) where T : class, new()
    {
        return await FirebaseDatabaseClient.Instance.SubscribeAsync(path, onValueChanged, onError);
    }

    /// <summary>
    /// Tracks only the keys (names) of the child nodes at a given path.
    /// Useful for getting a list of item IDs or usernames without loading full data.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="onKeysChanged"></param>
    /// <param name="onError"></param>
    /// <returns></returns>
    public async Task<string> SubscribeChildKeysAsync(string path, Action<System.Collections.Generic.List<string>> onKeysChanged, Action<string> onError = null)
    {
        return await FirebaseDatabaseClient.Instance.SubscribeChildKeysAsync(path, onKeysChanged, onError);
    }

    /// <summary>
    /// Stops tracking a specific subscription using its unique ID to save network resources.
    /// </summary>
    /// <param name="subscriptionId"></param>
    public void Unsubscribe(string subscriptionId)
    {
        FirebaseDatabaseClient.Instance.Unsubscribe(subscriptionId);
    }

    /// <summary>
    /// Clears all active subscriptions at once, usually called when changing scenes or logging out.
    /// </summary>
    public void UnsubscribeAll()
    {
        FirebaseDatabaseClient.Instance.UnsubscribeAll();
    }
}
