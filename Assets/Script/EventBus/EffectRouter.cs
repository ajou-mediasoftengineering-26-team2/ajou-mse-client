using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

//202322158 이준상

/// <summary>
/// Use EventBus for 3D Animation
/// </summary>
public class EffectRouter : MonoBehaviour
{
    
    //define animator event name
    private static readonly int HitAction = Animator.StringToHash("HitAction");
    private static readonly int LeftHitAction = Animator.StringToHash("LeftHitAction");
    private static readonly int RightHitAction = Animator.StringToHash("RightHitAction");
    private static readonly int RepeatCountHash = Animator.StringToHash("RepeatCount");
    private static readonly int Both1HitAction = Animator.StringToHash("Both1HitAction");
    private static readonly int Both7HitAction = Animator.StringToHash("Both7HitAction");

    //define animator
    [FormerlySerializedAs("fxAnimator")] [SerializeField] private Animator player1Animator;
    [SerializeField] private Animator player2Animator;
    private const string HandActionParameter = "HandAction";
    
    //define status event
    [SerializeField] private GameObject hitStatus1;
    [SerializeField] private GameObject hitStatus2;

    
    private Dictionary<Player, Animator> _playerAnimatorMap;
    
    
    /// <summary>
    /// Define Event Bus
    /// </summary>
    private void OnEnable()
    {
        _playerAnimatorMap = new Dictionary<Player, Animator>
        {
            { Player.First, player1Animator },
            { Player.Second, player2Animator }
        };
        EventBus.Subscribe<ActionSelectedEvent>(OnSelectFinished);
        EventBus.Subscribe<RoundWonEvent>(OnRoundWon);
        EventBus.Subscribe<HitAnimation>(OnHitAnimation);
        EventBus.Subscribe<StatusEvent>(OnStatusEvent);
        
        hitStatus1.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Status/MISSING");
        hitStatus2.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Status/MISSING");
    }

    private void OnStatusEvent(StatusEvent evt)
    {
        GameObject targetObject;
        
        if(evt.player == Player.First)
        {
            targetObject = hitStatus1;
        }
        else
        {
            targetObject = hitStatus2;
        }
        
        PlayStatusAnimation(targetObject, evt.status);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ActionSelectedEvent>(OnSelectFinished);
        EventBus.Unsubscribe<RoundWonEvent>(OnRoundWon);
        EventBus.Unsubscribe<HitAnimation>(OnHitAnimation);
    }

    
    /// <summary>
    /// Round won function
    /// </summary>
    /// <param name="evt"></param>
    private void OnRoundWon(RoundWonEvent evt)
    {
        if (player1Animator == null) return;
        player1Animator.SetTrigger(evt.IsPlayer ? "PlayerWin" : "EnemyWin");
    }

    
    /// <summary>
    /// Hand Action Animation Execute.
    /// </summary>
    /// <param name="evt"></param>
    private void OnSelectFinished(ActionSelectedEvent evt)
    {
        if (evt.Player1 == null || evt.Player2 == null)
        {
            Debug.LogError("[EffectRouter] ActionSelectedEvent missing player info.");
            return;
        }

        if (player1Animator == null || player2Animator == null)
        {
            Debug.LogError("[EffectRouter] Player animators are not assigned.");
            return;
        }

        if (!GameSetting.TryParseHandAction(evt.Player1.handChoice, out var action1))
        {
            Debug.LogError($"[EffectRouter] Unknown handChoice for player1: {evt.Player1.handChoice}");
        }

        if (!GameSetting.TryParseHandAction(evt.Player2.handChoice, out var action2))
        {
            Debug.LogError($"[EffectRouter] Unknown handChoice for player2: {evt.Player2.handChoice}");
        }

        var role1 = evt.Player1.attacking ? BattleRole.Attack : BattleRole.Defense;
        var role2 = evt.Player2.attacking ? BattleRole.Attack : BattleRole.Defense;

        
        int handActionValue1 = GetHandActionValue(role1, action1);
        int handActionValue2 = GetHandActionValue(role2, action2);

        _playerAnimatorMap[SceneDataBridge.myPlayer].SetInteger(HandActionParameter, handActionValue1);
        _playerAnimatorMap[SceneDataBridge.enemyPlayer].SetInteger(HandActionParameter, handActionValue2);
        StartCoroutine(ResetHandActionNextFrame(player1Animator));
        StartCoroutine(ResetHandActionNextFrame(player2Animator));
    }
    
    

    /// <summary>
    /// A function that maps animations for each player
    /// </summary>
    /// <param name="player"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    private Animator GetAnimatorByPlayer(Player player, BattleRole role)
    {
        if (!_playerAnimatorMap.TryGetValue(player, out var baseAnimator))
        {
            return null;
        }

        if (role == BattleRole.Defense)
        {
            return baseAnimator == player1Animator ? player2Animator : player1Animator;
        }
        else
        {
            return baseAnimator;
        }
    }

    
    /// <summary>
    /// Value must be different each of player role. so I define this function.
    /// </summary>
    /// <param name="role"></param>
    /// <param name="actionCode"></param>
    /// <returns></returns>
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
    
    /// <summary>
    /// Hit Hand Animaiton Event
    /// </summary>
    /// <param name="evt"></param>
    private void OnHitAnimation(HitAnimation evt)
    {
        Animator targetAnimator = GetAnimatorByPlayer(evt.Player,  evt.Role);
        Debug.Log("애니메이션이 실행됨!!!!!! ********");
        switch (evt.hitAction)
        {
            case HitActionType.Left:
                targetAnimator.SetTrigger(LeftHitAction);
                break;
            case HitActionType.Right:
                targetAnimator.SetTrigger(RightHitAction);
                break;
            case HitActionType.Both1:
                targetAnimator.SetTrigger(Both1HitAction);
                targetAnimator.SetInteger(RepeatCountHash, 0);
                break;
            case HitActionType.Both5:
                targetAnimator.SetTrigger(HitAction);
                targetAnimator.SetInteger(RepeatCountHash, 0);
                break;
            case HitActionType.Both7:
                targetAnimator.SetTrigger(Both7HitAction);
                targetAnimator.SetInteger(RepeatCountHash, 0);
                break;
        }
    }
    
    
    /// <summary>
    /// 3d Animation
    /// Code created by AI.
    /// </summary>
    /// <param name="targetObj"></param>
    /// <param name="status"></param>
    public void PlayStatusAnimation(GameObject targetObj, string status)
    {
        if (targetObj == null) return;

        SpriteRenderer sr = targetObj.GetComponent<SpriteRenderer>();
        Sprite missingSprite = Resources.Load<Sprite>($"Status/{status}");
        
        sr.sprite = missingSprite;
        targetObj.transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
        sr.color = new Color(1, 1, 1, 0); // 시작은 투명하게

        targetObj.transform.DOKill();
        sr.DOKill();

        Sequence seq = DOTween.Sequence();
        
        seq.Append(targetObj.transform.DOPunchScale(new Vector3(0.16f, 0.16f, 0.16f) * 0.5f, 0.2f, 10, 1f));
        seq.Join(sr.DOFade(1f, 0.1f));
        
        seq.AppendInterval(0.6f);
        
        seq.Append(sr.DOFade(0f, 0.3f));
    }

}