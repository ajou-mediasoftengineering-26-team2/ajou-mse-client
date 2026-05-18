using UnityEngine;
using UnityEngine.UIElements;

//202322158 이준상
public class MainBattleUIRefs
{
    public VisualElement MainBattleRoot { get; }
    public VisualElement PerksRoot { get; }
    public VisualElement TooltipRoot { get; }

    public VisualElement MyRoundWinning { get; }
    public VisualElement EnemyRoundWinning { get; }
    public VisualElement ActionContainer { get; }
    public Label Timer { get; }
    public Label MyName { get;  }
    public Label EnemyName { get;  }
    
    public CameraTurnManager CameraManager { get; }

    public MainBattleUIRefs(UIDocument mainBattle, UIDocument perks, UIDocument tooltip)
    {
        MainBattleRoot = mainBattle.rootVisualElement;
        PerksRoot = perks.rootVisualElement;
        TooltipRoot = tooltip.rootVisualElement;

        MyRoundWinning = MainBattleRoot.Q<VisualElement>("MyRoundContainer");
        EnemyRoundWinning = MainBattleRoot.Q<VisualElement>("EnemyRoundContainer");
        ActionContainer = MainBattleRoot.Q<VisualElement>("ChooseAction");
        Timer = MainBattleRoot.Q<Label>("Time");
        MyName = MainBattleRoot.Q<Label>("MyName");
        EnemyName = MainBattleRoot.Q<Label>("EnemyName");
        
        if (MainBattleRoot == null) Debug.LogError("MainBattle root is null.");
        if (TooltipRoot == null) Debug.LogError("Tooltip root is null.");
    }
}
