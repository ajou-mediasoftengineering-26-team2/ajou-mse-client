using System;


//202322158 이준상
public class NetworkException : Exception
{
    //server code 
    public long ResponseCode { get; private set; }
    
    //api code and error at ApiResponse.cs
    public string ErrorMessage { get; private set; }
    public int? ApiErrorCode { get; private set; }

    /// <summary>
    /// if just network error, we use this constructor.
    /// </summary>
    /// <param name="responseCode"></param>
    /// <param name="errorMessage"></param>
    public NetworkException(long responseCode, string errorMessage) 
        : base($"Network Error [{responseCode}]: {errorMessage}")
    {
        ResponseCode = responseCode;
        ErrorMessage = errorMessage;
        ApiErrorCode = null;
    }

    /// <summary>
    /// if network communication success but, server Apiresponse give error,
    /// we use this constructor.
    /// </summary>
    /// <param name="responseCode"></param>
    /// <param name="errorMessage"></param>
    /// <param name="apiErrorCode"></param>
    public NetworkException(long responseCode, string errorMessage, int? apiErrorCode)
        : base($"Network Error [{responseCode}] (API: {(apiErrorCode.HasValue ? apiErrorCode.Value.ToString() : "-")}): {errorMessage}")
    {
        ResponseCode = responseCode;
        ErrorMessage = errorMessage;
        ApiErrorCode = apiErrorCode;
    }

    
    //Variables that were created to handle errors before,
    //but variables that were put on hold after processing errors in apiResponse
    public bool IsClientError => ResponseCode >= 400 && ResponseCode < 500;
    public bool IsServerError => ResponseCode >= 500 && ResponseCode < 600;
    public bool IsTimeout => ResponseCode == 408;
    public bool IsUnauthorized => ResponseCode == 401;
    public bool IsForbidden => ResponseCode == 403;
    public bool IsNotFound => ResponseCode == 404;
}
