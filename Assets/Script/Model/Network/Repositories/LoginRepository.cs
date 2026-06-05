using System.Collections.Generic;
using System.Threading.Tasks;

//202422170 주형준
public interface ILoginRepository
{
    Task<ApiResponse<PostLoginResponse>> PostUserID(string playerName);
    Task<ApiResponse<EmptyResponse>> DeletePlayer(string playerId);
}

public class LoginRepository : BaseRepository, ILoginRepository 
{
    protected override string EndpointBase
    {
        get => "auth/player"; // 플레이어 엔드 포인트
        set
        {
        }
    }

    public async Task<ApiResponse<PostLoginResponse>> PostUserID(string playerName)
    {
        PostLoginRequest body = new PostLoginRequest();
        body.playerName = playerName;
        //PostLoginRequest body = new PostLoginRequest { playerName = playerName };
        return await networkManager.Post<PostLoginResponse>(EndpointBase, body);
    }
    
    public async Task<ApiResponse<EmptyResponse>> DeletePlayer(string playerId)
    {
        var body = new DeletePlayerRequest { playerId = playerId };
        return await networkManager.Delete<EmptyResponse>(EndpointBase, body);
    }
    
}
