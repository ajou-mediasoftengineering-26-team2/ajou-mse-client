using System;
using System.Threading.Tasks;

public interface IRoundRepository
{
    Task<ApiResponse<Object>> startAck(string playerId);
    Task<ApiResponse<Object>> endAck(string playerId);
}




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