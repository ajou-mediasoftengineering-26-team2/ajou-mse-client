using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// 202422170 주형준
public class HandShowView : MonoBehaviour
{
    private Coroutine _coroutine;

    public void Show(HandElementalChoiceResult obj)
    {
        if (_coroutine != null) StopCoroutine(_coroutine); // 중복 방지
        _coroutine = StartCoroutine(WaitThenAnimate());
    }

    private IEnumerator WaitThenAnimate()
    {
        var root   = GetComponent<UIDocument>().rootVisualElement;
        var panel1 = root.Q<VisualElement>("Player1");
        var panel2 = root.Q<VisualElement>("Player2");

        // 일단 숨기기
        SnapHidden(panel1);
        SnapHidden(panel2);

        // 두 손 다 유효할 때까지 대기 (최대 5초)
        var vm = ViewModelLocator.Instance.Get<MainBattleViewModel>();
        float elapsed = 0f;
        while (elapsed < 5f)
        {
            if (vm.MyHandElemental.Value != HandElementalType.NONE &&
                vm.EnemyHandElemental.Value != HandElementalType.NONE)
                break;
            elapsed += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        // 패널 숨긴 채로 데이터 먼저 세팅
        root.Q<Label>("Player1Id").text = vm.MyName.Value;
        root.Q<Label>("Player2Id").text = vm.EnemyName.Value;

        var hand1 = vm.MyHandElemental.Value;
        var hand2 = vm.EnemyHandElemental.Value;

        if (hand1 != HandElementalType.NONE)
        {
            root.Q<Image>("Player1Hand").sprite   = Resources.Load<Sprite>(HandInfoProvider.GetImagePath(hand1));
            root.Q<Label>("Player1HandName").text = HandInfoProvider.GetDisplayName(hand1);
        }
        if (hand2 != HandElementalType.NONE)
        {
            root.Q<Image>("Player2Hand").sprite   = Resources.Load<Sprite>(HandInfoProvider.GetImagePath(hand2));
            root.Q<Label>("Player2HandName").text = HandInfoProvider.GetDisplayName(hand2);
        }

        yield return null;

        // 이제 올라오는 애니메이션
        yield return new WaitForSeconds(0.2f);
        AnimateIn(panel1);

        yield return new WaitForSeconds(0.8f);
        AnimateIn(panel2);

        _coroutine = null;
    }

    private void SnapHidden(VisualElement el)
    {
        if (el == null) return;
        el.style.transitionProperty = StyleKeyword.Null;
        el.style.opacity   = 0f;
        el.style.translate = new StyleTranslate(new Translate(0, 60, 0));
    }

    private void AnimateIn(VisualElement el)
    {
        if (el == null) return;
        el.style.transitionProperty = new List<StylePropertyName> { "opacity", "translate" };
        el.style.transitionDuration = new List<TimeValue>
            { new TimeValue(0.5f, TimeUnit.Second), new TimeValue(0.5f, TimeUnit.Second) };
        el.style.transitionTimingFunction = new List<EasingFunction>
            { new EasingFunction(EasingMode.EaseOutBack) };
        el.style.opacity   = 1f;
        el.style.translate = new StyleTranslate(new Translate(0, 0, 0));
    }
}