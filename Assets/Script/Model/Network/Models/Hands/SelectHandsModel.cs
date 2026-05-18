// 202422170 주형준
using System;

[Serializable]
public class GetSelectHandsInfoResponse
{
    public string currentHand; 
}

[Serializable]
public class PostSelectHandRequest
{
    public string id; 
    public string handType; 
}

[Serializable]
public class PostSelectHandResponse { }