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
    private const string HandActionParameter = "HandAction";

    private void OnEnable()
    {
        EventBus.Subscribe<ActionSelectedEvent>(OnSelectFinished);
        EventBus.Subscribe<RoundWonEvent>(OnRoundWon);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ActionSelectedEvent>(OnSelectFinished);
        EventBus.Unsubscribe<RoundWonEvent>(OnRoundWon);
    }

    private void OnRoundWon(RoundWonEvent evt)
    {
        if (player1Animator == null) return;
        player1Animator.SetTrigger(evt.IsPlayer ? "PlayerWin" : "EnemyWin");
    }

    private void OnSelectFinished(ActionSelectedEvent evt)
    {
        Animator targetAnimator = GetAnimatorByPlayer(evt.Player);
        if (targetAnimator == null) return;

        int handActionValue = GetHandActionValue(evt.Role, evt.ActionCode);
        targetAnimator.SetInteger(HandActionParameter, handActionValue);
        StartCoroutine(ResetHandActionNextFrame(targetAnimator));
    }

    private Animator GetAnimatorByPlayer(Player player)
    {
        return player == Player.First ? player1Animator : player2Animator;
    }

    private int GetHandActionValue(BattleRole role, HandActionType actionCode)
    {
        // Role offset lets you keep HandActionType as a shared key while separating attack/defense animations.
        int roleOffset = role == BattleRole.Attack ? 0 : 10;
        return roleOffset + (int)actionCode;
    }

    private System.Collections.IEnumerator ResetHandActionNextFrame(Animator targetAnimator)
    {
        yield return null; // 1 frame
        if (targetAnimator != null)
            targetAnimator.SetInteger(HandActionParameter, 0);
    }

}
