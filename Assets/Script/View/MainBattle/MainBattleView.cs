
using System;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

//202322158 이준상


/// <summary>
/// Main battle root view that coordinates UI presenters and ViewModel binding.
/// </summary>
public class MainBattleView : MonoBehaviour
{
    
    //viewModel
    private MainBattleViewModel _viewModel;

    //manage ui references
    private MainBattleUIRefs _uiRefs;
    
    //manage ui fragment Presenter
    private MainBattleDotsRenderer _dotsRenderer;
    private MainBattleActionRenderer _actionRenderer;
    private MainBattleBindingRenderer _bindingRenderer;


    private MainBattleEventManager _event;
    [SerializeField]
    public UIDocument mainBattle;
    public UIDocument perks;
    public VisualTreeAsset roundItemTemplate;
    public VisualTreeAsset actionItemSelect;
    public UIDocument toopTip;
    public CameraTurnManager cameraManager;

    private void OnEnable()
    {
        _viewModel = ViewModelLocator.Instance.Get<MainBattleViewModel>();
        _viewModel.SetPlayerAndMatchId(SceneDataBridge.playerId, 
            SceneDataBridge.MatchId, 
            SceneDataBridge.enemyId,
            SceneDataBridge.playerCamera,
            SceneDataBridge.enemyCamera
            );
        
        var activeCameraManager = cameraManager != null ? cameraManager : CameraTurnManager.Instance;
        if (activeCameraManager != null)
        {
            EventBus.Publish(new CameraAction(SceneDataBridge.playerCamera));
        }
        _uiRefs = new MainBattleUIRefs(mainBattle, perks, toopTip);
        //_dotsRenderer = new MainBattleDotsRenderer(roundItemTemplate);
        //_dotsRenderer.Initialize(_uiRefs.MyRoundWinning, _uiRefs.EnemyRoundWinning);
        _actionRenderer = new MainBattleActionRenderer(actionItemSelect, OnActionClicked);
        
        _bindingRenderer = new MainBattleBindingRenderer(
            _viewModel,
            _uiRefs,
            _dotsRenderer,
            _actionRenderer
            );
        _bindingRenderer.Bind();
        
        
    }

    public void UpdateRoundWithDelay()
    {
        _actionRenderer.ShowActions(_uiRefs.ActionContainer, _viewModel.IsAttacker.Value);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("space");
            EventBus.Publish(new CameraAction(CameraType.Action));
            EventBus.Publish(new HitAnimation(
                _viewModel.IsAttacker.Value ? BattleRole.Attack :  BattleRole.Defense,
                SceneDataBridge.playerCamera == CameraType.Camera1 ? Player.First : Player.Second,
                HitActionType.Both5,
                _viewModel.IsAttacker.Value ? _uiRefs.LeftHp : _uiRefs.RightHp));
        }
        
        if (Keyboard.current != null && Keyboard.current.digit0Key.wasPressedThisFrame)
        {
            Debug.Log("space");
            EventBus.Publish(new CameraAction(CameraType.Action));
            EventBus.Publish(new HitAnimation(
                _viewModel.IsAttacker.Value ? BattleRole.Attack :  BattleRole.Defense,
                SceneDataBridge.playerCamera == CameraType.Camera1 ? Player.First : Player.Second,
                HitActionType.Left,
                _viewModel.IsAttacker.Value ? _uiRefs.LeftHp : _uiRefs.RightHp));
        }
        
        if (Keyboard.current != null && Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Debug.Log("space");
            EventBus.Publish(new CameraAction(CameraType.Action));
            EventBus.Publish(new HitAnimation(
                _viewModel.IsAttacker.Value ? BattleRole.Attack :  BattleRole.Defense,
                SceneDataBridge.playerCamera == CameraType.Camera1 ? Player.First : Player.Second,
                HitActionType.Right,
                _viewModel.IsAttacker.Value ? _uiRefs.LeftHp : _uiRefs.RightHp));
        }
        
        if (Keyboard.current != null && Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            Debug.Log("space");
            EventBus.Publish(new CameraAction(CameraType.Action));
            EventBus.Publish(new HitAnimation(
                BattleRole.Attack,
                SceneDataBridge.playerCamera == CameraType.Camera1 ? Player.First : Player.Second,
                HitActionType.Right,
                _viewModel.IsAttacker.Value ? _uiRefs.LeftHp : _uiRefs.RightHp));
        }
        
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            EventBus.Publish(new MatchStartEvent());
        }
        
        if (Keyboard.current != null && Keyboard.current.nKey.wasPressedThisFrame)
        {
            EventBus.Publish(new IntroduceStationEvent("Hongik univ."));
        }
    }

    /// <summary>
    /// When player clicked action buton, this function will be call back.
    /// </summary>
    /// <param name="actionIndex"></param>
    private void OnActionClicked(HandActionType actionIndex, String actionText)
    {
        Debug.Log($"Action clicked: {actionIndex}");
        _viewModel.OnChangeActionIndex(actionIndex, actionText);
        //_viewModel.OnHandAction(actionIndex);
        
        // EventBus.Publish(new ActionSelectedEvent(actionIndex,
        //         _viewModel.IsAttacker.Value ? BattleRole.Attack :  BattleRole.Defense,
        //         SceneDataBridge.playerCamera == CameraType.Camera1 ? Player.First : Player.Second
        //     ));
    }
    private void OnDestroy()
    {
        _viewModel?.Dispose();
    }
}
