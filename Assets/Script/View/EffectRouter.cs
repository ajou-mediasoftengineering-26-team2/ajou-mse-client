using UnityEngine;

public class EffectRouter : MonoBehaviour
{
    [SerializeField] private Animator fxAnimator;

    private void OnEnable()
    {
        EventBus.Subscribe<RoundWonEvent>(OnRoundWon);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<RoundWonEvent>(OnRoundWon);
    }

    private void OnRoundWon(RoundWonEvent evt)
    {
        if (fxAnimator == null) return;
        fxAnimator.SetTrigger(evt.IsPlayer ? "PlayerWin" : "EnemyWin");
    }
}
