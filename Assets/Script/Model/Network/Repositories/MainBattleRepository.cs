using System.Threading.Tasks;

public interface IMainBattleRepository
{
    Task<ApiResponse<RoomInfoModel>> PostHandAction(string playerId, int moveType);
}

public class MainBattleRepository : BaseRepository, IMainBattleRepository
{
    protected override string EndpointBase => "main";

    public async Task<ApiResponse<RoomInfoModel>> PostHandAction(string playerId, int moveType)
    {
        PostHandActionRequest body = new PostHandActionRequest();
        body.playerId = playerId;
        body.moveType = moveType;
        return await networkManager.Post<RoomInfoModel>(EndpointBase, body);
    }
}
