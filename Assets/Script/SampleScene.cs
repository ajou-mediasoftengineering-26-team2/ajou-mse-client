using UnityEngine;
//202322158 이준상

/// <summary>
/// Module Test Scene
/// </summary>
public class SampleScene : MonoBehaviour
{
    private TestViewModel _viewModel;

    private void Start() 
    {
        _viewModel = ViewModelLocator.Instance.Get<TestViewModel>();

        _viewModel.FirebaseMessage.Subscribe(LogFirebase);
        _viewModel.RepoData.Subscribe(LogRepo);
        
    }

    private void LogFirebase(string msg) => Debug.Log($"Firebase: {msg}");
    private void LogRepo(string data) => Debug.Log($"Repo: {data}");
    private void OnDestroy()
    {
        _viewModel?.Dispose();
    }
}