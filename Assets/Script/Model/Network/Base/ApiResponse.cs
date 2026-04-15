using System;

/// <summary>
/// 표준 API 응답 래퍼
/// 성공/실패 상태와 데이터를 포함
/// </summary>
[Serializable]
public class ApiResponse<T>
{
    public bool Success;
    public string Message;
    public T Data;
    public int Code;
}
