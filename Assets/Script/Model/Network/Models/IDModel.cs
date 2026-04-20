using System;

[Serializable]
public class IDRequestBody
{
    public string userID;
    public IDRequestBody()
    {
        userID = "";
    }
}

[Serializable]
public class IDResponse
{
    public string userUID;
}