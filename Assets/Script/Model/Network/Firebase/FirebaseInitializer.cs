using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using UnityEngine;

//202322158 이준상
public class FirebaseInitializer : MonoBehaviour
{
    private static Task<bool> _initializeTask;
    public static bool IsInitialized { get; private set; }

    [Header("Firebase")]
    [Tooltip("Optional. Leave empty to use the URL in Firebase config files.")]
    [SerializeField] private string databaseUrlOverride;
    [SerializeField] private bool initializeOnAwake = true;

    private async void Awake()
    {
        if (!initializeOnAwake)
        {
            return;
        }

        bool isReady = await EnsureInitializedAsync(databaseUrlOverride);
        if (!isReady)
        {
            NetworkLogger.LogError("Firebase initialization failed.");
        }
    }

    public static Task<bool> EnsureInitializedAsync(string databaseUrl = null)
    {
        if (IsInitialized)
        {
            return Task.FromResult(true);
        }

        if (_initializeTask == null)
        {
            _initializeTask = InitializeInternalAsync(databaseUrl);
        }

        return _initializeTask;
    }

    private static async Task<bool> InitializeInternalAsync(string databaseUrl)
    {
        DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (status != DependencyStatus.Available)
        {
            NetworkLogger.LogError($"Firebase dependencies are unavailable: {status}");
            _initializeTask = null;
            return false;
        }

        try
        {
            FirebaseApp.DefaultInstance.ToString();

            if (string.IsNullOrWhiteSpace(databaseUrl))
            {
                FirebaseDatabase.DefaultInstance.ToString();
            }
            else
            {
                FirebaseDatabase.GetInstance(databaseUrl).ToString();
            }

            IsInitialized = true;
            NetworkLogger.Log("Firebase Realtime Database initialized.");
            return true;
        }
        catch (Exception ex)
        {
            NetworkLogger.LogError($"Firebase initialization error: {ex.Message}");
            _initializeTask = null;
            return false;
        }
    }
}
