using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;



//202322158 이준상

/// <summary>
/// View class that controls login UI elements and binds them to the ViewModel.
/// </summary>
public class LoginView : MonoBehaviour
{
   // Reference to the associated ViewModel
   private LoginViewModel _viewModel;
   
   // UI elements from UXML
   private TextField _LoginIDInput;
   private Button _createButton;
   
   // Labels for displaying information
   private Label _stationUILabel;
   private Label _nickNameLabel;

   // VisualElements for different UI sections
   private VisualElement loginUIRoot;
   private VisualElement stationUIRoot;
   private VisualElement lobbyWaitingUIRoot;
   
   [FormerlySerializedAs("displayController")] 
   [SerializeField] public SubwayDisplayView displayView;

   private void OnEnable()
   {
      // 1. Retrieve the ViewModel via the Locator (Handles lazy instantiation)
      _viewModel = ViewModelLocator.Instance.Get<LoginViewModel>();
      
      // 2. Locate and bind the root visual elements from UIDocuments in the scene.
      loginUIRoot = GameObject.Find("LoginUI").GetComponent<UIDocument>().rootVisualElement;
      stationUIRoot = GameObject.Find("StationUI").GetComponent<UIDocument>().rootVisualElement;
      lobbyWaitingUIRoot = GameObject.Find("LobbyWaitingUI").GetComponent<UIDocument>().rootVisualElement;
      
      // Hide the lobby waiting UI initially.
      lobbyWaitingUIRoot.style.display = DisplayStyle.None;
      
      // 3. Assign specific UI controls using UQuery (Q).
      _LoginIDInput = loginUIRoot.Q<TextField>("LoginID");
      _createButton = loginUIRoot.Q<Button>("Create");
      _stationUILabel = stationUIRoot.Q<Label>("StationName");
      _nickNameLabel = lobbyWaitingUIRoot.Q<Label>("NicknameLabel");
      
      // 4. Setup button click interactions.
      _createButton.clicked += () =>
      {
         // Notify the EventBus (e.g., for playing UI sound effects).
         EventBus.Publish(new ButtonEvent());
         // Pass the input ID to the ViewModel to attempt login.
         _viewModel.OnSubmitID(_LoginIDInput.value);
      };
      
      // 5. [Observer Subscription] Bind UI updates to ViewModel state changes.
      
      // Observe login success state.
      _viewModel.IsSuccess.Subscribe(OnLoginSuccess);
      
      // Observe error messages: Display a toast notification on failure.
      _viewModel.ErrorMsg.Subscribe(msg =>
      {
         if (string.IsNullOrEmpty(msg)) return;
         Toast.Show(msg);
         Debug.LogError($"{msg}: Login Error");
      });
      
      // Observe current station info.
      _viewModel.SubwayStation.Subscribe(station =>
      {
         _stationUILabel.text = station;
      });
      
      // Observe match start: Transition to the battle scene when triggered.
      _viewModel.IsMatchStarted.Subscribe(started =>
      {
         if (!started) return;
         displayView.StopDisplay();
         SceneManager.LoadScene("MainBattleScene");
      });
   }

   /// <summary>
   /// Updates the UI and prepares the lobby state upon successful login.
   /// </summary>
   /// <param name="isSuccess"></param>
   private void OnLoginSuccess(bool isSuccess)
   {
      if (!isSuccess) return;
      
      string lobbyId = _viewModel.LobbyId.Value;
      string playerId = _viewModel.PlayerId.Value;
      Debug.Log($"{GameSetting.LOGINSUCCESS}! Player: {playerId}, Lobby: {lobbyId}");

      // UI Transition: Swap Login UI with Lobby UI.
      loginUIRoot.style.display = DisplayStyle.None;
      lobbyWaitingUIRoot.style.display = DisplayStyle.Flex;
      
      Toast.Show(GameSetting.LOGINSUCCESS);
      _nickNameLabel.text = _LoginIDInput.value;
      
      // Start subway display visuals.
      displayView.StartDisplay();
   }
   
   // Clean up the ViewModel to prevent memory leaks and dangling subscriptions.
   private void OnDestroy()
   {
      _viewModel?.Dispose();
   }
}
