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
        EventBus.Subscribe<SortHitEvent>(OnPlaySortSfx);
        EventBus.Subscribe<HardHitEvent>(OnPlayHardSfx);
    }

    private void OnPlayHardSfx(HardHitEvent obj)
    {
        Play(hardAttackClip);
    }

    private void OnPlaySortSfx(SortHitEvent evt)
    {
        Play(softAttackClip);
    }

    /// <summary>
    /// Unsubscribe EventBus
    /// </summary>
    private void OnDisable()
    {
        EventBus.Unsubscribe<ButtonEvent>(OnButton);
        EventBus.Unsubscribe<SortHitEvent>(OnPlaySortSfx);
        EventBus.Unsubscribe<HardHitEvent>(OnPlayHardSfx);
        // EventBus.Unsubscribe<AttackStartedEvent>(OnAttackStarted);
        // EventBus.Unsubscribe<RoundWonEvent>(OnRoundWon);
        // EventBus.Unsubscribe<PlaySfxEvent>(OnPlaySfx);
    }

    private void OnButton(ButtonEvent evt) => Play(buttonClickClip);

    

    /// <summary>
    /// Play the Music case of me and enemy
    /// </summary>
    /// <param name="evt"></param>
    // private void OnRoundWon(RoundWonEvent evt)
    // {
    //     Play(evt.IsPlayer ? playerRoundWinClip : enemyRoundWinClip);
    // }

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
