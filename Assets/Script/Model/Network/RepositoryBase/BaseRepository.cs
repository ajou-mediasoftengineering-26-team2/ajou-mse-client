using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// 레포지토리 베이스 클래스
/// NetworkManager를 사용하여 실제 HTTP 요청을 수행
/// </summary>
public abstract class BaseRepository
{
    protected readonly NetworkManager networkManager;
    protected abstract string EndpointBase { get; }

    protected BaseRepository()
    {
        networkManager = NetworkManager.Instance;
    }
}
