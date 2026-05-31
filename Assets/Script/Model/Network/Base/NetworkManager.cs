using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;


//202322158 이준상

/// <summary>
/// It is a network class to be used in other layers.
/// The http method is use in this class.
/// </summary>
public class NetworkManager : MonoBehaviour
{
    /// <summary>
    /// This variable implements a Lazy-Loaded Singleton pattern that automatically creates a "NetworkManager"
    /// GameObject in the scene if one doesn't already exist.
    /// </summary>
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

    /// <summary>
    /// This Awake() removes duplicate-generated instances to maintain single-tone identity,
    /// and sets the object to not be destroyed during scene transition.
    /// </summary>
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


    /// <summary>
    /// Sends an asynchronous GET request, waits for the request to complete, and parses the result into an object of the specified type (T) and returns it.
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="queryParams"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
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

    /// <summary>
    /// Sends an asynchronous Post request, waits for the request to complete, and parses the result into an object of the specified type (T) and returns it.
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="body"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
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

            //setting file content
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            //handle about request
            return HandleResponse<T>(request);
        }
    }

    /// <summary>
    /// Sends an asynchronous Put request, waits for the request to complete, and parses the result into an object of the specified type (T) and returns it.
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="body"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<ApiResponse<T>> Put<T>(string endpoint, object body)
    {
        string url = BuildUrl(endpoint);
        string jsonBody = JsonUtility.ToJson(body);

        Debug.Log("Https put request url ************** : "+url);
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


    /// <summary>
    /// Sends an asynchronous Delete request, waits for the request to complete, and parses the result into an object of the specified type (T) and returns it.
    /// </summary>
    /// <param name="endpoint"></param>
    /// <returns></returns>
    /// <exception cref="NetworkException"></exception>
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
                ApiErrorResponse errorResponse = TryParseErrorResponse(request.downloadHandler?.text);
                string errorMessage = GetErrorMessage(request.error, errorResponse);
                throw new NetworkException(request.responseCode, errorMessage, errorResponse?.error?.code);
            }

            return request.responseCode >= 200 && request.responseCode < 300;
        }
    }


    /// <summary>
    /// This function combines base URLs and endpoints and converts query parameters in dictionary form into URL-encoded query strings to generate a full request URL.
    /// We created a helper function to build URLs because typing out the full strings every time is prone to errors.
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    private string BuildUrl(string endpoint, Dictionary<string, string> queryParams = null)
    {
        // 1. Sanitize and combine BaseUrl and endpoint to prevent double-slash (//) errors
        string url = Config.BaseUrl.TrimEnd('/') + "/" + endpoint.TrimStart('/');

        // 2. If there are query parameters, start building the query string
        if (queryParams != null && queryParams.Count > 0)
        {
            var queryString = new StringBuilder("?");
            foreach (var param in queryParams)
            {
                // 3. Encode both Key and Value to safely handle special characters (like spaces or symbols)
                queryString.Append($"{Uri.EscapeDataString(param.Key)}={Uri.EscapeDataString(param.Value)}&");
            }
            
            // 4. Remove the trailing '&' character and append the query string to the URL
            url += queryString.ToString().TrimEnd('&');
        }

        return url;
    }


    /// <summary>
    /// This is a function that is initially set up for the network.
    /// Currently, it is only set for timeout.
    /// </summary>
    /// <param name="request"></param>
    private void SetupRequest(UnityWebRequest request)
    {
        //timeout setting
        request.timeout = Config.TimeoutSeconds;

        // if (!string.IsNullOrEmpty(Config.AuthToken))
        // {
        //     request.SetRequestHeader("Authorization", $"Bearer {Config.AuthToken}");
        // }
        //
        // foreach (var header in Config.DefaultHeaders)
        // {
        //     request.SetRequestHeader(header.Key, header.Value);
        // }
    }

    /// <summary>
    /// Validates the raw network response, handles error cases, and deserializes the JSON data into a typed ApiResponse object.
    /// It acts as a central gateway to ensure all incoming data follows the expected format before being used by the game logic.
    /// I got help from AI regarding the logic of the code.
    /// </summary>
    /// <param name="request"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="NetworkException"></exception>
    private ApiResponse<T> HandleResponse<T>(UnityWebRequest request)
    {
        // 1. Extract response body and log it for debugging
        string responseText = request.downloadHandler?.text ?? string.Empty;
        NetworkLogger.Log($"Response: {responseText}");

        // 2. Check for physical connection errors or HTTP protocol errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            ApiErrorResponse errorResponse = TryParseErrorResponse(responseText);
            string errorMessage = GetErrorMessage(request.error, errorResponse);

            NetworkLogger.LogError($"Request failed: {errorMessage}");
            throw new NetworkException(request.responseCode, errorMessage, errorResponse?.error?.code);
        }

        try
        {
            // 3. Deserialize the JSON string into the generic ApiResponse<T> structure
            ApiResponse<T> response = JsonUtility.FromJson<ApiResponse<T>>(responseText);

            // 4. Validate if the parsing was successful and check the server's internal success flag
            if (response == null)
            {
                throw new NetworkException(request.responseCode, "Empty response body");
            }

            if (!response.isSuccess)
            {
                string errorMessage = GetErrorMessage("Request failed", response.error);
                throw new NetworkException(request.responseCode, errorMessage, response.error?.code);
            }

            // 5. Return the verified and parsed response object
            return response;
        }
        catch (NetworkException)
        {
            // Re-throw known network exceptions to be handled by the caller
            throw;
        }
        catch (Exception ex)
        {
            // 6. Catch unexpected errors during the JSON parsing process
            NetworkLogger.LogError($"JSON parsing failed: {ex.Message}");
            throw new NetworkException(request.responseCode, $"Failed to parse JSON: {ex.Message}");
        }
    }

    
    
    /// <summary>
    /// Attempts to parse the raw response text into an ApiErrorResponse object.
    /// This helps extract specific server-side error codes or messages when a request fails.
    /// </summary>
    /// <param name="responseText"></param>
    /// <returns></returns>
    private ApiErrorResponse TryParseErrorResponse(string responseText)
    {
        // 1. Return null immediately if the response body is empty or null
        if (string.IsNullOrWhiteSpace(responseText))
        {
            return null;
        }

        try
        {
            // 2. Attempt to deserialize the text into the predefined error structure
            return JsonUtility.FromJson<ApiErrorResponse>(responseText);
        }
        catch (Exception ex)
        {
            // 3. If parsing fails (e.g., response is HTML or plain text), log the error and return null
            NetworkLogger.LogError($"Error response parsing failed: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Verify that there is an error value in the response value
    /// This setup lets us safely grab the error message whether we have the full response object or just the error part itself.
    /// The idea by AI
    /// </summary>
    /// <param name="fallback"></param>
    /// <param name="errorResponse"></param>
    /// <returns></returns>
    private string GetErrorMessage(string fallback, ApiErrorResponse errorResponse)
    {
        if (errorResponse?.error == null)
        {
            return fallback;
        }

        return GetErrorMessage(fallback, errorResponse.error);
    }

    /// <summary>
    /// Verify that there is an error "message" value in the response value
    /// </summary>
    /// <param name="fallback"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    private string GetErrorMessage(string fallback, ApiError error)
    {
        if (error == null || string.IsNullOrWhiteSpace(error.message))
        {
            return fallback;
        }

        return error.message;
    }
}