using UnityEngine;
//202322158 이준상
public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    [Header("SFX Output")]
    [SerializeField] private AudioSource sfxSource;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private AudioClip playerAttackClip;
    [SerializeField] private AudioClip enemyAttackClip;
    [SerializeField] private AudioClip playerRoundWinClip;
    [SerializeField] private AudioClip enemyRoundWinClip;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        EventBus.Subscribe<ButtonEvent>(OnButton);
        EventBus.Subscribe<AttackStartedEvent>(OnAttackStarted);
        EventBus.Subscribe<RoundWonEvent>(OnRoundWon);
        EventBus.Subscribe<PlaySfxEvent>(OnPlaySfx);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ButtonEvent>(OnButton);
        EventBus.Unsubscribe<AttackStartedEvent>(OnAttackStarted);
        EventBus.Unsubscribe<RoundWonEvent>(OnRoundWon);
        EventBus.Unsubscribe<PlaySfxEvent>(OnPlaySfx);
    }

    private void OnButton(ButtonEvent evt) => Play(buttonClickClip);

    private void OnAttackStarted(AttackStartedEvent evt)
    {
        Play(evt.IsPlayer ? playerAttackClip : enemyAttackClip);
    }

    private void OnRoundWon(RoundWonEvent evt)
    {
        Play(evt.IsPlayer ? playerRoundWinClip : enemyRoundWinClip);
    }

    private void OnPlaySfx(PlaySfxEvent evt)
    {
        switch (evt.SfxType)
        {
            case SfxType.ButtonClick:
                Play(buttonClickClip);
                break;
            case SfxType.PlayerAttack:
                Play(playerAttackClip);
                break;
            case SfxType.EnemyAttack:
                Play(enemyAttackClip);
                break;
            case SfxType.PlayerRoundWin:
                Play(playerRoundWinClip);
                break;
            case SfxType.EnemyRoundWin:
                Play(enemyRoundWinClip);
                break;
        }
    }

    private void Play(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }
}
