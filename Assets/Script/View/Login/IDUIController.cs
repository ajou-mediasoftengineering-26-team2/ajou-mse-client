using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class IDUIController : MonoBehaviour
{
   //뷰모델 참조
   private IDViewModel _viewModel;
   //UXML ID창, 생성 버튼
   private TextField _LoginIDInput;
   private Button _createButton;

   private void OnEnable()
   {
      var root = GetComponent<UIDocument>().rootVisualElement;
      
      _LoginIDInput = root.Q<TextField>("LoginID");
      _createButton = root.Q<Button>("Create");
      
      _viewModel = ViewModelLocator.Instance.Get<IDViewModel>();
      
      _createButton.clicked += () =>
      {
         EventBus.Publish(new ButtonEvent());
         _viewModel.OnSubmitID(_LoginIDInput.value);
      };
      
      
      // 옵저버 구독 후 받아온 값에 대해서 성공 실패 여부를 묻고  성공시 로그인 성공, 아니면 로그인 에러
      _viewModel.IsSuccess.Subscribe(OnLoginSuccess);
      _viewModel.ErrorMsg.Subscribe(msg =>
      {
         if (string.IsNullOrEmpty(msg)) return;
         Toast.Show(msg);
         Debug.LogError(msg + ": 로그인 에러");
      });
   }

   private void OnLoginSuccess(bool isSuccess)
   {
      if (!isSuccess) return;
      
      string lobbyId = _viewModel.LobbyId.Value;
      string playerId = _viewModel.PlayerId.Value;
      Debug.Log($"{GameSetting.LOGINSUCCESS}! playerId: {playerId}, lobbyId: {lobbyId}");// 씬 혹은 ui 바꾸기
      GameObject.Find("LoginUI").GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.None;
      Toast.Show(GameSetting.LOGINSUCCESS);
   }
   
   
   // 메모리 누수 방지를 위해 할당 해제
   private void OnDestroy()
   {
      _viewModel?.Dispose();
   }
}
