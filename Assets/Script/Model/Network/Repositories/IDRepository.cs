using System.Collections.Generic;
using System.Threading.Tasks;

public interface IIDRepository
{
    Task<ApiResponse<IDRequestBody>> PostUserID(string UserID);
}

public class IDRepository : BaseRepository, IIDRepository 
{
    protected override string EndpointBase => "test";// 아직 엔드포인트 없어서 test로 진행했습니다

    public async Task<ApiResponse<IDRequestBody>> PostUserID(string userID)
    {
        IDRequestBody body = new IDRequestBody();
        body.userID = userID;
        var response = await networkManager.Post<ApiResponse<IDRequestBody>>(EndpointBase, body);
        return response.data;
    }
    
}