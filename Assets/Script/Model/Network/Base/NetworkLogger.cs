using UnityEngine;

//202322158 이준상

/// <summary>
/// Classes that view network logs
/// </summary>
public static class NetworkLogger
{
    public static bool EnableLogging { get; set; } = true;

    public static void Log(string message)
    {
        if (EnableLogging)
        {
            Debug.Log($"[Network] {message}");
        }
    }

    public static void LogWarning(string message)
    {
        if (EnableLogging)
        {
            Debug.LogWarning($"[Network] {message}");
        }
    }

    public static void LogError(string message)
    {
        if (EnableLogging)
        {
            Debug.LogError($"[Network] {message}");
        }
    }
}
