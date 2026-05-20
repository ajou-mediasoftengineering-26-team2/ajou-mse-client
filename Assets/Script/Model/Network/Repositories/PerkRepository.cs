using System.Threading.Tasks;

//202422170 주형준
public interface IPerkRepository
{
    Task<ApiResponse<GetPerkChoicesResponse>> GetPerkChoices(string playerId);
    Task<ApiResponse<SelectPerkResponse>> PostSelectPerk(string playerId, int perkId);
}

public class PerkRepository : BaseRepository, IPerkRepository
{
    protected override string EndpointBase
    {
        get => "perk"; // TODO: 엔드포인트 확정 후 수정
        set
        {
        }
    }

    public async Task<ApiResponse<GetPerkChoicesResponse>> GetPerkChoices(string playerId)
    {
        return await networkManager.Get<GetPerkChoicesResponse>(
            EndpointBase + "/choices",
            new System.Collections.Generic.Dictionary<string, string> { { "id", playerId } }
        );
    }

    public async Task<ApiResponse<SelectPerkResponse>> PostSelectPerk(string playerId, int perkId)
    {
        var body = new PostSelectPerkRequest { id = playerId, perkId = perkId };
        return await networkManager.Post<SelectPerkResponse>(EndpointBase + "/select", body);
    }
}