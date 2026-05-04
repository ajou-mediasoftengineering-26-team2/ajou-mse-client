using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine;


//202322158 이준상
public class FirebaseDatabaseClient
{
    private static FirebaseDatabaseClient _instance;
    public static FirebaseDatabaseClient Instance => _instance ??= new FirebaseDatabaseClient();

    private readonly FirebaseDatabase _database;
    private readonly Dictionary<string, (DatabaseReference Ref, EventHandler<ValueChangedEventArgs> Handler)> _subs = new();

    private FirebaseDatabaseClient()
    {
        _database = FirebaseDatabase.DefaultInstance;
    }

    public async Task<T> GetOnceAsync<T>(string path) where T : class, new()
    {
        await FirebaseInitializer.EnsureInitializedAsync();
        DatabaseReference reference = _database.GetReference(path.Trim('/'));
        DataSnapshot snapshot = await reference.GetValueAsync();
        return Deserialize<T>(snapshot);
    }

    public async Task<string> SubscribeAsync<T>(string path, Action<T> onValueChanged, Action<string> onError = null) where T : class, new()
    {
        await FirebaseInitializer.EnsureInitializedAsync();
        DatabaseReference reference = _database.GetReference(path.Trim('/'));
        string id = Guid.NewGuid().ToString("N");

        EventHandler<ValueChangedEventArgs> handler = (sender, args) =>
        {
            if (args.DatabaseError != null)
            {
                onError?.Invoke($"[{args.DatabaseError.Code}] {args.DatabaseError.Message}");
                return;
            }

            try
            {
                T model = Deserialize<T>(args.Snapshot);
                onValueChanged(model);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
            }
        };

        reference.ValueChanged += handler;
        _subs[id] = (reference, handler);
        return id;
    }

    public void Unsubscribe(string id)
    {
        if (_subs.TryGetValue(id, out var item))
        {
            item.Ref.ValueChanged -= item.Handler;
            _subs.Remove(id);
        }
    }

    public void UnsubscribeAll()
    {
        foreach (var item in _subs.Values)
        {
            item.Ref.ValueChanged -= item.Handler;
        }
        _subs.Clear();
    }

    private static T Deserialize<T>(DataSnapshot snapshot) where T : class, new()
    {
        if (snapshot == null || !snapshot.Exists)
            return null;

        string json = snapshot.GetRawJsonValue();
        if (string.IsNullOrWhiteSpace(json) || json == "null")
            return null;

        try
        {
            return JsonUtility.FromJson<T>(json);
        }
        catch (Exception ex)
        {
            NetworkLogger.LogError($"Parse error: {ex.Message}");
            return null;
        }
    }
}
