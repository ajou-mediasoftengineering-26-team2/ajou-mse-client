using System.Threading.Tasks;

public interface ITestRepository
{
    Task<ApiResponse<Test>> getTest();
}

public class TestRepository : BaseRepository, ITestRepository
{
    protected override string EndpointBase
    {
        get => "test";
        set => throw new System.NotImplementedException();
    }

    public async Task<ApiResponse<Test>> getTest()
    {
        var response = await networkManager.Get<Test>(EndpointBase);
        return response;
    }
}