using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// 202422170 주형준
/// <summary>
/// Displays the round result notification (WIN/LOSE) at the end of each round.
/// Shows the current round number and coins earned, then plays a slide-up animation.
/// </summary>
public class RoundResultView : MonoBehaviour
{
    private Label _currentRound;
    private Label _roundResult;
    private Label _getMoney;
    private VisualElement _panel;
    private Coroutine _coroutine;

    private void OnEnable()
    {
        var root  = GetComponent<UIDocument>().rootVisualElement;
        _panel        = root.Q<VisualElement>("RoundResultUI");
        _currentRound = root.Q<Label>("CurrentRound");
        _roundResult  = root.Q<Label>("RoundResult");
        _getMoney     = root.Q<Label>("GetMoney");
    }

    /// <summary>
    /// Populates result labels and plays the entrance animation.
    /// Re-queries UI elements each call because UIDocument root may be
    /// recreated after an enable/disable cycle.
    /// Stops any currently running animation before starting a new one.
    /// </summary>
    public void ShowResult(RoundResultEvent evt)
    {
        var root  = GetComponent<UIDocument>().rootVisualElement; // 매번 새로
        _panel        = root.Q<VisualElement>("RoundResultUI");
        _currentRound = root.Q<Label>("CurrentRound");
        _roundResult  = root.Q<Label>("RoundResult");
        _getMoney     = root.Q<Label>("GetMoney");

        _currentRound.text = $"Round {evt.currentRound}";
        _roundResult.text  = evt.isWin ? "WIN" : "LOSE";
        _getMoney.text     = $"Coin: {evt.coin}";

        if (_coroutine != null) StopCoroutine(_coroutine); // 중복 방지
        _coroutine = StartCoroutine(PlayAnimation());
    }

    /// <summary>
    /// Plays a slide-up fade-in entrance animation on the result panel.
    /// The panel is snapped to its hidden state first (no transition),
    /// then transitions are enabled and the target values applied on the next frame
    /// to trigger the animation correctly.
    /// Uses EaseOutBack for a spring overshoot effect consistent with other UI panels.
    /// </summary>
    private IEnumerator PlayAnimation()
    {
        if (_panel == null) yield break;

        _panel.style.transitionProperty = StyleKeyword.Null;
        _panel.style.opacity  = 0f;
        _panel.style.translate = new StyleTranslate(new Translate(0, 80, 0));

        yield return null;

        _panel.style.transitionProperty = new List<StylePropertyName> { "opacity", "translate" };
        _panel.style.transitionDuration  = new List<TimeValue>
            { new TimeValue(0.5f, TimeUnit.Second), new TimeValue(0.5f, TimeUnit.Second) };
        _panel.style.transitionTimingFunction = new List<EasingFunction>
            { new EasingFunction(EasingMode.EaseOutBack) };

        yield return null;

        _panel.style.opacity   = 1f;
        _panel.style.translate = new StyleTranslate(new Translate(0, 0, 0));
    }
}