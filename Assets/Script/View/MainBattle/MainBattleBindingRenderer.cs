using UnityEngine;
using UnityEngine.UIElements;

//202322158 이준상
public class MainBattleBindingRenderer
{
    private readonly MainBattleViewModel _viewModel;
    private readonly MainBattleUIRefs _uiRefs;
    private readonly MainBattleDotsRenderer _dotsRenderer;
    private readonly MainBattleActionRenderer _actionRenderer;

    public MainBattleBindingRenderer(MainBattleViewModel viewModel,
        MainBattleUIRefs uiRefs,
        MainBattleDotsRenderer dotsRenderer,
        MainBattleActionRenderer actionRenderer)
    {
        _viewModel = viewModel;
        _uiRefs = uiRefs;
        _dotsRenderer = dotsRenderer;
        _actionRenderer = actionRenderer;
    }

    public void Bind()
    {
        BindSlotHover();

        _viewModel.LeftRoundWin.Subscribe(data =>
        {
            _uiRefs.MyScore.text = data.ToString();
        });
        _viewModel.RightRoundWin.Subscribe(data =>
        {
            _uiRefs.EnemyScore.text = data.ToString();
        });

        // _viewModel.StationName.Subscribe(station =>
        // {
        //     var label = _uiRefs.MainBattleRoot.Q<Label>("CurrentStation");
        //     label.text = station;
        // });

        _viewModel.IsAttacker.Subscribe(data =>
        {
            _uiRefs.MyAttack.text = data ? "Attack" :  "Defend";
            _uiRefs.EnemyAttack.text = data ? "Defend" : "Attack";
        });
        _viewModel.HoverTest.Subscribe(test =>
        {
            if (test == null)
            {
                _uiRefs.TooltipRoot.style.display = DisplayStyle.None;
                return;
            }

            _uiRefs.TooltipRoot.Q<Label>("ItemTitle").text = test;
            _uiRefs.TooltipRoot.Q<Label>("ItemDescription").text = "LEE JUN SANG";
            _uiRefs.TooltipRoot.style.display = DisplayStyle.Flex;
        });

        _viewModel.MySelectingE.Subscribe(_ =>
        {
            var indicator = _uiRefs.MainBattleRoot.Q<VisualElement>("TurnIndicator");
            bool isMyTurn = _viewModel.MySelecting.Value;
            indicator.EnableInClassList("my-turn", isMyTurn);
            indicator.EnableInClassList("enemy-turn", !isMyTurn);

            if (isMyTurn)
            {
                _actionRenderer.ShowActions(_uiRefs.ActionContainer, _viewModel.IsAttacker.Value);
            }
            else
            {
                _actionRenderer.HideAllActionOptions();
            }
        });

        _viewModel.LabelState.Subscribe(_ =>
        {
            var label = _uiRefs.MainBattleRoot.Q<Label>("TurnText");
            label.text = _viewModel.LabelState.Value;
        });

        _viewModel.LeftHp.Subscribe(myHp =>
        {
            var hpFill = _uiRefs.MainBattleRoot.Q<VisualElement>("MyHPFill");
            float targetRatio = Mathf.Clamp01((float)myHp / GameSetting.maxHP);
            hpFill.style.width = new Length(targetRatio * 100, LengthUnit.Percent);
        });

        _viewModel.RightHp.Subscribe(enemyHp =>
        {
            var hpFill = _uiRefs.MainBattleRoot.Q<VisualElement>("EnemyHPFill");
            float targetRatio = Mathf.Clamp01((float)enemyHp / GameSetting.maxHP);
            hpFill.style.width = new Length(targetRatio * 100, LengthUnit.Percent);
        });

        _viewModel.CountDown.Subscribe(time => _uiRefs.Timer.text = time);
        
        _viewModel.CameraPoint.Subscribe(camera =>
        {
            if (CameraTurnManager.Instance != null)
            {
                EventBus.Publish(new CameraAction(camera));
            }
        });

        _viewModel.MyName.Subscribe(name =>
        {
            _uiRefs.MyName.text = name;
        });
        
        _viewModel.EnemyName.Subscribe(name =>
        {
            _uiRefs.EnemyName.text = name;
        });
        
        _viewModel.CurrentHandActionText.Subscribe(name =>
        {
            _uiRefs.ActionName.text = "Current Action : " + name.ToString();
        });
        
        _viewModel.MyHandElemental.Subscribe(data =>
        {
            Debug.Log("hand elemental data HANDLED" + data);
            _uiRefs.HandElemental.style.backgroundImage =
                new StyleBackground(Resources.Load<Sprite>(HandInfoProvider.GetImagePath(data)));
        });
        
        
        _viewModel.ItemLists.Subscribe(name =>
        {
            for (int i = 0; i < name.Count; i++)
            {
                var sprite = Resources.Load<Sprite>($"Items/{name[i]}");
                _uiRefs.ItemSlots[i].style.backgroundImage = new StyleBackground(sprite);
            }
        });
    }

    private void BindSlotHover()
    {
        _uiRefs.MainBattleRoot.Query<VisualElement>(className: "slot").ForEach(slot =>
        {
            slot.RegisterCallback<MouseEnterEvent>(evt =>
            {
                _viewModel.HoverTesttest("test");
                _uiRefs.TooltipRoot.style.left = evt.mousePosition.x;
                _uiRefs.TooltipRoot.style.top = evt.mousePosition.y;
            });

            slot.RegisterCallback<MouseLeaveEvent>(_ => _viewModel.HoverTest.Value = null);
        });
    }
}
