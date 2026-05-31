public class MainBattleEventManager
{
    private readonly MainBattleViewModel _viewModel;
    private readonly MainBattleUIRefs _uiRefs;
    private readonly MainBattleDotsRenderer _dotsRenderer;
    private readonly MainBattleActionRenderer _actionRenderer;
    public MainBattleEventManager(
        MainBattleViewModel viewModel, 
        MainBattleUIRefs uiRefs, 
        MainBattleDotsRenderer dotsRenderer, 
        MainBattleActionRenderer actionRenderer)
    {
        _viewModel = viewModel;
        _uiRefs = uiRefs;
        _dotsRenderer = dotsRenderer;
        _actionRenderer = actionRenderer;
    }
}