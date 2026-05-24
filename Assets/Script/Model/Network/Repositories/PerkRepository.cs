using System.Threading.Tasks;

public interface IPerkRepository
{
    Task<ApiResponse<EmptyResponse>> PutAck(string playerId);
    Task<ApiResponse<EmptyResponse>> PutChoice(string playerId, string perk);
}

public class PerkRepository : BaseRepository, IPerkRepository
{
    protected override string EndpointBase => "perk";

    public async Task<ApiResponse<EmptyResponse>> PutAck(string playerId)
    {
        var body = new PutPerkAckRequest{playerId = playerId};
        return await networkManager.Put<EmptyResponse>(EndpointBase + "/ack", body);
    }

    public async Task<ApiResponse<EmptyResponse>> PutChoice(string playerId, string perk)
    {
        var body = new PutPerkChoiceRequest{perk = perk, playerId = playerId};
        return await networkManager.Put<EmptyResponse>(EndpointBase + "/choice", body);
    }
}