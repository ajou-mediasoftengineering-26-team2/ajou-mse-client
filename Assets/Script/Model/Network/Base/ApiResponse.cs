using System;

/// <summary>
/// 표준 API 응답 래퍼
/// 성공/실패 상태와 데이터를 포함
/// </summary>
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
