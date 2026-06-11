using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// 202422170 주형준
/// <summary>
/// Displays the hand elemental reveal animation at the start of a round.
/// Shows both players' chosen elemental types sequentially using a slide-up animation.
/// Reads data directly from the event object to avoid ViewModel polling race conditions.
/// </summary>
public class HandShowView : MonoBehaviour
{
    private Coroutine _coroutine;


    public void Show(HandElementalChoiceResult obj)
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(WaitThenAnimate(obj)); // obj 전달
    }
    
    /// <summary>
    /// Reads player data from the event object, populates the UI elements,
    /// then animates both player panels into view sequentially.
    /// Player 1 appears first, followed by Player 2 after a short delay,
    /// creating a dramatic reveal effect.
    /// 
    /// Note: A previous version polled the ViewModel for hand elemental data,
    /// which caused timing issues when data was not yet available (returned NONE).
    /// The current approach reads directly from the event payload to guarantee data integrity.
    /// </summary>

    private IEnumerator WaitThenAnimate(HandElementalChoiceResult obj)
    {
        var root   = GetComponent<UIDocument>().rootVisualElement;
        var panel1 = root.Q<VisualElement>("Player1");
        var panel2 = root.Q<VisualElement>("Player2");

        SnapHidden(panel1);
        SnapHidden(panel2);

        // VM 폴링 제거 - obj에서 직접 읽기
        var hand1 = HandInfoProvider.FromString(obj.player1?.handElemental);
        var hand2 = HandInfoProvider.FromString(obj.player2?.handElemental);

        root.Q<Label>("Player1Id").text = obj.player1?.username ?? "";
        root.Q<Label>("Player2Id").text = obj.player2?.username ?? "";

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

        yield return new WaitForSeconds(0.2f);
        AnimateIn(panel1);

        yield return new WaitForSeconds(0.8f);
        AnimateIn(panel2);

        yield return new WaitForSeconds(2.0f);
        
        _coroutine = null;
    }

    /// <summary>
    /// Immediately sets the element to its off-screen hidden state with no transition.
    /// Must be called before AnimateIn to establish a clean starting position.
    /// </summary>
    private void SnapHidden(VisualElement el)
    {
        if (el == null) return;
        el.style.transitionProperty = StyleKeyword.Null;
        el.style.opacity   = 0f;
        el.style.translate = new StyleTranslate(new Translate(0, 60, 0));
    }

    /// <summary>
    /// Applies a slide-up fade-in transition to the given element.
    /// Uses EaseOutBack easing to produce a subtle spring overshoot on arrival.
    /// </summary>
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