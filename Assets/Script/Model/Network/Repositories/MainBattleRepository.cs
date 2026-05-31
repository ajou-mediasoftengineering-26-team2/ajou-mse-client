using System.Threading.Tasks;
using UnityEngine;
using Object = System.Object;

//202422170 주형준
public interface IMainBattleRepository
{
    Task<ApiResponse<RoomInfoModel>> PutChoice(string playerId, string choice);
    Task<ApiResponse<Object>> PutAck(string playerId);
}

public class MainBattleRepository : BaseRepository, IMainBattleRepository
{
    private string _endpointBase = "turn";

    protected override string EndpointBase
    {
        get => _endpointBase;        // 이제 저장된 변수 값을 반환합니다.
        set => _endpointBase = value; // 이제 더해진 값이 정상적으로 저장됩니다.
    }

    public async Task<ApiResponse<RoomInfoModel>> PutChoice(string playerId, string choice)
    {
        PutChoiceRequest body = new PutChoiceRequest
        {
            id = playerId,
            choice = choice
        };
        string fullEndpoint = EndpointBase + "/choice";
        Debug.Log(body.choice + " " +  body.id);
        return await networkManager.Put<RoomInfoModel>(fullEndpoint, body);
    }
    
    
    public async Task<ApiResponse<Object>> PutAck(string playerId)
    {
        PutTurnAckRequest body = new PutTurnAckRequest
        {
            playerId = playerId
        };
        string fullEndpoint = EndpointBase + "/ack";
        return await networkManager.Put<Object>(fullEndpoint, body);
    }
}
