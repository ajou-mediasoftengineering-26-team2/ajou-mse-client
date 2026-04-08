using UnityEngine;
using System.Threading.Tasks;


public class TestRepo : MonoBehaviour
{
    private IMainGameRepository  mainGameRepository;

    private void Start()
    {
        RepositoryFactory.Instance.Register<IMainGameRepository, MainGameRepository>();
        mainGameRepository = RepositoryFactory.Instance.Get<IMainGameRepository>();


        mainGameRepository.PostMainGameModel(1, 1);
    }
    
}
