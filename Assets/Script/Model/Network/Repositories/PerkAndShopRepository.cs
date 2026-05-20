using System.Threading.Tasks;

// 202422170 주형준
public interface IPerkAndShopRepository
{
    Task<ApiResponse<GetPerkAndShopInfoResponse>>  GetInfo(string playerId);
    Task<ApiResponse<PerkAndShopUpgradeResponse>>  PostUpgrade(string playerId);
    Task<ApiResponse<PerkAndShopSelectResponse>>   PostSelectPerk(string playerId, int perkId);
}

public class PerkAndShopRepository : BaseRepository, IPerkAndShopRepository
{
    protected override string EndpointBase
    {
        get => "shop"; // 서버 팀 확정 후 수정
        set
        {
            
        }
    }

    public async Task<ApiResponse<GetPerkAndShopInfoResponse>> GetInfo(string playerId)
    {
        return await networkManager.Get<GetPerkAndShopInfoResponse>(
            EndpointBase + "/info",
            new System.Collections.Generic.Dictionary<string, string> { { "id", playerId } }
        );
    }

    public async Task<ApiResponse<PerkAndShopUpgradeResponse>> PostUpgrade(string playerId)
    {
        var body = new PostPerkAndShopUpgradeRequest { id = playerId };
        return await networkManager.Post<PerkAndShopUpgradeResponse>(EndpointBase + "/upgrade", body);
    }

    public async Task<ApiResponse<PerkAndShopSelectResponse>> PostSelectPerk(string playerId, int perkId)
    {
        var body = new PostPerkAndShopSelectRequest { id = playerId, perkId = perkId };
        return await networkManager.Post<PerkAndShopSelectResponse>(EndpointBase + "/perk/select", body);
    }
}