using System;
//202322158 이준상

//DTO that defines the basic response of the api.
[Serializable]
public class ApiError
{
    public int code;
    public string message;
}

[Serializable]
public class ApiResponse<T>
{
    public bool isSuccess;
    public T data;
    public ApiError error;
}

[Serializable]
public class ApiErrorResponse
{
    public bool isSuccess;
    public ApiError error;
}
