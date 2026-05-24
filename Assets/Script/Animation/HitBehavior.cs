using UnityEngine;

public class HitBehavior: StateMachineBehaviour
{
    private static readonly int RepeatCountHash = Animator.StringToHash("RepeatCount");
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventBus.Publish(new HitAnimationZoomIn());
        EventBus.Publish(new HardHitEvent());
    }
    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int currentCount = animator.GetInteger(RepeatCountHash);
        Debug.Log(currentCount + " ***********");
        animator.SetInteger(RepeatCountHash, currentCount + 1);
    }
}