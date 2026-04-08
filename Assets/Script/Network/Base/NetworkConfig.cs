using System.Collections.Generic;

/// <summary>
/// 네트워크 설정 관리 클래스
/// </summary>
public class NetworkConfig
{
    public string BaseUrl { get; set; } = "http://localhost:8080";
    public int TimeoutSeconds { get; set; } = 30;
    public string AuthToken { get; set; }
    public Dictionary<string, string> DefaultHeaders { get; private set; }

    public NetworkConfig()
    {
        DefaultHeaders = new Dictionary<string, string>();
    }

    /// <summary>
    /// 기본 헤더 추가
    /// </summary>
    public void AddDefaultHeader(string key, string value)
    {
        DefaultHeaders[key] = value;
    }

    /// <summary>
    /// 기본 헤더 제거
    /// </summary>
    public void RemoveDefaultHeader(string key)
    {
        DefaultHeaders.Remove(key);
    }
}
