using System.Threading.Tasks;

// 202422170 주형준
public interface IElementalRepository
{
    Task<ApiResponse<EmptyResponse>> PutChoice(string playerId, string handElemental);
    Task<ApiResponse<EmptyResponse>> PutAck(string playerId);
}

public class ElementalRepository : BaseRepository, IElementalRepository
{
    protected override string EndpointBase
    {
        get => "elemental";
        set
        {
        }
    }

    public async Task<ApiResponse<EmptyResponse>> PutChoice(string playerId, string handElemental)
    {
        var body = new PutElementalChoiceRequest { playerId = playerId, handElemental = handElemental };
        return await networkManager.Put<EmptyResponse>(EndpointBase + "/choice", body);
    }

    public async Task<ApiResponse<EmptyResponse>> PutAck(string playerId)
    {
        var body = new PutElementalAckRequest { playerId = playerId };
        return await networkManager.Put<EmptyResponse>(EndpointBase + "/ack", body);
    }
}
