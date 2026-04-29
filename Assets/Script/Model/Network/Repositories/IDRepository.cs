using System.Collections.Generic;
using System.Threading.Tasks;

public interface IIDRepository
{
    Task<ApiResponse<PostLoginResponse>> PostUserID(string playerName);
}

public class IDRepository : BaseRepository, IIDRepository 
{
    protected override string EndpointBase => "auth/player"; // 플레이어 엔드 포인트

    public async Task<ApiResponse<PostLoginResponse>> PostUserID(string playerName)
    {
        PostLoginRequest body = new PostLoginRequest();
        body.playerName = playerName;
        var response = await networkManager.Post<ApiResponse<PostLoginResponse>>(EndpointBase, body);
        return response.data;
    }
    
}