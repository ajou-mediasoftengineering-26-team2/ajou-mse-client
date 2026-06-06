using System.Collections.Generic;
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
    public Label ActionName { get; }
    
    public CameraTurnManager CameraManager { get; }
    
    public Label MyAttack { get; }
    public Label EnemyAttack { get; }
    public VisualElement LeftHp { get; }
    public VisualElement RightHp { get; }
    
    public Label MyScore { get; }
    public Label EnemyScore { get; }
    
    
    public VisualElement MyHandElemental { get; }
    //item 6 slot
    public VisualElement[] MyPerkSlots { get; } = new VisualElement[3];
    
    public VisualElement[] MyItemSlots { get; } = new VisualElement[3];
    
    
    public VisualElement EnemyHandElemental { get; }
    public VisualElement[] EnemyPerkSlots { get; } = new VisualElement[3];
    public VisualElement[] EnemyItemSlots { get; } = new VisualElement[3];
    
    
    public VisualElement MyEffectContainer { get; }
    public VisualElement EnemyEffectContainer { get; }
    
    public VisualElement ItemIcon;
    public Label ItemTitle;
    public Label ItemDescription;
    

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
        ActionName = MainBattleRoot.Q<Label>("ActionLogText");
        MyAttack = MainBattleRoot.Q<Label>("MyRoleText");
        EnemyAttack = MainBattleRoot.Q<Label>("EnemyRoleText");
        MyScore =  MainBattleRoot.Q<Label>("LeftScore");
        EnemyScore = MainBattleRoot.Q<Label>("RightScore");
        
        MyHandElemental = MainBattleRoot.Q<VisualElement>("Profile");
        EnemyHandElemental = MainBattleRoot.Q<VisualElement>("EnemyProfile");
        LeftHp = MainBattleRoot.Q<VisualElement>("LeftHp");
        RightHp = MainBattleRoot.Q<VisualElement>("RightHp");
        if (MainBattleRoot == null) Debug.LogError("MainBattle root is null.");
        if (TooltipRoot == null) Debug.LogError("Tooltip root is null.");
        
        
        ItemIcon = TooltipRoot.Q<VisualElement>("ItemIcon");
        ItemTitle = TooltipRoot.Q<Label>("ItemTitle");
        ItemDescription = TooltipRoot.Q<Label>("ItemDescription");
        
        MyEffectContainer = MainBattleRoot.Q<VisualElement>("StatusEffectContainer");
        EnemyEffectContainer = MainBattleRoot.Q<VisualElement>("EnemyStatusEffectContainer");
        
        //item slot parsing
        VisualElement infoGrid = MainBattleRoot.Q<VisualElement>("InfoGrid");
        if (infoGrid != null)
        {
            // InfoGrid 내부에 있는 두 개의 'grid-row' 클래스를 순서대로 가져옵니다.
            List<VisualElement> rows = infoGrid.Query<VisualElement>(className: "grid-row").ToList();

            if (rows.Count >= 2)
            {
                // 1. 첫 번째 줄 (위쪽 - 퍽) 내부의 slot 3개 파싱
                List<VisualElement> perkElements = rows[0].Query<VisualElement>(className: "slot").ToList();
                for (int i = 0; i < MyPerkSlots.Length && i < perkElements.Count; i++)
                {
                    MyPerkSlots[i] = perkElements[i];
                }

                // 2. 두 번째 줄 (아래쪽 - 아이템) 내부의 slot 3개 파싱
                List<VisualElement> itemElements = rows[1].Query<VisualElement>(className: "slot-item").ToList();
                for (int i = 0; i < MyItemSlots.Length && i < itemElements.Count; i++)
                {
                    MyItemSlots[i] = itemElements[i];
                }
            }
        }
        
        VisualElement enemyInfoGrid = MainBattleRoot.Q<VisualElement>("EnemyInfoGrid");
        if (enemyInfoGrid != null)
        {
            // EnemyInfoGrid 내부에 있는 두 개의 'grid-row' 클래스를 순서대로 가져옵니다.
            List<VisualElement> enemyRows = enemyInfoGrid.Query<VisualElement>(className: "grid-row").ToList();

            if (enemyRows.Count >= 2)
            {
                // 1. 첫 번째 줄 (위쪽 - 적 퍽) 내부의 slot 3개 파싱
                List<VisualElement> enemyPerkElements = enemyRows[0].Query<VisualElement>(className: "slot").ToList();
                for (int i = 0; i < EnemyPerkSlots.Length && i < enemyPerkElements.Count; i++)
                {
                    EnemyPerkSlots[i] = enemyPerkElements[i];
                }

                // 2. 두 번째 줄 (아래쪽 - 적 아이템) 내부의 slot 3개 파싱
                List<VisualElement> enemyItemElements = enemyRows[1].Query<VisualElement>(className: "slot-item").ToList();
                for (int i = 0; i < EnemyItemSlots.Length && i < enemyItemElements.Count; i++)
                {
                    EnemyItemSlots[i] = enemyItemElements[i];
                }
            }
        }
        
        
    }
}
