// 202422170 주형준
using System.Threading.Tasks;

public interface ISelectHandsRepository
{
    Task<ApiResponse<EmptyResponse>> PostSelectHand(string playerId, string handType);
}

public class SelectHandsRepository : BaseRepository, ISelectHandsRepository
{
    protected override string EndpointBase
    {
        get => "elemental";
        set { }
    }

    public async Task<ApiResponse<EmptyResponse>> PostSelectHand(string playerId, string handType)
    {
        var body = new PostSelectHandRequest { playerId = playerId, handElemental = handType };
        return await networkManager.Put<EmptyResponse>($"{EndpointBase}/choice", body);
    }
}