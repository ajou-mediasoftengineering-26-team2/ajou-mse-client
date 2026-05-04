using System.Threading.Tasks;

public interface IShopRepository
{
    Task<ApiResponse<GetShopInfoResponse>> GetShopInfo(string playerId);
    Task<ApiResponse<UpgradeResponse>> PostUpgrade(string playerId);
}

public class ShopRepository : BaseRepository, IShopRepository
{
    protected override string EndpointBase => "shop"; // TODO: 엔드포인트 확정 후 수정

    public async Task<ApiResponse<GetShopInfoResponse>> GetShopInfo(string playerId)
    {
        return await networkManager.Get<GetShopInfoResponse>(
            EndpointBase + "/info",
            new System.Collections.Generic.Dictionary<string, string> { { "id", playerId } }
        );
    }

    public async Task<ApiResponse<UpgradeResponse>> PostUpgrade(string playerId)
    {
        var body = new PostUpgradeRequest { id = playerId };
        return await networkManager.Post<UpgradeResponse>(EndpointBase + "/upgrade", body);
    }
}