using System.Threading.Tasks;

// 202422170 주형준
public interface IPerkAndShopRepository
{
    Task<ApiResponse<EmptyResponse>> PutChoice(string playerId, string perk);
    Task<ApiResponse<EmptyResponse>> PutAck(string playerId);
}

public class PerkAndShopRepository : BaseRepository, IPerkAndShopRepository
{
    protected override string EndpointBase
    {
        get => "perk";
        set
        {
        }
    }

    public async Task<ApiResponse<EmptyResponse>> PutChoice(string playerId, string perk)
    {
        var body = new PutPerkAndShopChoiceRequest { playerId = playerId, perk = perk };
        return await networkManager.Put<EmptyResponse>(EndpointBase + "/choice", body);
    }

    public async Task<ApiResponse<EmptyResponse>> PutAck(string playerId)
    {
        var body = new PutPerkAndShopAckRequest { playerId = playerId };
        return await networkManager.Put<EmptyResponse>(EndpointBase + "/ack", body);
    }
}