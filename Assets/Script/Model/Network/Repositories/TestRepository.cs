using System.Threading.Tasks;

public interface ITestRepository
{
    Task<Test> getTest();
}

public class TestRepository : BaseRepository, ITestRepository
{
    protected override string EndpointBase => "test";
    public async Task<Test> getTest()
    {
        Test body = new Test();
        var response = await networkManager.Get<Test>(EndpointBase);
        return response;
    }
}