using System;
using UnityEngine;
//202322158 이준상

/// <summary>
/// Use EventBus few
/// </summary>
public class EffectRouter : MonoBehaviour
{
    [SerializeField] private Animator fxAnimator;

    private void OnEnable()
    {
        EventBus.Subscribe<ActionSelectedEvent>(OnActionSelected);
        EventBus.Subscribe<RoundWonEvent>(OnRoundWon);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ActionSelectedEvent>(OnActionSelected);
        EventBus.Unsubscribe<RoundWonEvent>(OnRoundWon);
    }

    private void OnRoundWon(RoundWonEvent evt)
    {
        if (fxAnimator == null) return;
        fxAnimator.SetTrigger(evt.IsPlayer ? "PlayerWin" : "EnemyWin");
    }
    private void OnActionSelected(ActionSelectedEvent evt)
    {
        if (fxAnimator == null) return;
        fxAnimator.SetInteger("HandAction", (int)evt.ActionCode);

        StartCoroutine(ResetHandActionNextFrame());
    }
    
    private System.Collections.IEnumerator 
        ResetHandActionNextFrame()
    {
        yield return null; // 1 frame
        if (fxAnimator != null)
            fxAnimator.SetInteger("HandAction", 0);
    }

}
