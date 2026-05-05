using System.Threading.Tasks;

public interface IMainBattleRepository
{
    Task<ApiResponse<RoomInfoModel>> PutChoice(string playerId, string choice);
}

public class MainBattleRepository : BaseRepository, IMainBattleRepository
{
    protected override string EndpointBase => "turn/choice";

    public async Task<ApiResponse<RoomInfoModel>> PutChoice(string playerId, string choice)
    {
        PutChoiceRequest body = new PutChoiceRequest
        {
            id = playerId,
            choice = choice
        };
        return await networkManager.Put<RoomInfoModel>(EndpointBase, body);
    }
}
