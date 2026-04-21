using System;

[Serializable]
public class PostLoginRequest
{
    public string userID;
    public PostLoginRequest()
    {
        userID = "";
    }
}

[Serializable]
public class PostLoginResponse
{
    public string userUID;
}