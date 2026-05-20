using System.Threading.Tasks;

//202422170 주형준
public interface IShopRepository
{
    Task<ApiResponse<GetShopInfoResponse>> GetShopInfo(string playerId);
    Task<ApiResponse<UpgradeResponse>> PostUpgrade(string playerId);
}

public class ShopRepository : BaseRepository, IShopRepository
{
    protected override string EndpointBase
    {
        get => "shop"; // TODO: 엔드포인트 확정 후 수정
        set
        {
        }
    }

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