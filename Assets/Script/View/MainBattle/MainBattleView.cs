
using System;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

//202322158 이준상


/// <summary>
/// Main battle root view that coordinates UI presenters and ViewModel binding.
/// Before, MainBattleView have so many of code line. and I separated each of domain.
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
            EventBus.Publish(new HitEndAction(SceneDataBridge.playerCamera));
        }

        _uiRefs = new MainBattleUIRefs(mainBattle, perks, toopTip);
        //_dotsRenderer = new MainBattleDotsRenderer(roundItemTemplate);
        //_dotsRenderer.Initialize(_uiRefs.MyRoundWinning, _uiRefs.EnemyRoundWinning);
        _actionRenderer = new MainBattleActionRenderer(actionItemSelect, OnActionClicked);
        
        //I defined a class that sends a callback function when the Observable variables change and manages it.
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
    
    /// <summary>
    /// When player clicked action buton, this function will be call back.
    /// </summary>
    /// <param name="actionIndex"></param>
    private void OnActionClicked(HandActionType actionIndex, String actionText)
    {
        Debug.Log($"Action clicked: {actionIndex}");
        _viewModel.OnChangeActionIndex(actionIndex, actionText);
    }
    
    
    private void OnDestroy()
    {
        _viewModel?.Dispose();
    }
}
