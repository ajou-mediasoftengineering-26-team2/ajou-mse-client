using System.Collections;
using Microsoft.Win32.SafeHandles;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    //[SerializeField] UIDocument PerksAndShopUIDocument;
    [SerializeField] UIDocument MainBattle;
    [SerializeField] UIDocument MatchStart;
    [SerializeField] UIDocument ItemUI;
    [SerializeField] UIDocument RoundResultUI;
    [SerializeField] UIDocument IntroduceStation;
    [SerializeField] UIDocument ChoiceReveal;
    [SerializeField] UIDocument ElementalHandChoice;
    //[SerializeField] UIDocument RoundOver;
    [SerializeField] UIDocument PerksAndShop;
    [SerializeField] UIDocument GameEndUI;
    
    [SerializeField] public GameObject perkCardPrefab;
    [SerializeField] public GameObject itemCardPrefab;
    public Transform cardSpawnPoint;

    

    private PlayerInfoModel player1;
    private PlayerInfoModel player2;
    
    private HitAnimation current;
    private void OnEnable()
    {
        // 개별적으로 끄던 코드를 AllUIDown 하나로 대체
        AllUIDown();
        
        //EventBus.Subscribe<RoundOver>(RoundOverUI);
        EventBus.Subscribe<HitAnimation>(HitAnimation);
        EventBus.Subscribe<SortHitEvent>(HitUi);
        EventBus.Subscribe<HardHitEvent>(HitUi);
        EventBus.Subscribe<MatchStartEvent>(MatchStartUI);
        EventBus.Subscribe<ItemReceivedEvent>(ShowItemUI);
        EventBus.Subscribe<RoundResultEvent>(ShowRoundResultUI);
        EventBus.Subscribe<IntroduceStationEvent>(ShowStationUI);
        EventBus.Subscribe<ChoiceAnimation>(ChoiceAnimation);
        EventBus.Subscribe<HandElementalChoice>(HandElementalChoice);
        EventBus.Subscribe<HandElementalChoiceResult>(FinishAnimation);
        EventBus.Subscribe<PerkChoiceEvent>(PerksAndShopUIPOP);
        EventBus.Subscribe<GameEndEvent>(ShowGameEndUI);
        EventBus.Subscribe<HitDamageEvent>(OnHitDamageReceived);
    }

    private void PerksAndShopUIPOP(PerkChoiceEvent obj)
    {
        AllUIDown();
        PerksAndShop.enabled = true;
    }

    private void FinishAnimation(HandElementalChoiceResult obj)
    {
        AllUIDown();
        
        IntroduceStation.enabled = true;
    }


    private void OnDisable()
    {
        //EventBus.Unsubscribe<RoundOver>(RoundOverUI);
        EventBus.Unsubscribe<HitAnimation>(HitAnimation);
        EventBus.Unsubscribe<SortHitEvent>(HitUi);
        EventBus.Unsubscribe<HardHitEvent>(HitUi);
        EventBus.Unsubscribe<MatchStartEvent>(MatchStartUI);
        EventBus.Unsubscribe<ItemReceivedEvent>(ShowItemUI);
        EventBus.Unsubscribe<RoundResultEvent>(ShowRoundResultUI);
        EventBus.Unsubscribe<IntroduceStationEvent>(ShowStationUI);
        EventBus.Unsubscribe<ChoiceAnimation>(ChoiceAnimation);
        EventBus.Unsubscribe<HandElementalChoice>(HandElementalChoice);
        EventBus.Unsubscribe<HandElementalChoiceResult>(FinishAnimation);
        EventBus.Unsubscribe<PerkChoiceEvent>(PerksAndShopUIPOP);
        EventBus.Unsubscribe<GameEndEvent>(ShowGameEndUI);
        EventBus.Unsubscribe<HitDamageEvent>(OnHitDamageReceived);
    }


    private void HandElementalChoice(HandElementalChoice evt)
    {
        AllUIDown(); // UI 켜기 전에 모두 끄기 추가
        ElementalHandChoice.enabled = true;
        ElementalHandChoice.GetComponent<SelectHandsView>().StartScene();
    }
    
    private void HitUi(SortHitEvent obj)
    {
        //GetAnimatorByPlayer(current.Player, current.Role);
    }
    
    private void HitUi(HardHitEvent obj)
    {
        //GetAnimatorByPlayer(current.Player, current.Role);
    }

    /*
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
        PerksAndShop.enabled = false;
    }
    */

    private void HitAnimation(HitAnimation evt)
    {
        Debug.Log("hit animation" + "    " + evt.Player);
        current = evt;
    }
    
    // private void GetAnimatorByPlayer(Player player, BattleRole role)
    // {
    //     switch (player, role)
    //     {
    //         // 1. First(왼쪽)가 공격하는 상황 -> 당연히 Second(오른쪽)가 맞으므로 오른쪽 팝업!
    //         case (Player.First, BattleRole.Attack):
    //             Toast.ShowDamagePopupLeft(2);
    //             break;
    //         // 2. First(왼쪽)가 수비(피격)하는 상황 -> 내가 맞았으므로 내 위치(왼쪽)에 팝업!
    //         case (Player.First, BattleRole.Defense):
    //             Toast.ShowDamagePopupRight(2);
    //             break;
    //         // 3. Second(오른쪽)가 공격하는 상황 -> First(왼쪽)가 맞으므로 왼쪽 팝업!
    //         case (Player.Second, BattleRole.Attack):
    //             Toast.ShowDamagePopupRight(2);
    //             break;
    //         // 4. Second(오른쪽)가 수비(피격)하는 상황 -> 내가 맞았으므로 내 위치(오른쪽)에 팝업!
    //         case (Player.Second, BattleRole.Defense):
    //             Toast.ShowDamagePopupLeft(2);
    //             break;
    //     }
    // }
    
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
        if (!ItemUI.enabled)
            ItemUI.enabled = true;
        ItemUI.GetComponent<ItemView>().ShowItem(evt.ItemCode);
    }
    
    private void ShowRoundResultUI(RoundResultEvent evt)
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

        AllUIDown();
        RoundResultUI.enabled = true;
        view.ShowResult(evt);
    }

    private void ChoiceAnimation(ChoiceAnimation evt)
    {
        AllUIDown(); // UI 켜기 전에 모두 끄기 추가
        ChoiceReveal.enabled = true;
        ChoiceReveal.GetComponent<ChoiceRevealView>().StartChoiceReveal(evt.Player1, evt.Player2);
    }
    
    private void ShowGameEndUI(GameEndEvent evt)
    {
        if (GameEndUI == null)
        {
            Debug.LogError("[UIManager] GameEnd UI document is not set.");
            return;
        }

        var view = GameEndUI.GetComponent<GameEndView>();
        if (view == null)
        {
            Debug.LogError("[UIManager] GameEndView component is missing.");
            return;
        }

        AllUIDown();
        GameEndUI.enabled = true;
        view.ShowResult(evt);
    }

    private void OnHitDamageReceived(HitDamageEvent e)
    {
        // 🔥 일반 함수에서는 딜레이를 줄 수 없으므로, 코루틴을 구동합니다.
        StartCoroutine(SpawnCardsSequential(e.damage, e.isLeft));
    }
    
    private IEnumerator SpawnCardsSequential(Damage damageData, bool isLeft)
    {
        // 카드와 카드 사이의 등장 간격 (원하는 초 단위로 조절하세요)
        float delayTime = 0.3f; 

        // 1. 사용된 퍽(Perk) 리스트 처리
        if (damageData.usedPerks != null)
        {
            foreach (string perkStr in damageData.usedPerks)
            {
                if (System.Enum.TryParse(perkStr, out PerkType perkType))
                {
                    SpawnNewCard(perkType, isLeft);
                
                    // 🔥 카드를 하나 만들고 설정한 시간만큼 대기합니다.
                    yield return new WaitForSeconds(delayTime); 
                }
                else
                {
                    Debug.LogWarning($"[UI Manager] 알 수 없는 퍽 이름입니다: {perkStr}");
                }
            }
        }

        // 2. 사용된 아이템(Item) 리스트 처리
        if (damageData.usedItems != null)
        {
            foreach (string itemStr in damageData.usedItems)
            {
                if (System.Enum.TryParse(itemStr, out ItemType itemType))
                {
                    SpawnItemCard(itemType, isLeft);
                
                    // 🔥 아이템 카드를 하나 만들고 설정한 시간만큼 대기합니다.
                    yield return new WaitForSeconds(delayTime); 
                }
                else
                {
                    Debug.LogWarning($"[UI Manager] 알 수 없는 아이템 이름입니다: {itemStr}");
                }
            }
        }
    }
    void SpawnNewCard(PerkType perkType, bool isLeft)
    {
        GameObject newCardObj = Instantiate(perkCardPrefab);

        PerkCard cardScript = newCardObj.GetComponent<PerkCard>();

        if (cardScript != null)
        {
            cardScript.SetUpAndAnimationCard(perkType, isLeft);
        }
    }

    void SpawnItemCard(ItemType itemType, bool isLeft)
    {
        GameObject newCardObj = Instantiate(perkCardPrefab);
        ItemCard itemCardScript = newCardObj.GetComponent<ItemCard>();
        
        itemCardScript.SetUpAndAnimationCard(itemType, isLeft);
    }
    
    /// <summary>
    /// 상단에 선언된 모든 10개의 UIDocument를 안전하게 비활성화합니다.
    /// </summary>
    private void AllUIDown()
    {
        //if (PerksAndShopUIDocument != null) PerksAndShopUIDocument.enabled = false;
        //if (MainBattle != null) MainBattle.enabled = false;
        if (MatchStart != null) MatchStart.enabled = false;
        if (ItemUI != null) ItemUI.enabled = false;
        if (RoundResultUI != null) RoundResultUI.enabled = false;
        if (IntroduceStation != null) IntroduceStation.enabled = false;
        if (ChoiceReveal != null) ChoiceReveal.enabled = false;
        if (ElementalHandChoice != null) ElementalHandChoice.enabled = false;
        //if (RoundOver != null) RoundOver.enabled = false;
        if (PerksAndShop != null) PerksAndShop.enabled = false;
        if (GameEndUI != null) GameEndUI.enabled = false;
    }
}