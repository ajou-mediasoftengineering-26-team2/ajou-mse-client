using UnityEngine;

public class HitEndBehavior : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventBus.Publish(new HitEndAction(SceneDataBridge.playerCamera));
    }
}