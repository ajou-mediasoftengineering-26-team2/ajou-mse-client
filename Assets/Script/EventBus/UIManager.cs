using Microsoft.Win32.SafeHandles;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] UIDocument PerksAndShopUIDocument;
    [SerializeField] UIDocument MainBattle;
    [SerializeField] UIDocument MatchStart;
    [SerializeField] UIDocument ItemUI;
    [SerializeField] UIDocument RoundResultUI;
    [SerializeField] UIDocument IntroduceStation;
    [SerializeField] UIDocument ChoiceReveal;
    [SerializeField] UIDocument ElementalHandChoice;
    [SerializeField] UIDocument RoundOver;
    [SerializeField] UIDocument PerksAndShop;

    private PlayerInfoModel player1;
    private PlayerInfoModel player2;
    
    private HitAnimation current;
    private void OnEnable()
    {
        // 개별적으로 끄던 코드를 AllUIDown 하나로 대체
        AllUIDown();
        
        EventBus.Subscribe<RoundOver>(RoundOverUI);
        EventBus.Subscribe<HitAnimation>(HitAnimation);
        EventBus.Subscribe<SortHitEvent>(HitUi);
        EventBus.Subscribe<HardHitEvent>(HitUi);
        EventBus.Subscribe<MatchStartEvent>(MatchStartUI);
        EventBus.Subscribe<ItemReceivedEvent>(ShowItemUI);
        EventBus.Subscribe<RoundOver>(ShowRoundResultUI);
        EventBus.Subscribe<IntroduceStationEvent>(ShowStationUI);
        EventBus.Subscribe<ChoiceAnimation>(ChoiceAnimation);
        EventBus.Subscribe<HandElementalChoice>(HandElementalChoice);
        EventBus.Subscribe<HandElementalChoiceResult>(FinishAnimation);
        EventBus.Subscribe<PerksAndItemReceiveEvent>(PerksAndShopUIPOP);
    }

    private void PerksAndShopUIPOP(PerksAndItemReceiveEvent obj)
    {
        
    }

    private void FinishAnimation(HandElementalChoiceResult obj)
    {
        AllUIDown();
        
        IntroduceStation.enabled = true;
    }


    private void OnDisable()
    {
        EventBus.Unsubscribe<RoundOver>(RoundOverUI);
        EventBus.Unsubscribe<HitAnimation>(HitAnimation);
        EventBus.Unsubscribe<SortHitEvent>(HitUi);
        EventBus.Unsubscribe<HardHitEvent>(HitUi);
        EventBus.Unsubscribe<MatchStartEvent>(MatchStartUI);
        EventBus.Unsubscribe<ItemReceivedEvent>(ShowItemUI);
        EventBus.Unsubscribe<RoundOver>(ShowRoundResultUI);
        EventBus.Unsubscribe<IntroduceStationEvent>(ShowStationUI);
        EventBus.Unsubscribe<ChoiceAnimation>(ChoiceAnimation);
        EventBus.Unsubscribe<HandElementalChoice>(HandElementalChoice);
        EventBus.Unsubscribe<HandElementalChoiceResult>(FinishAnimation);
        EventBus.Unsubscribe<PerksAndItemReceiveEvent>(PerksAndShopUIPOP);
    }


    private void HandElementalChoice(HandElementalChoice evt)
    {
        AllUIDown(); // UI 켜기 전에 모두 끄기 추가
        ElementalHandChoice.enabled = true;
    }
    
    private void HitUi(SortHitEvent obj)
    {
        GetAnimatorByPlayer(current.Player, current.Role);
    }
    
    private void HitUi(HardHitEvent obj)
    {
        GetAnimatorByPlayer(current.Player, current.Role);
    }

    private void RoundOverUI(RoundOver evt)
    {
        if (RoundOver == null)
        {
            Debug.LogError("[UIManager] RoundOver UI document is not set.");
            return;
        }
        AllUIDown(); // UI 켜기 전에 모두 끄기 추가
        RoundOver.enabled = true;
    }
    private void PerksAndShopUIPOP(RoundOver evt)
    {
        AllUIDown(); // UI 켜기 전에 모두 끄기 추가
        PerksAndShopUIDocument.enabled = true;
    }

    private void PerksAndShopUIDown(RoundOver evt)
    {
        PerksAndShopUIDocument.enabled = false;
    }


    private void HitAnimation(HitAnimation evt)
    {
        Debug.Log("hit animation" + "    " + evt.Player);
        current = evt;
    }
    
    private void GetAnimatorByPlayer(Player player, BattleRole role)
    {
        switch (player, role)
        {
            // 1. First(왼쪽)가 공격하는 상황 -> 당연히 Second(오른쪽)가 맞으므로 오른쪽 팝업!
            case (Player.First, BattleRole.Attack):
                Toast.ShowDamagePopupLeft(2);
                break;
            // 2. First(왼쪽)가 수비(피격)하는 상황 -> 내가 맞았으므로 내 위치(왼쪽)에 팝업!
            case (Player.First, BattleRole.Defense):
                Toast.ShowDamagePopupRight(2);
                break;
            // 3. Second(오른쪽)가 공격하는 상황 -> First(왼쪽)가 맞으므로 왼쪽 팝업!
            case (Player.Second, BattleRole.Attack):
                Toast.ShowDamagePopupRight(2);
                break;
            // 4. Second(오른쪽)가 수비(피격)하는 상황 -> 내가 맞았으므로 내 위치(오른쪽)에 팝업!
            case (Player.Second, BattleRole.Defense):
                Toast.ShowDamagePopupLeft(2);
                break;
        }
    }
    
    private void MatchStartUI(MatchStartEvent evt)
    {
        AllUIDown(); // UI 켜기 전에 모두 끄기 추가
        MatchStart.enabled = true;
        MatchStart.GetComponent<MatchStartView>().StartAnimation(player1, player2);
    }

    public void ShowStationUI(IntroduceStationEvent evt)
    {
        AllUIDown(); // UI 켜기 전에 모두 끄기 추가
        IntroduceStation.enabled = true;
        var view = IntroduceStation.GetComponent<IntroduceStationView>();
        player1 = evt.player1;
        player2 = evt.player2; 
        if (view != null)
        {
            view.StartAnimation(evt.station);
        }
    }
    
    private void ShowItemUI(ItemReceivedEvent evt)
    {
        AllUIDown(); // (기존 코드 유지) UI 켜기 전에 모두 끄기
        ItemUI.enabled = true;
        ItemUI.GetComponent<ItemView>().ShowItem(evt.ItemCode);
    }
    
    private void ShowRoundResultUI(RoundOver evt)
    {
        if (RoundResultUI == null)
        {
            Debug.LogError("[UIManager] RoundResult UI document is not set.");
            return;
        }

        var view = RoundResultUI.GetComponent<RoundResultView>();
        if (view == null)
        {
            Debug.LogError("[UIManager] RoundResultView component is missing.");
            return;
        }

        AllUIDown(); // UI 켜기 전에 모두 끄기 추가
        RoundResultUI.enabled = true;
        view.ShowResult(evt.isWin);
    }

    private void ChoiceAnimation(ChoiceAnimation evt)
    {
        AllUIDown(); // UI 켜기 전에 모두 끄기 추가
        ChoiceReveal.enabled = true;
        ChoiceReveal.GetComponent<ChoiceRevealView>().StartChoiceReveal(evt.Player1, evt.Player2);
    }

    /// <summary>
    /// 상단에 선언된 모든 10개의 UIDocument를 안전하게 비활성화합니다.
    /// </summary>
    private void AllUIDown()
    {
        if (PerksAndShopUIDocument != null) PerksAndShopUIDocument.enabled = false;
        //if (MainBattle != null) MainBattle.enabled = false;
        if (MatchStart != null) MatchStart.enabled = false;
        if (ItemUI != null) ItemUI.enabled = false;
        if (RoundResultUI != null) RoundResultUI.enabled = false;
        if (IntroduceStation != null) IntroduceStation.enabled = false;
        if (ChoiceReveal != null) ChoiceReveal.enabled = false;
        if (ElementalHandChoice != null) ElementalHandChoice.enabled = false;
        if (RoundOver != null) RoundOver.enabled = false;
        if (PerksAndShop != null) PerksAndShop.enabled = false;
    }
}