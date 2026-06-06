using Mono.Cecil;
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
        _viewModel.HoverItem.Subscribe(data =>
        {
            if (data == null)
            {
                _uiRefs.TooltipRoot.style.display = DisplayStyle.None;
                return;
            }

            _uiRefs.ItemTitle.text = _viewModel.HoverItemTitle.Value;
            _uiRefs.ItemDescription.text = _viewModel.HoverItemDes.Value;
            _uiRefs.TooltipRoot.style.display = DisplayStyle.Flex;
            var sprite = Resources.Load<Sprite>($"Items/{data}");
            _uiRefs.ItemIcon.style.backgroundImage = new StyleBackground(sprite);
        });
        
        _viewModel.HoverPerk.Subscribe(data =>
        {
            if (data == null)
            {
                _uiRefs.TooltipRoot.style.display = DisplayStyle.None;
                return;
            }

            _uiRefs.ItemTitle.text = _viewModel.HoverPerkTitle.Value;
            _uiRefs.ItemDescription.text = _viewModel.HoverPerkDes.Value;
            _uiRefs.TooltipRoot.style.display = DisplayStyle.Flex;
            var sprite = Resources.Load<Sprite>($"Perks/{data}");
            _uiRefs.ItemIcon.style.backgroundImage = new StyleBackground(sprite);
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
                EventBus.Publish(new HitEndAction(camera));
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
            _uiRefs.MyHandElemental.style.backgroundImage =
                new StyleBackground(Resources.Load<Sprite>(HandInfoProvider.GetImagePath(data)));
        });
        
        _viewModel.EnemyHandElemental.Subscribe(data =>
        {
            _uiRefs.EnemyHandElemental.style.backgroundImage =
                new  StyleBackground(Resources.Load<Sprite>(HandInfoProvider.GetImagePath(data)));
        });
        
        _viewModel.EnemyItemLists.Subscribe(data =>
        {
            if (data == null) return;
            for (int i = 0; i < data.Count; i++)
            {
                var sprite = Resources.Load<Sprite>($"Items/{data[i]}");
                Debug.Log("sprite : " + sprite.name);
                _uiRefs.EnemyItemSlots[i].style.backgroundImage = new StyleBackground(sprite);
            }
        });
        _viewModel.ItemLists.Subscribe(data =>
        {
            if (data == null) return;
            for (int i = 0; i < data.Count; i++)
            {
                var sprite = Resources.Load<Sprite>($"Items/{data[i]}");
                _uiRefs.MyItemSlots[i].style.backgroundImage = new StyleBackground(sprite);
            }
        });
        
        _viewModel.EnemyPerkList.Subscribe(data =>
        {
            if (data == null) return;
            for (int i = 0; i < data.Count; i++)
            {
                var sprite = Resources.Load<Sprite>($"Perks/{data[i]}");
                _uiRefs.EnemyPerkSlots[i].style.backgroundImage = new StyleBackground(sprite);
            }
        });
        
        _viewModel.MyPerkList.Subscribe(data =>
        {
            if (data == null) return;
            for (int i = 0; i < data.Count; i++)
            {
                var sprite = Resources.Load<Sprite>($"Perks/{data[i]}");
                _uiRefs.MyPerkSlots[i].style.backgroundImage = new StyleBackground(sprite);
            }
        });
        
        _viewModel.MyStatusList.Subscribe(data =>
        {
            ClearAllStatusEffects(false);
            if (data == null) return;

            for (int i = 0; i < data.Count; i++)
            {
                var sprite = Resources.Load<Sprite>($"Condition/{data[i]}");
                ApplyStatusEffect(sprite , data[i].ToString(), true);
            }
        });
        
        _viewModel.EnemyStatusList.Subscribe(data =>
        {
            ClearAllStatusEffects(false);
            if (data == null) return;

            for (int i = 0; i < data.Count; i++)
            {
                var sprite = Resources.Load<Sprite>($"Condition/{data[i]}");
                ApplyStatusEffect(sprite , data[i].ToString(), false);
            }
        });
    }

    private void BindSlotHover()
    {
        _uiRefs.TooltipRoot.style.position = Position.Absolute;
        _uiRefs.MainBattleRoot.Query<VisualElement>(className: "slot").ForEach(slot =>
        {
            Debug.Log("slot에 log가 뜸");
            slot.RegisterCallback<MouseEnterEvent>(evt =>
            {

                // 1. 마우스가 올라간 현재 슬롯 요소를 가져옵니다.
                var currentSlot = evt.currentTarget as VisualElement;
                if (currentSlot == null) return;

                // 2. 슬롯의 전역(World) 좌표와 크기를 가져옵니다.
                Rect slotBounds = currentSlot.worldBound;

                
                string imageName = "None";
                
                var bg = currentSlot.resolvedStyle.backgroundImage; 

                // 2. 배경 이미지가 비어있지 않은지(Null이 아닌지) 확인합니다.
                if (bg.texture != null)
                {
                    // Texture2D 형식으로 등록되어 있는 경우 에셋 이름을 가져옵니다.
                    imageName = bg.texture.name;
                }
                else if (bg.sprite != null)
                {
                    // Sprite 형식으로 등록되어 있는 경우 에셋 이름을 가져옵니다.
                    imageName = bg.sprite.name;
                }
                
                Debug.Log($"현재 호버된 슬롯의 이미지 이름: {imageName}");


                if (!imageName.Equals("None"))
                {
                    
                    _viewModel.HoverEventPerk(imageName);
                }
    
                bool isEnemySlot = currentSlot.parent?.parent?.name == "EnemyInfoGrid";

                float spacing = 10f; // 슬롯과 툴팁 사이의 간격


                if (!isEnemySlot)
                {
                    _uiRefs.TooltipRoot.style.left = slotBounds.x; 
                    _uiRefs.TooltipRoot.style.top = slotBounds.y - 150f - spacing; // 150f는 툴팁 예상 높이 (상황에 따라 조절)
                }
                else
                {
                    float tooltipWidth = 550f; 
                    _uiRefs.TooltipRoot.style.left = slotBounds.xMax - tooltipWidth; 
                    _uiRefs.TooltipRoot.style.top = slotBounds.y - 150f - spacing; // 똑같이 마이너스(-) 처리
                }
    
            });

            slot.RegisterCallback<MouseLeaveEvent>(_ => _viewModel.HoverPerk.Value = null);
        });
        
        
        _uiRefs.MainBattleRoot.Query<VisualElement>(className: "slot-item").ForEach(slot =>
        {
            Debug.Log("slot에 log가 뜸");
            slot.RegisterCallback<MouseEnterEvent>(evt =>
            {

                var currentSlot = evt.currentTarget as VisualElement;
                if (currentSlot == null) return;

                Rect slotBounds = currentSlot.worldBound;

                
                string imageName = "None";
                
                var bg = currentSlot.resolvedStyle.backgroundImage; 

                // 2. 배경 이미지가 비어있지 않은지(Null이 아닌지) 확인합니다.
                if (bg.texture != null)
                {
                    // Texture2D 형식으로 등록되어 있는 경우 에셋 이름을 가져옵니다.
                    imageName = bg.texture.name;
                }
                else if (bg.sprite != null)
                {
                    // Sprite 형식으로 등록되어 있는 경우 에셋 이름을 가져옵니다.
                    imageName = bg.sprite.name;
                }
                
                Debug.Log($"현재 호버된 슬롯의 이미지 이름: {imageName}");

                if (!imageName.Equals("None"))
                {
                    _viewModel.HoverEventItem(imageName);
                }
    
                bool isEnemySlot = currentSlot.parent?.parent?.name == "EnemyInfoGrid";

                float spacing = 10f; // 슬롯과 툴팁 사이의 간격


                if (!isEnemySlot)
                {
                    _uiRefs.TooltipRoot.style.left = slotBounds.x; 
                    _uiRefs.TooltipRoot.style.top = slotBounds.y - 150f - spacing; // 150f는 툴팁 예상 높이 (상황에 따라 조절)
                }
                else
                {
                    float tooltipWidth = 550f; 
                    _uiRefs.TooltipRoot.style.left = slotBounds.xMax - tooltipWidth; 
                    _uiRefs.TooltipRoot.style.top = slotBounds.y - 150f - spacing; // 똑같이 마이너스(-) 처리
                }
    
            });

            slot.RegisterCallback<MouseLeaveEvent>(_ => _viewModel.HoverItem.Value = null);
        });
    }
    
    public void ApplyStatusEffect(Sprite effectIcon, string effectName, bool isMy)
    {
        // 1.isMy 값에 따라 어떤 컨테이너를 쓸지 결정합니다.
        VisualElement targetContainer = isMy ? _uiRefs.MyEffectContainer : _uiRefs.EnemyEffectContainer;
        
        if (targetContainer == null)
        {
            Debug.LogWarning($"{(isMy ? "내" : "적")} 상태이상 컨테이너를 찾을 수 없습니다.");
            return;
        }

        if (targetContainer.Q<VisualElement>(effectName) != null) return;

        VisualElement newIcon = new VisualElement();
        newIcon.name = effectName; 
        
        newIcon.style.width = 36f;
        newIcon.style.height = 36f;
        newIcon.style.marginRight = 5f;
        // newIcon.style.borderRightWidth = 1f;
        // newIcon.style.borderBottomWidth = 1f;
        // newIcon.style.borderLeftWidth = 1f;
        // newIcon.style.borderTopWidth = 1f;
        // newIcon.style.borderRightColor = Color.white;
        // newIcon.style.borderBottomColor = Color.white;
        // newIcon.style.borderLeftColor = Color.white;
        // newIcon.style.borderTopColor = Color.white;
        newIcon.style.backgroundImage = new StyleBackground(effectIcon);

        targetContainer.Add(newIcon);
    }

    /// <summary>
    /// 상태이상이 해제되었을 때 제거하는 함수
    /// </summary>
    public void RemoveStatusEffect(string effectName, bool isMy)
    {
        VisualElement targetContainer = isMy ? _uiRefs.MyEffectContainer : _uiRefs.EnemyEffectContainer;
        if (targetContainer == null) return;

        VisualElement targetIcon = targetContainer.Q<VisualElement>(effectName);
        if (targetIcon != null)
        {
            targetContainer.Remove(targetIcon);
        }
    }
    
    public void ClearAllStatusEffects(bool isMy)
    {
        VisualElement targetContainer = isMy ? _uiRefs.MyEffectContainer : _uiRefs.EnemyEffectContainer;
        
    
        if (targetContainer != null)
        {
            targetContainer.Clear();
        }
    }
}
