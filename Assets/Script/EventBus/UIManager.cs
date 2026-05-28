using Microsoft.Win32.SafeHandles;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] UIDocument PerksAndShopUIDocument;
    [SerializeField] UIDocument MainBattle;
    [SerializeField] UIDocument MatchStart;
    [SerializeField] UIDocument IntroduceStation;
    [SerializeField] UIDocument ChoiceReveal;

    private PlayerInfoModel player1;
    private PlayerInfoModel player2;
    
    private HitAnimation current;
    private void OnEnable()
    {
        PerksAndShopUIDocument.enabled = false;
        MatchStart.enabled = false;
        IntroduceStation.enabled = false;
        ChoiceReveal.enabled = false;
        
        EventBus.Subscribe<RoundOver>(PerksAndShopUIPOP);
        EventBus.Subscribe<HitAnimation>(HitAnimation);
        EventBus.Subscribe<SortHitEvent>(HitUi);
        EventBus.Subscribe<HardHitEvent>(HitUi);
        EventBus.Subscribe<MatchStartEvent>(MatchStartUI);
        EventBus.Subscribe<IntroduceStationEvent>(ShowStationUI);
        EventBus.Subscribe<ChoiceAnimation>(ChoiceAnimation);
    }
    

    private void OnDisable()
    {
        EventBus.Unsubscribe<RoundOver>(PerksAndShopUIPOP);
        EventBus.Unsubscribe<HitAnimation>(HitAnimation);
        EventBus.Unsubscribe<SortHitEvent>(HitUi);
        EventBus.Unsubscribe<HardHitEvent>(HitUi);
        EventBus.Unsubscribe<MatchStartEvent>(MatchStartUI);
        EventBus.Unsubscribe<IntroduceStationEvent>(ShowStationUI);
        EventBus.Unsubscribe<ChoiceAnimation>(ChoiceAnimation);
    }
    
    
    private void HitUi(SortHitEvent obj)
    {
        GetAnimatorByPlayer(current.Player, current.Role);
    }
    
    private void HitUi(HardHitEvent obj)
    {
        GetAnimatorByPlayer(current.Player, current.Role);
    }
    
    private void PerksAndShopUIPOP(RoundOver evt)
    {
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
        MatchStart.enabled = true;
        MatchStart.GetComponent<MatchStartView>().StartAnimation(player1, player2);
    }

    public void ShowStationUI(IntroduceStationEvent evt)
    {
        IntroduceStation.enabled = true;
        var view = IntroduceStation.GetComponent<IntroduceStationView>();
        player1 = evt.player1;
        player2 = evt.player2; 
        if (view != null)
        {
            view.StartAnimation(evt.station);
        }
    }

    private void ChoiceAnimation(ChoiceAnimation evt)
    {
        ChoiceReveal.enabled = true;
        ChoiceReveal.GetComponent<ChoiceRevealView>().StartChoiceReveal();
    }
    
}