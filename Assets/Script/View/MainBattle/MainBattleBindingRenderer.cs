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

        _viewModel.StationName.Subscribe(station =>
        {
            var label = _uiRefs.MainBattleRoot.Q<Label>("Station");
            label.text = station;
        });

        _viewModel.IsAttacker.Subscribe(data =>
        {
            _uiRefs.MyAttack.text = data ? "Attack" :  "Defend";
            _uiRefs.EnemyAttack.text = data ? "Defend" : "Attack";
        });
        _viewModel.HoverItem.Subscribe(data =>
        {
            if (_uiRefs.TooltipContainer == null) return;

            if (data == null)
            {
                _uiRefs.TooltipContainer.style.display = DisplayStyle.None;
                return;
            }

            _uiRefs.ItemTitle.text = _viewModel.HoverItemTitle.Value;
            _uiRefs.ItemDescription.text = _viewModel.HoverItemDes.Value;
            _uiRefs.TooltipContainer.style.display = DisplayStyle.Flex;
            var sprite = Resources.Load<Sprite>($"Items/{data}");
            _uiRefs.ItemIcon.style.backgroundImage = new StyleBackground(sprite);
        });
        
        _viewModel.HoverPerk.Subscribe(data =>
        {
            if (_uiRefs.TooltipContainer == null) return;

            if (data == null)
            {
                _uiRefs.TooltipContainer.style.display = DisplayStyle.None;
                return;
            }

            _uiRefs.ItemTitle.text = _viewModel.HoverPerkTitle.Value;
            _uiRefs.ItemDescription.text = _viewModel.HoverPerkDes.Value;
            _uiRefs.TooltipContainer.style.display = DisplayStyle.Flex;
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
            for (int i = 0; i < _uiRefs.EnemyItemSlots.Length; i++)
            {
                if (i < data.Count)
                {
                    var sprite = Resources.Load<Sprite>($"Items/{data[i]}");
                    if (sprite != null)
                    {
                        Debug.Log("sprite : " + sprite.name);
                        _uiRefs.EnemyItemSlots[i].style.backgroundImage = new StyleBackground(sprite);
                    }
                    else
                    {
                        _uiRefs.EnemyItemSlots[i].style.backgroundImage = null;
                    }
                }
                else
                {
                    _uiRefs.EnemyItemSlots[i].style.backgroundImage = null;
                }
            }
        });

        _viewModel.ItemLists.Subscribe(data =>
        {
            if (data == null) return;
            for (int i = 0; i < _uiRefs.MyItemSlots.Length; i++)
            {
                if (i < data.Count)
                {
                    var sprite = Resources.Load<Sprite>($"Items/{data[i]}");
                    if (sprite != null)
                    {
                        _uiRefs.MyItemSlots[i].style.backgroundImage = new StyleBackground(sprite);
                    }
                    else
                    {
                        _uiRefs.MyItemSlots[i].style.backgroundImage = null;
                    }
                }
                else
                {
                    _uiRefs.MyItemSlots[i].style.backgroundImage = null;
                }
            }
        });
        
        _viewModel.EnemyPerkList.Subscribe(data =>
        {
            if (data == null) return;
            for (int i = 0; i < _uiRefs.EnemyPerkSlots.Length; i++)
            {
                if (i < data.Count)
                {
                    var sprite = Resources.Load<Sprite>($"Perks/{data[i]}");
                    if (sprite != null)
                    {
                        _uiRefs.EnemyPerkSlots[i].style.backgroundImage = new StyleBackground(sprite);
                    }
                    else
                    {
                        _uiRefs.EnemyPerkSlots[i].style.backgroundImage = null;
                    }
                }
                else
                {
                    _uiRefs.EnemyPerkSlots[i].style.backgroundImage = null;
                }
            }
        });
        
        _viewModel.MyPerkList.Subscribe(data =>
        {
            if (data == null) return;
            for (int i = 0; i < _uiRefs.MyPerkSlots.Length; i++)
            {
                if (i < data.Count)
                {
                    var sprite = Resources.Load<Sprite>($"Perks/{data[i]}");
                    if (sprite != null)
                    {
                        _uiRefs.MyPerkSlots[i].style.backgroundImage = new StyleBackground(sprite);
                    }
                    else
                    {
                        _uiRefs.MyPerkSlots[i].style.backgroundImage = null;
                    }
                }
                else
                {
                    _uiRefs.MyPerkSlots[i].style.backgroundImage = null;
                }
            }
        });
        
        _viewModel.MyStatusList.Subscribe(data =>
        {
            ClearAllStatusEffects(true);
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
        if (_uiRefs.TooltipContainer == null)
        {
            Debug.LogError("TooltipContainer is null! Hover events will not work.");
            return;
        }

        _uiRefs.MainBattleRoot.Query<VisualElement>(className: "slot").ForEach(slot =>
        {
            Debug.Log("slot 생성이 됨." + slot.style.backgroundColor);
            slot.RegisterCallback<MouseEnterEvent>(evt =>
            {
                var currentSlot = evt.currentTarget as VisualElement;
                if (currentSlot == null)
                {
                    Debug.Log("log 출력함.");
                    return;
                }

                bool isEnemySlot = currentSlot.parent?.parent?.name == "EnemyInfoGrid";
                Debug.Log($"[PerkSlot] MouseEnter. isEnemy: {isEnemySlot}, name: {currentSlot.name}");

                Rect slotBounds = currentSlot.worldBound;
                string imageName = "None";
                var bg = currentSlot.resolvedStyle.backgroundImage; 

                if (bg.texture != null) imageName = bg.texture.name;
                else if (bg.sprite != null) imageName = bg.sprite.name;
                
                //if (!imageName.Equals("None"))
                //{
                    _viewModel.HoverEventPerk(imageName);
                    
                    float spacing = 10f; 
                    if (!isEnemySlot)
                    {
                        _uiRefs.TooltipContainer.style.left = slotBounds.x; 
                        _uiRefs.TooltipContainer.style.top = slotBounds.y - 150f - spacing; 
                    }
                    else
                    {
                        float tooltipWidth = 550f; 
                        _uiRefs.TooltipContainer.style.left = slotBounds.xMax - tooltipWidth; 
                        _uiRefs.TooltipContainer.style.top = slotBounds.y - 150f - spacing; 
                    }
                //
            });

            slot.RegisterCallback<MouseLeaveEvent>(_ => 
            {
                // Only clear if the tooltip is currently showing this perk
                _viewModel.HoverPerk.Value = null;
            });
        });
        
        _uiRefs.MainBattleRoot.Query<VisualElement>(className: "slot-item").ForEach(slot =>
        {
            Debug.Log("slot item 생성이 됨." + slot.style.backgroundColor);
            slot.RegisterCallback<MouseEnterEvent>(evt =>
            {
                var currentSlot = evt.currentTarget as VisualElement;
                if (currentSlot == null) return;

                bool isEnemySlot = currentSlot.parent?.parent?.name == "EnemyInfoGrid";
                Debug.Log($"[ItemSlot] MouseEnter. isEnemy: {isEnemySlot}, name: {currentSlot.name}");

                Rect slotBounds = currentSlot.worldBound;
                string imageName = "None";
                var bg = currentSlot.resolvedStyle.backgroundImage; 

                if (bg.texture != null) imageName = bg.texture.name;
                else if (bg.sprite != null) imageName = bg.sprite.name;
                
                //if (!imageName.Equals("None"))
                //{
                    _viewModel.HoverEventItem(imageName);
                    
                    float spacing = 10f; 
                    if (!isEnemySlot)
                    {
                        _uiRefs.TooltipContainer.style.left = slotBounds.x; 
                        _uiRefs.TooltipContainer.style.top = slotBounds.y - 150f - spacing; 
                    }
                    else
                    {
                        float tooltipWidth = 550f; 
                        _uiRefs.TooltipContainer.style.left = slotBounds.xMax - tooltipWidth; 
                        _uiRefs.TooltipContainer.style.top = slotBounds.y - 150f - spacing; 
                    }
                //}
            });

            slot.RegisterCallback<MouseLeaveEvent>(_ => 
            {
                _viewModel.HoverItem.Value = null;
            });
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
