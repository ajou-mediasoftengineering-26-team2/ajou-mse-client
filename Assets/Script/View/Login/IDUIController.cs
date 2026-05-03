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
      
      // 로케이터한테 원하는 뷰모델 가져오기
      _viewModel = ViewModelLocator.Instance.Get<IDViewModel>();
      
      //클릭시 로그인 리턴 값을 받아옴
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
      StartDelayedAction();  
   }
   
   public async void StartDelayedAction()
   {
      await Task.Delay(1000); // 밀리초 단위 (1000ms = 1s)
      // 실행할 코드 (메인 스레드에서 실행되도록 주의가 필요할 수 있음)
      Toast.Show(GameSetting.LOGINSUCCESS);
      Debug.Log("1초 지남!");
   }
   
   // 메모리 누수 방지를 위해 할당 해제
   private void OnDestroy()
   {
      _viewModel?.Dispose();
   }
}
