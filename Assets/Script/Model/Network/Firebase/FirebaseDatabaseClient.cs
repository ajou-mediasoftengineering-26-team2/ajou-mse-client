using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine;


//202322158 이준상
public class FirebaseDatabaseClient
{
    private static FirebaseDatabaseClient _instance;
    // Singleton pattern: Provides global access and ensures only one instance exists.
    public static FirebaseDatabaseClient Instance => _instance ??= new FirebaseDatabaseClient();

    private readonly FirebaseDatabase _database;
    // Manages active subscriptions to prevent memory leaks (ID, Reference, Handler).
    private readonly Dictionary<string, (DatabaseReference Ref, EventHandler<ValueChangedEventArgs> Handler)> _subs = new();

    private FirebaseDatabaseClient()
    {
        // Initializes the default Firebase Database instance.
        _database = FirebaseDatabase.DefaultInstance;
    }

    /// <summary>
    /// Asynchronously retrieves data once from a specific path.
    /// </summary>
    /// <param name="path"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<T> GetOnceAsync<T>(string path) where T : class, new()
    {
        // Ensure initialization before any operation
        await FirebaseInitializer.EnsureInitializedAsync();
        DatabaseReference reference = _database.GetReference(path.Trim('/'));
        DataSnapshot snapshot = await reference.GetValueAsync();
        return Deserialize<T>(snapshot);
    }

    /// <summary>
    /// Asynchronously retrieves data once from a specific path.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="onValueChanged"></param>
    /// <param name="onError"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<string> SubscribeAsync<T>(string path, Action<T> onValueChanged, Action<string> onError = null) where T : class, new()
    {
        await FirebaseInitializer.EnsureInitializedAsync();
        DatabaseReference reference = _database.GetReference(path.Trim('/'));
        // Generate a unique ID to identify this specific subscription
        string id = Guid.NewGuid().ToString("N");

        // Define the handler to execute when data updates on the server
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

        // Attach event and store it in the dictionary for later removal
        reference.ValueChanged += handler;
        _subs[id] = (reference, handler);
        return id;
    }

    /// <summary>
    /// Asynchronously retrieves data once from a specific path.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="onKeysChanged"></param>
    /// <param name="onError"></param>
    /// <returns></returns>
    public async Task<string> SubscribeChildKeysAsync(string path, Action<List<string>> onKeysChanged, Action<string> onError = null)
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
                var keys = new List<string>();
                if (args.Snapshot != null && args.Snapshot.Exists)
                {
                    foreach (DataSnapshot child in args.Snapshot.Children)
                    {
                        keys.Add(child.Key);
                    }
                }
                onKeysChanged?.Invoke(keys);
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

    /// <summary>
    /// Asynchronously retrieves data once from a specific path.
    /// </summary>
    /// <param name="id"></param>
    public void Unsubscribe(string id)
    {
        if (_subs.TryGetValue(id, out var item))
        {
            // Detach the handler from the reference to stop receiving updates
            item.Ref.ValueChanged -= item.Handler;
            _subs.Remove(id);
        }
    }

    /// <summary>
    /// Asynchronously retrieves data once from a specific path.
    /// </summary>
    public void UnsubscribeAll()
    {
        foreach (var item in _subs.Values)
        {
            item.Ref.ValueChanged -= item.Handler;
        }
        _subs.Clear();
    }

    /// <summary>
    /// Converts the DataSnapshot to JSON and deserializes it back into a C# object.
    /// </summary>
    /// <param name="snapshot"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
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
            NetworkLogger.LogError($"Parsing failed: {ex.Message}");
            return null;
        }
    }
}