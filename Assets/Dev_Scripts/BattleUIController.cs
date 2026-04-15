using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class BattleUIController : MonoBehaviour
{
    //HP바
    private ProgressBar leftPlayerHPBar;
    private ProgressBar rightPlayerHPBar;
    
    //라운드 승리 마커 (LWin_1~3, RWin_1~3)
    private List<VisualElement> leftWinMarkers = new List<VisualElement>();
    private List<VisualElement> rightWinMarkers = new List<VisualElement>();
    
    //타이머와 역이름
    private Label timerLabel;
    private Label stationLabel;

    //특성, 상태이상, 아이템
    private List<VisualElement> itemSlots = new List<VisualElement>();
    private List<VisualElement> perkSlots = new List<VisualElement>();
    private List<VisualElement> statusEffectSlots = new List<VisualElement>();
    //손 속성
    private VisualElement elementalHand;
    
    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        //HP바
        leftPlayerHPBar = root.Q<ProgressBar>("LPlayerHPBar");
        rightPlayerHPBar = root.Q<ProgressBar>("RPlayerHPBar");
        //타이머, 역 이름
        timerLabel = root.Q<Label>("Time");
        stationLabel = root.Q<Label>("CurrentStation");      
        
        //손 속성
        elementalHand = root.Q<VisualElement>("Hand");

        for (int i = 1; i <= 3; i++)
        {
            leftWinMarkers.Add(root.Q<VisualElement>("LWin" + i));
            rightWinMarkers.Add(root.Q<VisualElement>("RWin" + i));
            itemSlots.Add(root.Q<VisualElement>("Item" + i));
            perkSlots.Add(root.Q<VisualElement>("Perk" + i));
        }

        for (int i = 1; i <= 4; i++)
        {
            statusEffectSlots.Add(root.Q<VisualElement>("Effect" + i));
        }
        
        //초기 설정 값?! 더미 데이터
        leftPlayerHPBar.value = 50;
        rightPlayerHPBar.value = 50;
        timerLabel.text = "5";
        stationLabel.text = "City Hall";
    }

    void Update()
    {
        //데이터 받으면 업데이트
    }
    
    //매 턴마다 업데이트 정보
    public void UpdateTurn(int leftHp, int rightHp, int timer)
    {
        leftPlayerHPBar.value = leftHp;
        rightPlayerHPBar.value = rightHp;
        timerLabel.text = timer.ToString();
    }
    
    //라운드 승리 시 순서대로 마커 채우기
    public void SetLeftRoundWin()
    {
        foreach (var m in leftWinMarkers)
        {
            if (m.style.backgroundColor == StyleKeyword.Null)
            {
                m.style.backgroundColor = new StyleColor(Color.forestGreen);
                return;
            }
        }
    }

    public void SetRightRoundWin()
    {
        foreach (var m in rightWinMarkers)
        {
            if (m.style.backgroundColor == StyleKeyword.Null)
            {
                m.style.backgroundColor = new StyleColor(Color.softRed);
                return;
            }
        }
    }

    //매치 시작 시 마커 초기화 -> 이거는 빼도 되고 추가해도 될 거 같습니다..!
    public void ResetRoundMarkers()
    {
        foreach (var m in leftWinMarkers)
            m.style.backgroundColor = StyleKeyword.Null;
        foreach (var m in rightWinMarkers)
            m.style.backgroundColor = StyleKeyword.Null;
    }
    
    //시작 시 역 이름 호출
    public void UpdateStation(string stationName)
    {
        stationLabel.text = stationName;
    }
    
    //아이템 받고 호출 
    public void AddItem(int slotIndex)
    {
        if (slotIndex >= itemSlots.Count)
        {
            return;
        }
        itemSlots[slotIndex].style.opacity = 1.0f;
    }
    
    //아이템 사용 후 
    public void UseItem(int slotIndex)
    {
        if (slotIndex >= itemSlots.Count)
        {
            return;
        }
        itemSlots[slotIndex].style.opacity = 0.3f;
    }

    //특성 선택 시
    public void AddPerk(int slotIndex)
    {
        if (slotIndex >= perkSlots.Count)
        {
            return;
        }
        perkSlots[slotIndex].style.opacity = 1.0f;
    }
    
    //상태이상 걸렸을 시
    public void AddStatusEffect(int slotIndex)
    {
        if (slotIndex >= statusEffectSlots.Count)
        {
            return;
        }
        statusEffectSlots[slotIndex].style.opacity = 1.0f;
    }
    
    //상태이상 해제 시
    public void RemoveStatusEffect(int slotIndex)
    {
        if (slotIndex >= statusEffectSlots.Count)
        {
            return;
        }
        statusEffectSlots[slotIndex].style.opacity = 0.3f;
    }
}
