using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// 202422170 주형준
public class SelectHandsView : MonoBehaviour
{
    private SelectHandsViewModel _viewModel;

    private Button _hand1Sel, _hand2Sel, _hand3Sel, _hand4Sel, _hand5Sel, _hand6Sel;
    private Image  _hand1Img, _hand2Img, _hand3Img, _hand4Img, _hand5Img, _hand6Img;
    private Label  _hand1Title, _hand2Title, _hand3Title, _hand4Title, _hand5Title, _hand6Title;
    private Label  _hand1Info,  _hand2Info,  _hand3Info,  _hand4Info,  _hand5Info,  _hand6Info;

    private VisualElement root;
    private VisualElement timer;
    private Image timerImg;

    private void OnEnable()
    {
        //StartScene();
    }

    public void StartScene()
    {
        _viewModel?.Dispose();
        root     = GetComponent<UIDocument>().rootVisualElement;
        timer    = root.Q<VisualElement>("Timer");
        timerImg = root.Q<Image>("TimerImg");

        _hand1Sel = root.Q<Button>("Hand1Sel");
        _hand2Sel = root.Q<Button>("Hand2Sel");
        _hand3Sel = root.Q<Button>("Hand3Sel");
        _hand4Sel = root.Q<Button>("Hand4Sel");
        _hand5Sel = root.Q<Button>("Hand5Sel");
        _hand6Sel = root.Q<Button>("Hand6Sel");

        _hand1Img = root.Q<Image>("Hand1Img");
        _hand2Img = root.Q<Image>("Hand2Img");
        _hand3Img = root.Q<Image>("Hand3Img");
        _hand4Img = root.Q<Image>("Hand4Img");
        _hand5Img = root.Q<Image>("Hand5Img");
        _hand6Img = root.Q<Image>("Hand6Img");

        _hand1Title = root.Q<Label>("Hand1Title");
        _hand2Title = root.Q<Label>("Hand2Title");
        _hand3Title = root.Q<Label>("Hand3Title");
        _hand4Title = root.Q<Label>("Hand4Title");
        _hand5Title = root.Q<Label>("Hand5Title");
        _hand6Title = root.Q<Label>("Hand6Title");

        _hand1Info = root.Q<Label>("Hand1Info");
        _hand2Info = root.Q<Label>("Hand2Info");
        _hand3Info = root.Q<Label>("Hand3Info");
        _hand4Info = root.Q<Label>("Hand4Info");
        _hand5Info = root.Q<Label>("Hand5Info");
        _hand6Info = root.Q<Label>("Hand6Info");

        SetHandCard(_hand1Img, _hand1Title, _hand1Info, HandElementalType.FIRE);
        SetHandCard(_hand2Img, _hand2Title, _hand2Info, HandElementalType.WATER);
        SetHandCard(_hand3Img, _hand3Title, _hand3Info, HandElementalType.WIND);
        SetHandCard(_hand4Img, _hand4Title, _hand4Info, HandElementalType.LIGHTNING);
        SetHandCard(_hand5Img, _hand5Title, _hand5Info, HandElementalType.POISON);
        SetHandCard(_hand6Img, _hand6Title, _hand6Info, HandElementalType.PLANT);

        _viewModel = new SelectHandsViewModel();
        _viewModel.SetPlayerInfo(SceneDataBridge.playerId, SceneDataBridge.MatchId);
        _viewModel.Initialize();

        _viewModel.CanSelect.Subscribe(can =>
        {
            _hand1Sel.SetEnabled(can);
            _hand2Sel.SetEnabled(can);
            _hand3Sel.SetEnabled(can);
            _hand4Sel.SetEnabled(can);
            _hand5Sel.SetEnabled(can);
            _hand6Sel.SetEnabled(can);
        });

        if (timerImg != null)
            timerImg.sprite = Resources.Load<Sprite>("Pixel Clock/TimerImg");

        _viewModel.IsVisible.Subscribe(visible =>
            root.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None);

        _viewModel.TimerRatio.Subscribe(ratio =>
        {
            if (timer != null)
                timer.style.width = new Length(ratio * 100, LengthUnit.Percent);
        });

        _hand1Sel.clicked += () => { _viewModel.OnSelectHand(1); OnHandSelected(_hand1Sel); };
        _hand2Sel.clicked += () => { _viewModel.OnSelectHand(2); OnHandSelected(_hand2Sel); };
        _hand3Sel.clicked += () => { _viewModel.OnSelectHand(3); OnHandSelected(_hand3Sel); };
        _hand4Sel.clicked += () => { _viewModel.OnSelectHand(4); OnHandSelected(_hand4Sel); };
        _hand5Sel.clicked += () => { _viewModel.OnSelectHand(5); OnHandSelected(_hand5Sel); };
        _hand6Sel.clicked += () => { _viewModel.OnSelectHand(6); OnHandSelected(_hand6Sel); };

        StartCoroutine(PlayEntranceAnimation());
    }

