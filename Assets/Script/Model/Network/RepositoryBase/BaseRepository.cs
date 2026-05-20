using System.Collections.Generic;
using System.Threading.Tasks;

//202322158 이준상

/// <summary>
/// Repository Base Class
/// Performing a Real HTTP Request Using Network Manager
/// </summary>
public abstract class BaseRepository
{
    protected readonly NetworkManager networkManager;
    protected abstract string EndpointBase { get; set; }

    protected BaseRepository()
    {
        networkManager = NetworkManager.Instance;
    }
}
