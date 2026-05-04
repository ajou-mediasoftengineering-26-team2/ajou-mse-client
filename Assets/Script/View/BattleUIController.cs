using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleUIController : MonoBehaviour
{
    private MainBattleViewModel _viewModel;

    // HP
    private ProgressBar _lPlayerHPBar;
    private ProgressBar _rPlayerHPBar;

    // 타이머 / 역 이름
    private Label _timeLabel;
    private Label _currentStationLabel;

    // 라운드 승리 마커
    private VisualElement _lWin1, _lWin2, _lWin3;
    private VisualElement _rWin1, _rWin2, _rWin3;

    // 상태이상 슬롯
    private VisualElement _effect1, _effect2, _effect3, _effect4;

    // 아이템 슬롯
    private VisualElement _item1, _item2, _item3;

    // 퍽 슬롯
    private VisualElement _perk1, _perk2, _perk3;

    // 손 속성 슬롯
    private VisualElement _hand;

    // 돈
    private Label _moneyCountLabel;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // ── UXML 바인딩 ──────────────────────────────────────────────
        _lPlayerHPBar        = root.Q<ProgressBar>("LPlayerHPBar");
        _rPlayerHPBar        = root.Q<ProgressBar>("RPlayerHPBar");
        _timeLabel           = root.Q<Label>("Time");
        _currentStationLabel = root.Q<Label>("CurrentStation");
        _moneyCountLabel     = root.Q<Label>("MoneyCount");

        _lWin1 = root.Q<VisualElement>("LWin1");
        _lWin2 = root.Q<VisualElement>("LWin2");
        _lWin3 = root.Q<VisualElement>("LWin3");
        _rWin1 = root.Q<VisualElement>("RWin1");
        _rWin2 = root.Q<VisualElement>("RWin2");
        _rWin3 = root.Q<VisualElement>("RWin3");

        _effect1 = root.Q<VisualElement>("Effect1");
        _effect2 = root.Q<VisualElement>("Effect2");
        _effect3 = root.Q<VisualElement>("Effect3");
        _effect4 = root.Q<VisualElement>("Effect4");

        _item1 = root.Q<VisualElement>("Item1");
        _item2 = root.Q<VisualElement>("Item2");
        _item3 = root.Q<VisualElement>("Item3");

        _perk1 = root.Q<VisualElement>("Perk1");
        _perk2 = root.Q<VisualElement>("Perk2");
        _perk3 = root.Q<VisualElement>("Perk3");

        _hand = root.Q<VisualElement>("Hand");

        // ── ViewModel 생성 ───────────────────────────────────────────
        var idViewModel = ViewModelLocator.Instance.Get<IDViewModel>();
        _viewModel = new MainBattleViewModel(idViewModel.PlayerId.Value, idViewModel.LobbyId.Value);
        _viewModel.Initialize();

        // ── Observable 구독 ──────────────────────────────────────────

        // HP
        _viewModel.LeftHp.Subscribe(hp  => _lPlayerHPBar.value = hp);
        _viewModel.RightHp.Subscribe(hp => _rPlayerHPBar.value = hp);

        // 라운드 승리 마커
        _viewModel.LeftWin1.Subscribe(active  => _lWin1.style.opacity = active ? 1f : 0f);
        _viewModel.LeftWin2.Subscribe(active  => _lWin2.style.opacity = active ? 1f : 0f);
        _viewModel.LeftWin3.Subscribe(active  => _lWin3.style.opacity = active ? 1f : 0f);
        _viewModel.RightWin1.Subscribe(active => _rWin1.style.opacity = active ? 1f : 0f);
        _viewModel.RightWin2.Subscribe(active => _rWin2.style.opacity = active ? 1f : 0f);
        _viewModel.RightWin3.Subscribe(active => _rWin3.style.opacity = active ? 1f : 0f);

        // 타이머
        _viewModel.RemainingSeconds.Subscribe(sec => _timeLabel.text = sec.ToString());

        // 역 이름
        _viewModel.StationName.Subscribe(name => _currentStationLabel.text = name ?? "");

        // 돈
        _viewModel.Money.Subscribe(money => _moneyCountLabel.text = $"Coin : {money}");

        // 아이템 슬롯
        _viewModel.Item1Active.Subscribe(active => _item1.style.opacity = active ? 1f : 0.3f);
        _viewModel.Item2Active.Subscribe(active => _item2.style.opacity = active ? 1f : 0.3f);
        _viewModel.Item3Active.Subscribe(active => _item3.style.opacity = active ? 1f : 0.3f);

        // 퍽 슬롯
        _viewModel.Perk1Active.Subscribe(active => _perk1.style.opacity = active ? 1f : 0.3f);
        _viewModel.Perk2Active.Subscribe(active => _perk2.style.opacity = active ? 1f : 0.3f);
        _viewModel.Perk3Active.Subscribe(active => _perk3.style.opacity = active ? 1f : 0.3f);

        // 상태이상 슬롯
        _viewModel.Effect1Active.Subscribe(active => _effect1.style.opacity = active ? 1f : 0.3f);
        _viewModel.Effect2Active.Subscribe(active => _effect2.style.opacity = active ? 1f : 0.3f);
        _viewModel.Effect3Active.Subscribe(active => _effect3.style.opacity = active ? 1f : 0.3f);
        _viewModel.Effect4Active.Subscribe(active => _effect4.style.opacity = active ? 1f : 0.3f);

        // ── 손 클릭 입력 ─────────────────────────────────────────────
        // TODO: 액션 선택 방식 확정 후 choice 값 연결
        _hand.RegisterCallback<PointerDownEvent>(evt =>
        {
            // _viewModel.OnHandAction("LEFT");
        });
    }

    // ── 정리 ─────────────────────────────────────────────────────────
    private void OnDestroy()
    {
        _viewModel?.Dispose();
    }
}