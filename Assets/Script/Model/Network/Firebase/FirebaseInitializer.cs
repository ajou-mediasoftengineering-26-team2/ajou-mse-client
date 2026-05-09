using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using UnityEngine;

//202322158 이준상
public class FirebaseInitializer : MonoBehaviour
{
    // Caches the initialization task to prevent multiple simultaneous initialization attempts
    private static Task<bool> _initializeTask;
    public static bool IsInitialized { get; private set; }

    [Header("Firebase")]
    [Tooltip("Optional. Leave empty to use the URL in Firebase config files.")]
    [SerializeField] private string databaseUrlOverride;
    [SerializeField] private bool initializeOnAwake = true;

    private async void Awake()
    {
        // Automatically start initialization if the option is enabled in the Inspector
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

    /// <summary>
    /// Static entry point to ensure Firebase is ready before any database operations.
    /// Returns immediately if already initialized.
    /// </summary>
    /// <param name="databaseUrl"></param>
    /// <returns></returns>
    public static Task<bool> EnsureInitializedAsync(string databaseUrl = null)
    {
        if (IsInitialized)
        {
            return Task.FromResult(true);
        }

        // Start the initialization process if it hasn't been requested yet
        if (_initializeTask == null)
        {
            _initializeTask = InitializeInternalAsync(databaseUrl);
        }

        return _initializeTask;
    }

    /// <summary>
    /// Function to initialize Firebase.
    /// Some codes refer to Google official documents.
    /// https://firebase.google.com/docs/unity/setup?hl=ko
    /// </summary>
    /// <param name="databaseUrl"></param>
    /// <returns></returns>
    private static async Task<bool> InitializeInternalAsync(string databaseUrl)
    {
        // 1. Check and fix Firebase dependencies.
        // (When I searched on the internet, this mainly serves to verify the SDK status on PC)
        DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (status != DependencyStatus.Available)
        {
            // If dependencies cannot be resolved, log an error and reset the initialization task.
            NetworkLogger.LogError($"Firebase dependencies are unavailable: {status}");
            _initializeTask = null; // Reset so we can retry later
            return false;
        }

        try
        {
            // 2. Access DefaultInstance to trigger internal initialization
            FirebaseApp.DefaultInstance.ToString();

            // 3. Initialize Database with either a custom URL or the default config
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
            _initializeTask = null; // Reset on failure
            return false;
        }
    }
}