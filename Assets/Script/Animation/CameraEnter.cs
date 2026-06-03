using UnityEngine;

namespace Script.Animation
{
    public class CameraEnter : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            EventBus.Publish(new HitEndAction(CameraType.Action));
        }
    }
}