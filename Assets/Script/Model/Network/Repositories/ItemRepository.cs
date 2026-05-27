using System.Threading.Tasks;

// 202422170 주형준
public interface IItemRepository
{
    Task<ApiResponse<EmptyResponse>> PutAck(string playerId);
}

public class ItemRepository : BaseRepository, IItemRepository
{
    protected override string EndpointBase
    {
        get => "item";
        set
        {
        }
    }

    public async Task<ApiResponse<EmptyResponse>> PutAck(string playerId)
    {
        var body = new PutItemAckRequest { playerId = playerId };
        return await networkManager.Put<EmptyResponse>(EndpointBase + "/ack", body);
    }
}