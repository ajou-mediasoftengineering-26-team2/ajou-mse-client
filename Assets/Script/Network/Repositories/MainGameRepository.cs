using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework.Internal;

public interface IMainGameRepository
{
    Task<ApiResponse<Test>> PostMainGameModel(int playerId, int moveType);
}


public class MainGameRepository : BaseRepository, IMainGameRepository
{
    protected override string EndpointBase => "main";
    
    public async Task<ApiResponse<Test>> PostMainGameModel(int playerId, int moveType)
    {
        MainGameModelBody body = new MainGameModelBody();
        body.playerNumber = playerId;
        body.selectMoveType =  moveType;
        var response = await networkManager.Post<ApiResponse<Test>>(EndpointBase, body);
        return response;
    }
}