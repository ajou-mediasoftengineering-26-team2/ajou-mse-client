using System.Threading.Tasks;

public interface IMainBattleRepository
{
    Task<ApiResponse<PutChoiceResponse>> PutChoice(string playerId, string choice);
}

public class MainBattleRepository : BaseRepository, IMainBattleRepository
{
    protected override string EndpointBase => "turn/choice";

    public async Task<ApiResponse<PutChoiceResponse>> PutChoice(string playerId, string choice)
    {
        PostHandActionRequest body = new PostHandActionRequest();
        body.playerId = playerId;
        body.moveType = moveType;
        return await networkManager.Post<RoomInfoModel>(EndpointBase, body);
    }
}
