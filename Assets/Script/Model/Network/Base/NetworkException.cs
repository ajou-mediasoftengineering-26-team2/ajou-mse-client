using System;

public class NetworkException : Exception
{
    public long ResponseCode { get; private set; }
    public string ErrorMessage { get; private set; }

    public NetworkException(long responseCode, string errorMessage) 
        : base($"Network Error [{responseCode}]: {errorMessage}")
    {
        ResponseCode = responseCode;
        ErrorMessage = errorMessage;
    }

    public bool IsClientError => ResponseCode >= 400 && ResponseCode < 500;
    public bool IsServerError => ResponseCode >= 500 && ResponseCode < 600;
    public bool IsTimeout => ResponseCode == 408;
    public bool IsUnauthorized => ResponseCode == 401;
    public bool IsForbidden => ResponseCode == 403;
    public bool IsNotFound => ResponseCode == 404;
}
