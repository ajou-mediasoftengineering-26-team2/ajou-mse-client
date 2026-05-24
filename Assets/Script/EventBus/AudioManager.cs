using UnityEngine;
//202322158 이준상


/// <summary>
/// AudioManager that subscribes to and uses eventbus.
/// </summary>
public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    [Header("SFX Output")]
    [SerializeField] private AudioSource sfxSource;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private AudioClip softAttackClip;
    [SerializeField] private AudioClip hardAttackClip;
    [SerializeField] private AudioClip RoundWinClip;
    [SerializeField] private AudioClip enemyRoundWinClip;

    /// <summary>
    /// Use SingleTon
    /// </summary>
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

    /// <summary>
    /// Subscribe EventBus
    /// </summary>
    private void OnEnable()
    {
        EventBus.Subscribe<ButtonEvent>(OnButton);
        EventBus.Subscribe<AttackStartedEvent>(OnAttackStarted);
        EventBus.Subscribe<RoundWonEvent>(OnRoundWon);
        EventBus.Subscribe<PlaySfxEvent>(OnPlaySfx);
    }

    /// <summary>
    /// Unsubscribe EventBus
    /// </summary>
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

    /// <summary>
    /// Play the Music case of me and enemy
    /// </summary>
    /// <param name="evt"></param>
    private void OnRoundWon(RoundWonEvent evt)
    {
        Play(evt.IsPlayer ? playerRoundWinClip : enemyRoundWinClip);
    }

    /// <summary>
    /// PlayMusic each case of event
    /// </summary>
    /// <param name="evt"></param>
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

    /// <summary>
    /// Play Music
    /// </summary>
    /// <param name="clip"></param>
    private void Play(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }
}
