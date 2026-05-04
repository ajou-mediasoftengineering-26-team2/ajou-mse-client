using System.Threading.Tasks;

public interface IMainBattleRepository
{
    Task<ApiResponse<RoomInfoModel>> PutChoice(string playerId, int choice);
}

public class MainBattleRepository : BaseRepository, IMainBattleRepository
{
    protected override string EndpointBase => "turn/choice";

    public async Task<ApiResponse<RoomInfoModel>> PutChoice(string playerId, int choice)
    {
        PostHandActionRequest body = new PostHandActionRequest();
        body.playerId = playerId;
        body.moveType = choice;
        return await networkManager.Post<RoomInfoModel>(EndpointBase, body);
    }
}
