using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;


public class NetworkManager : MonoBehaviour
{
    private static NetworkManager _instance;
    public static NetworkManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("NetworkManager");
                _instance = go.AddComponent<NetworkManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public NetworkConfig Config { get; private set; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        Config = new NetworkConfig();
    }


    public async Task<ApiResponse<T>> Get<T>(string endpoint, Dictionary<string, string> queryParams = null)
    {
        string url = BuildUrl(endpoint, queryParams);
        
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            SetupRequest(request);
            
            var operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }
            
            return HandleResponse<T>(request);
        }
    }

    public async Task<ApiResponse<T>> Post<T>(string endpoint, object body)
    {
        string url = BuildUrl(endpoint);
        string jsonBody = JsonUtility.ToJson(body);
        
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            SetupRequest(request);
            request.SetRequestHeader("Content-Type", "application/json");
            
            var operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }
            
            return HandleResponse<T>(request);
        }
    }

    public async Task<ApiResponse<T>> Put<T>(string endpoint, object body)
    {
        string url = BuildUrl(endpoint);
        string jsonBody = JsonUtility.ToJson(body);
        
        using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            SetupRequest(request);
            request.SetRequestHeader("Content-Type", "application/json");
            
            var operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }
            
            return HandleResponse<T>(request);
        }
    }

    
    public async Task<bool> Delete(string endpoint)
    {
        string url = BuildUrl(endpoint);
        
        using (UnityWebRequest request = UnityWebRequest.Delete(url))
        {
            SetupRequest(request);
            
            var operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new NetworkException(request.responseCode, request.error);
            }
            
            return request.responseCode >= 200 && request.responseCode < 300;
        }
    }

    private string BuildUrl(string endpoint, Dictionary<string, string> queryParams = null)
    {
        string url = Config.BaseUrl.TrimEnd('/') + "/" + endpoint.TrimStart('/');
        
        if (queryParams != null && queryParams.Count > 0)
        {
            var queryString = new StringBuilder("?");
            foreach (var param in queryParams)
            {
                queryString.Append($"{Uri.EscapeDataString(param.Key)}={Uri.EscapeDataString(param.Value)}&");
            }
            url += queryString.ToString().TrimEnd('&');
        }
        
        return url;
    }

    private void SetupRequest(UnityWebRequest request)
    {
        request.timeout = Config.TimeoutSeconds;
        
        if (!string.IsNullOrEmpty(Config.AuthToken))
        {
            request.SetRequestHeader("Authorization", $"Bearer {Config.AuthToken}");
        }
        
        foreach (var header in Config.DefaultHeaders)
        {
            request.SetRequestHeader(header.Key, header.Value);
        }
    }

    private ApiResponse<T> HandleResponse<T>(UnityWebRequest request)
    {
        if (request.result != UnityWebRequest.Result.Success)
        {
            NetworkLogger.LogError($"Request failed: {request.error}");
            throw new NetworkException(request.responseCode, request.error);
        }
        
        string responseText = request.downloadHandler.text;
        NetworkLogger.Log($"Response: {responseText}");
        
        try
        {
            return JsonUtility.FromJson<ApiResponse<T>>(responseText);
        }
        catch (Exception ex)
        {
            NetworkLogger.LogError($"JSON parsing failed: {ex.Message}");
            throw new NetworkException(request.responseCode, $"Failed to parse JSON: {ex.Message}");
        }
    }
}
