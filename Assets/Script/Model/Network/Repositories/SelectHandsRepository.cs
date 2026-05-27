// 202422170 주형준
using System.Threading.Tasks;

public interface ISelectHandsRepository
{
    Task<ApiResponse<GetSelectHandsInfoResponse>> GetInfo(string playerId);
    Task<ApiResponse<PostSelectHandResponse>> PostSelectHand(string playerId, string handType);
}

public class SelectHandsRepository : BaseRepository, ISelectHandsRepository
{
    protected override string EndpointBase => "hand"; // confirm with server team

    public async Task<ApiResponse<GetSelectHandsInfoResponse>> GetInfo(string playerId)
    {
        return await networkManager.Get<GetSelectHandsInfoResponse>($"{EndpointBase}/info?id={playerId}");
    }

    public async Task<ApiResponse<PostSelectHandResponse>> PostSelectHand(string playerId, string handType)
    {
        var body = new PostSelectHandRequest { id = playerId, handType = handType };
        return await networkManager.Post<PostSelectHandResponse>($"{EndpointBase}/select", body);
    }
}