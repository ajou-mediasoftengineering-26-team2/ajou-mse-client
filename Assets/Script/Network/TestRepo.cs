using UnityEngine;
using System.Threading.Tasks;


public class TestRepo : MonoBehaviour
{
    private IMainGameRepository  mainGameRepository;

    private void Start()
    {
        RepositoryFactory.Instance.Register<IMainGameRepository, MainGameRepository>();
        
        mainGameRepository = RepositoryFactory.Instance.Get<IMainGameRepository>();
        var a = mainGameRepository.PostMainGameModel(1, 1);
    }
}
