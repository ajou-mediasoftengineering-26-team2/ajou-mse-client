using System.Collections.Generic;
using System.Threading.Tasks;

public interface ILoginRepository
{
    Task<ApiResponse<PostLoginResponse>> PostUserID(string playerName);
}

public class LoginRepository : BaseRepository, ILoginRepository 
{
    protected override string EndpointBase => "auth/player"; // 플레이어 엔드 포인트

    public async Task<ApiResponse<PostLoginResponse>> PostUserID(string playerName)
    {
        PostLoginRequest body = new PostLoginRequest();
        body.playerName = playerName;
        //PostLoginRequest body = new PostLoginRequest { playerName = playerName };
        return await networkManager.Post<PostLoginResponse>(EndpointBase, body);
    }
    
}
