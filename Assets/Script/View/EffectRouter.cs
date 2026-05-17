using System;
using UnityEngine;
using UnityEngine.Serialization;

//202322158 이준상

/// <summary>
/// Use EventBus few
/// </summary>
public class EffectRouter : MonoBehaviour
{
    [FormerlySerializedAs("fxAnimator")] [SerializeField] private Animator player1Animator;
    [SerializeField] private Animator player2Animator;

    private void OnEnable()
    {
        EventBus.Subscribe<ActionSelectedEvent>(OnActionSelected);
        EventBus.Subscribe<RoundWonEvent>(OnRoundWon);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ActionSelectedEvent>(onSelectFinished);
        EventBus.Unsubscribe<RoundWonEvent>(OnRoundWon);
    }

    private void OnRoundWon(RoundWonEvent evt)
    {
        if (player1Animator == null) return;
        player1Animator.SetTrigger(evt.IsPlayer ? "PlayerWin" : "EnemyWin");
    }

    private void OnActionSelected(ActionSelectedEvent evt)
    {


        
    }

    private void onSelectFinished(ActionSelectedEvent evt)
    {
        if  (player1Animator == null) return;
        player1Animator.SetInteger("HandAction", (int)evt.ActionCode);

        if (player2Animator == null) return;
        player2Animator.SetInteger("HandAction", (int)evt.ActionCode);
        StartCoroutine(ResetHandActionNextFrame());
    }

    private System.Collections.IEnumerator 
        ResetHandActionNextFrame()
    {
        yield return null; // 1 frame
        if (player1Animator != null)
            player1Animator.SetInteger("HandAction", 0);
    }

}
