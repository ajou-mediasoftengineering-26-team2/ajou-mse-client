using System;
using UnityEngine;
using UnityEngine.Serialization;

//202322158 이준상

/// <summary>
/// Use EventBus few
/// </summary>
public class EffectRouter : MonoBehaviour
{
    private static readonly int HitAction = Animator.StringToHash("HitAction");
    private static readonly int LeftHitAction = Animator.StringToHash("LeftHitAction");
    private static readonly int RightHitAction = Animator.StringToHash("RightHitAction");
    private static readonly int RepeatCountHash = Animator.StringToHash("RepeatCount");

    [FormerlySerializedAs("fxAnimator")] [SerializeField] private Animator player1Animator;
    [SerializeField] private Animator player2Animator;
    private const string HandActionParameter = "HandAction";

    private void OnEnable()
    {
        EventBus.Subscribe<ActionSelectedEvent>(OnSelectFinished);
        EventBus.Subscribe<RoundWonEvent>(OnRoundWon);
        EventBus.Subscribe<HitAnimation>(OnHitAnimation);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ActionSelectedEvent>(OnSelectFinished);
        EventBus.Unsubscribe<RoundWonEvent>(OnRoundWon);
        EventBus.Unsubscribe<HitAnimation>(OnHitAnimation);
    }

    private void OnRoundWon(RoundWonEvent evt)
    {
        if (player1Animator == null) return;
        player1Animator.SetTrigger(evt.IsPlayer ? "PlayerWin" : "EnemyWin");
    }

    private void OnSelectFinished(ActionSelectedEvent evt)
    {
        Animator targetAnimator = GetAnimatorByPlayer(evt.Role);
        if (targetAnimator == null) return;

        int handActionValue = GetHandActionValue(evt.Role, evt.ActionCode);
        targetAnimator.SetInteger(HandActionParameter, handActionValue);
        StartCoroutine(ResetHandActionNextFrame(targetAnimator));
    }
    
    

    private Animator GetAnimatorByPlayer(BattleRole player)
    {
        return player == BattleRole.Attack ? player1Animator : player2Animator;
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
    
    private void OnHitAnimation(HitAnimation evt)
    {
        Animator targetAnimator = GetAnimatorByPlayer(evt.Role);


        switch (evt.hitAction)
        {
            case HitActionType.Left:
                targetAnimator.SetTrigger(LeftHitAction);
                break;
            case HitActionType.Right:
                targetAnimator.SetTrigger(RightHitAction);
                break;
            case HitActionType.Both5:
                targetAnimator.SetTrigger(HitAction);
                targetAnimator.SetInteger(RepeatCountHash, 0);
                break;
        }
    }

}
