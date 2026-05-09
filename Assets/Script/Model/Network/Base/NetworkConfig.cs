using System.Collections.Generic;

//2023322158 이준상

//Class that manage network settings.
public class NetworkConfig
{
    //Base of Url to use server communication.
    public string BaseUrl { get; set; } = "http://ajou-mse-sss.zikbakguri.com:8080/";
    
    //timeout setting
    public int TimeoutSeconds { get; set; } = 30;
    
    //auth/header variables.
    public string AuthToken { get; set; }
    public Dictionary<string, string> DefaultHeaders { get; private set; }

    /// <summary>
    /// define header dictionary
    /// </summary>
    public NetworkConfig()
    {
        DefaultHeaders = new Dictionary<string, string>();
    }

    
    /// <summary>
    /// I created add header and remove header function
    /// because I think we needed a header for login and battle, but we don't use it anymore
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void AddDefaultHeader(string key, string value)
    {
        DefaultHeaders[key] = value;
    }

    
    
    public void RemoveDefaultHeader(string key)
    {
        DefaultHeaders.Remove(key);
    }
}
