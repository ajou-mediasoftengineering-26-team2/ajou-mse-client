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
        Animator targetAnimator = GetAnimatorByPlayer(evt.Player1, );
        Animator targetAnimator2 = GetAnimatorByPlayer(evt.Player2, );
        
        if (targetAnimator == null) return;
        if (targetAnimator2 == null) return;
        

        int handActionValue1 = GetHandActionValue(evt.Role, evt.ActionCode);
        int handActionValue2 = GetHandActionValue(evt.Role, evt.ActionCode);
        targetAnimator.SetInteger(HandActionParameter, handActionValue1);
        targetAnimator2.SetInteger(HandActionParameter, handActionValue2);
        StartCoroutine(ResetHandActionNextFrame(targetAnimator));
        StartCoroutine(ResetHandActionNextFrame(targetAnimator2));
    }
    
    

    private Animator GetAnimatorByPlayer(Player player, BattleRole role)
    {
        switch (player, role)
        {
            // 1. First(왼쪽)가 공격하는 상황 -> 당연히 Second(오른쪽)가 맞으므로 오른쪽 팝업!
            case (Player.First, BattleRole.Attack):
                return player1Animator;

            // 2. First(왼쪽)가 수비(피격)하는 상황 -> 내가 맞았으므로 내 위치(왼쪽)에 팝업!
            case (Player.First, BattleRole.Defense):
                return player2Animator;

            // 3. Second(오른쪽)가 공격하는 상황 -> First(왼쪽)가 맞으므로 왼쪽 팝업!
            case (Player.Second, BattleRole.Attack):
                return player2Animator;
            // 4. Second(오른쪽)가 수비(피격)하는 상황 -> 내가 맞았으므로 내 위치(오른쪽)에 팝업!
            case (Player.Second, BattleRole.Defense):
                return player1Animator;
        }

        return null;
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
        Animator targetAnimator = GetAnimatorByPlayer(evt.Player,  evt.Role);


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