    private IEnumerator PlayEntranceAnimation()
    {
        SnapHidden(_hand1Sel); SnapHidden(_hand2Sel); SnapHidden(_hand3Sel);
        SnapHidden(_hand4Sel); SnapHidden(_hand5Sel); SnapHidden(_hand6Sel);

        yield return null;

        AnimateIn(_hand1Sel); yield return new WaitForSeconds(0.1f);
        AnimateIn(_hand2Sel); yield return new WaitForSeconds(0.1f);
        AnimateIn(_hand3Sel); yield return new WaitForSeconds(0.2f);

        AnimateIn(_hand4Sel); yield return new WaitForSeconds(0.1f);
        AnimateIn(_hand5Sel); yield return new WaitForSeconds(0.1f);
        AnimateIn(_hand6Sel);
    }

    private void SnapHidden(VisualElement el)
    {
        if (el == null) return;
        el.style.transitionProperty = StyleKeyword.Null;
        el.style.opacity   = 0f;
        el.style.translate = new StyleTranslate(new Translate(0, 50, 0));
    }

    private void AnimateIn(VisualElement el)
    {
        if (el == null) return;
        el.style.transitionProperty = new List<StylePropertyName> { "opacity", "translate" };
        el.style.transitionDuration = new List<TimeValue>
            { new TimeValue(0.4f, TimeUnit.Second), new TimeValue(0.4f, TimeUnit.Second) };
        el.style.transitionTimingFunction = new List<EasingFunction>
            { new EasingFunction(EasingMode.EaseOutBack) };
        el.style.opacity   = 1f;
        el.style.translate = new StyleTranslate(new Translate(0, 0, 0));
    }

    private void OnHandSelected(Button selected)
    {
        var all = new Button[]
            { _hand1Sel, _hand2Sel, _hand3Sel, _hand4Sel, _hand5Sel, _hand6Sel };
        foreach (var card in all)
        {
            card.style.borderTopColor    = new StyleColor(new Color(0.94f, 0.90f, 0.35f));
            card.style.borderBottomColor = new StyleColor(new Color(0.94f, 0.90f, 0.35f));
            card.style.borderLeftColor   = new StyleColor(new Color(0.94f, 0.90f, 0.35f));
            card.style.borderRightColor  = new StyleColor(new Color(0.94f, 0.90f, 0.35f));
        }
        selected.style.borderTopColor    = new StyleColor(Color.white);
        selected.style.borderBottomColor = new StyleColor(Color.white);
        selected.style.borderLeftColor   = new StyleColor(Color.white);
        selected.style.borderRightColor  = new StyleColor(Color.white);
    }

    private void SetHandCard(Image img, Label title, Label info, HandElementalType hand)
    {
        img.sprite = Resources.Load<Sprite>(HandInfoProvider.GetImagePath(hand));
        title.text = HandInfoProvider.GetDisplayName(hand);
        info.text  = HandInfoProvider.GetDescription(hand);
    }

    private void OnDestroy() => _viewModel?.Dispose();
}