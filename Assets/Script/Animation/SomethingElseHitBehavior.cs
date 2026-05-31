using UnityEngine;

public class SomethingElseHitBehavior : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventBus.Publish(new HitAnimationZoomIn());
        EventBus.Publish(new SortHitEvent());
    }
}