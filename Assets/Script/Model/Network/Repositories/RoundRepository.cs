using System;
using System.Threading.Tasks;

//202322158 이준상

/// <summary>
/// Define Round Repository
/// </summary>
public interface IRoundRepository
{
    Task<ApiResponse<Object>> startAck(string playerId);
    Task<ApiResponse<Object>> endAck(string playerId);
}



/// <summary>
/// Implement IRoundRepository
/// </summary>
public class RoundRepository : BaseRepository, IRoundRepository
{
    private string _endpointBase = "round";

    protected override string EndpointBase
    {
        get => _endpointBase; 
        set => _endpointBase = value;
        
    }
    public async Task<ApiResponse<object>> startAck(string playerId)
    {
        RoundModel body = new RoundModel()
        {
            playerId = playerId,
        };
        string fullEndPoint = _endpointBase + "/start-ack";
        
        return await networkManager.Put<Object>(fullEndPoint, body);
    }

    public async Task<ApiResponse<object>> endAck(string playerId)
    {
        RoundModel body = new RoundModel()
        {
            playerId = playerId,
        };
        string fullEndPoint = _endpointBase + "/end-ack";
        
        return await networkManager.Put<Object>(fullEndPoint, body);
    }
}