// 202422170 주형준
using System.Threading.Tasks;

public interface ISelectHandsRepository
{
    Task<ApiResponse<EmptyResponse>> PostSelectHand(string playerId, string handType);
    Task<ApiResponse<EmptyResponse>> PutAck(string playerId); 
}

public class SelectHandsRepository : BaseRepository, ISelectHandsRepository
{
    protected override string EndpointBase { get => "elemental"; set { } }

    public async Task<ApiResponse<EmptyResponse>> PostSelectHand(string playerId, string handType)
    {
        var body = new PostSelectHandRequest { playerId = playerId, handElemental = handType };
        return await networkManager.Put<EmptyResponse>($"{EndpointBase}/choice", body);
    }

    public async Task<ApiResponse<EmptyResponse>> PutAck(string playerId) 
    {
        var body = new PutTurnAckRequest { playerId = playerId };
        return await networkManager.Put<EmptyResponse>($"{EndpointBase}/ack", body);
    }
}