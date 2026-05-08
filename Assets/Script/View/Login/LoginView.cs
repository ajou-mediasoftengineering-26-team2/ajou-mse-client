using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LoginView : MonoBehaviour
{
   //뷰모델 참조
   private LoginViewModel _viewModel;
   //UXML ID창, 생성 버튼
   private TextField _LoginIDInput;
   private Button _createButton;
   
   private Label _stationUILabel;
   private Label _nickNameLabel;

   private VisualElement loginUIRoot;
   private VisualElement stationUIRoot;
   private VisualElement lobbyWaitingUIRoot;
   
   
   [FormerlySerializedAs("displayController")] [SerializeField]
   public SubwayDisplayView displayView;
   private void OnEnable()
   {
      
      _viewModel = ViewModelLocator.Instance.Get<LoginViewModel>();
      
      
      loginUIRoot = GameObject.Find("LoginUI").GetComponent<UIDocument>().rootVisualElement;
      stationUIRoot = GameObject.Find("StationUI").GetComponent<UIDocument>().rootVisualElement;
      lobbyWaitingUIRoot = GameObject.Find("LobbyWaitingUI").GetComponent<UIDocument>().rootVisualElement;
      
      lobbyWaitingUIRoot.style.display = DisplayStyle.None;
      
      _LoginIDInput = loginUIRoot.Q<TextField>("LoginID");
      _createButton = loginUIRoot.Q<Button>("Create");
      _stationUILabel = stationUIRoot.Q<Label>("StationName");
      _nickNameLabel = lobbyWaitingUIRoot.Q<Label>("NicknameLabel");
      
      _createButton.clicked += () =>
      {
         EventBus.Publish(new ButtonEvent());
         _viewModel.OnSubmitID(_LoginIDInput.value);
      };
      
      
      // 옵저버 구독 후 받아온 값에 대해서 성공 실패 여부를 묻고  성공시 로그인 성공, 아니면 로그인 에러
      _viewModel.IsSuccess.Subscribe(OnLoginSuccess);
      _viewModel.ErrorMsg.Subscribe(msg =>
      {
         Debug.LogError(msg  +" test");
         if (string.IsNullOrEmpty(msg)) return;
         Toast.Show(msg);
         Debug.LogError(msg + ": 로그인 에러");
      });
      _viewModel.SubwayStation.Subscribe(station =>
      {
         _stationUILabel.text = station;
      });
      
      _viewModel.IsMatchStarted.Subscribe(started =>
      {
         if (!started) return;
         displayView.StopDisplay();
         SceneManager.LoadScene("111HyungJun_Dev_Junsang");//여기 부분을 그 다음에 battle씬으로 가게 하면 될 것 같습니다
      });
   }

   private void OnLoginSuccess(bool isSuccess)
   {
      if (!isSuccess) return;
      
      string lobbyId = _viewModel.LobbyId.Value;
      string playerId = _viewModel.PlayerId.Value;
      Debug.Log($"{GameSetting.LOGINSUCCESS}! playerId: {playerId}, lobbyId: {lobbyId}");// 씬 혹은 ui 바꾸기
      loginUIRoot.style.display = DisplayStyle.None;
      lobbyWaitingUIRoot.style.display = DisplayStyle.Flex;
      Toast.Show(GameSetting.LOGINSUCCESS);
      _nickNameLabel.text = _LoginIDInput.value;
      displayView.StartDisplay();
   }
   
   
   // 메모리 누수 방지를 위해 할당 해제
   private void OnDestroy()
   {
      _viewModel?.Dispose();
   }
}
