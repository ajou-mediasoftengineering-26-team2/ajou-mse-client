using UnityEngine;
using UnityEngine.UIElements;

// 202422170 주형준
public class SettingsView : MonoBehaviour
{
    private const int MAX_VALUE = 10;
    private const int MIN_VALUE = 0;

    private Label  _brightnessIndex;
    private Label  _bgmIndex;
    private Label  _sfxIndex;
    private Button _brightnessLButton;
    private Button _brightnessRButton;
    private Button _bgmLButton;
    private Button _bgmRButton;
    private Button _sfxLButton;
    private Button _sfxRButton;
    private Button _exitButton;
    private Image  _brightnessImg;
    private Image  _bgmImg;
    private Image  _sfxImg;
    private Image  _exitImg;
    private Image  _brightnessLImg;
    private Image  _brightnessRImg;
    private Image  _bgmLImg;
    private Image  _bgmRImg;
    private Image  _sfxLImg;
    private Image  _sfxRImg;

    private int _brightnessValue;
    private int _bgmValue;
    private int _sfxValue;

    private Sprite _brightnessSprite;
    private Sprite _music0Sprite;
    private Sprite _music1Sprite;
    private Sprite _speaker0Sprite;
    private Sprite _speaker1Sprite;
    private Sprite _speaker2Sprite;
    private Sprite _leftArrowSprite;
    private Sprite _rightArrowSprite;
    private Sprite _exitSprite;

    private void Awake()
    {
        _brightnessSprite = Resources.Load<Sprite>("Settings/Brightness");
        _music0Sprite     = Resources.Load<Sprite>("Settings/Music0");
        _music1Sprite     = Resources.Load<Sprite>("Settings/Music1");
        _speaker0Sprite   = Resources.Load<Sprite>("Settings/Speaker0");
        _speaker1Sprite   = Resources.Load<Sprite>("Settings/Speaker1");
        _speaker2Sprite   = Resources.Load<Sprite>("Settings/Speaker2");
        _leftArrowSprite  = Resources.Load<Sprite>("Settings/LeftArrow");
        _rightArrowSprite = Resources.Load<Sprite>("Settings/RightArrow");
        _exitSprite       = Resources.Load<Sprite>("Settings/Exit");
    }

    private void Start()
    {
        GetComponent<UIDocument>().enabled = false;
    }

    public void InitUI()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        _brightnessIndex   = root.Q<Label>("BrightnessIndex");
        _bgmIndex          = root.Q<Label>("BGMIndex");
        _sfxIndex          = root.Q<Label>("SFXIndex");
        _brightnessLButton = root.Q<Button>("BrightnessLButton");
        _brightnessRButton = root.Q<Button>("BrightnessRButton");
        _bgmLButton        = root.Q<Button>("BGMLButton");
        _bgmRButton        = root.Q<Button>("BGMRButton");
        _sfxLButton        = root.Q<Button>("SFXLButton");
        _sfxRButton        = root.Q<Button>("SFXRButton");
        _exitButton        = root.Q<Button>("ExitButton");
        _brightnessImg     = root.Q<Image>("BrightnessImg");
        _bgmImg            = root.Q<Image>("BGMImg");
        _sfxImg            = root.Q<Image>("SFXImg");
        _exitImg           = root.Q<Image>("ExitImg");
        _brightnessLImg    = root.Q<Image>("BrightnessLImg");
        _brightnessRImg    = root.Q<Image>("BrightnessRImg");
        _bgmLImg           = root.Q<Image>("BGMLImg");
        _bgmRImg           = root.Q<Image>("BGMRImg");
        _sfxLImg           = root.Q<Image>("SFXLImg");
        _sfxRImg           = root.Q<Image>("SFXRImg");

        _brightnessImg.sprite  = _brightnessSprite;
        _exitImg.sprite        = _exitSprite;
        _brightnessLImg.sprite = _leftArrowSprite;
        _brightnessRImg.sprite = _rightArrowSprite;
        _bgmLImg.sprite        = _leftArrowSprite;
        _bgmRImg.sprite        = _rightArrowSprite;
        _sfxLImg.sprite        = _leftArrowSprite;
        _sfxRImg.sprite        = _rightArrowSprite;

        _brightnessValue = PlayerPrefs.GetInt("Brightness", 8);
        _bgmValue        = PlayerPrefs.GetInt("BGMVolume", 8);
        _sfxValue        = PlayerPrefs.GetInt("SFXVolume", 8);

        UpdateAllLabels();
        UpdateBgmIcon();
        UpdateSfxIcon();

        _brightnessLButton.clicked += OnBrightnessDown;
        _brightnessRButton.clicked += OnBrightnessUp;
        _bgmLButton.clicked        += OnBgmDown;
        _bgmRButton.clicked        += OnBgmUp;
        _sfxLButton.clicked        += OnSfxDown;
        _sfxRButton.clicked        += OnSfxUp;
        _exitButton.clicked        += OnExit;
    }

    public void CleanUp()
    {
        if (_brightnessLButton == null) return;
        _brightnessLButton.clicked -= OnBrightnessDown;
        _brightnessRButton.clicked -= OnBrightnessUp;
        _bgmLButton.clicked        -= OnBgmDown;
        _bgmRButton.clicked        -= OnBgmUp;
        _sfxLButton.clicked        -= OnSfxDown;
        _sfxRButton.clicked        -= OnSfxUp;
        _exitButton.clicked        -= OnExit;
    }

    private void OnBrightnessDown() => SetBrightness(_brightnessValue - 1);
    private void OnBrightnessUp()   => SetBrightness(_brightnessValue + 1);
    private void OnBgmDown()        => SetBgm(_bgmValue - 1);
    private void OnBgmUp()          => SetBgm(_bgmValue + 1);
    private void OnSfxDown()        => SetSfx(_sfxValue - 1);
    private void OnSfxUp()          => SetSfx(_sfxValue + 1);

    private void SetBrightness(int value)
    {
        _brightnessValue      = Mathf.Clamp(value, MIN_VALUE, MAX_VALUE);
        _brightnessIndex.text = _brightnessValue.ToString();
        PlayerPrefs.SetInt("Brightness", _brightnessValue);
        BrightnessOverlayView.Instance.SetBrightness(_brightnessValue);
    }

    private void SetBgm(int value)
    {
        _bgmValue      = Mathf.Clamp(value, MIN_VALUE, MAX_VALUE);
        _bgmIndex.text = _bgmValue.ToString();
        PlayerPrefs.SetInt("BGMVolume", _bgmValue);
        AudioManager.Instance.SetBgmVolume(_bgmValue / (float)MAX_VALUE);
        UpdateBgmIcon();
    }

    private void SetSfx(int value)
    {
        _sfxValue      = Mathf.Clamp(value, MIN_VALUE, MAX_VALUE);
        _sfxIndex.text = _sfxValue.ToString();
        PlayerPrefs.SetInt("SFXVolume", _sfxValue);
        AudioManager.Instance.SetSfxVolume(_sfxValue / (float)MAX_VALUE);
        UpdateSfxIcon();
    }

    private void UpdateAllLabels()
    {
        _brightnessIndex.text = _brightnessValue.ToString();
        _bgmIndex.text        = _bgmValue.ToString();
        _sfxIndex.text        = _sfxValue.ToString();
    }

    private void UpdateBgmIcon()
    {
        _bgmImg.sprite = _bgmValue <= 4 ? _music0Sprite : _music1Sprite;
    }

    private void UpdateSfxIcon()
    {
        if (_sfxValue == 0)      _sfxImg.sprite = _speaker0Sprite;
        else if (_sfxValue <= 4) _sfxImg.sprite = _speaker1Sprite;
        else                     _sfxImg.sprite = _speaker2Sprite;
    }

    private void OnExit()
    {
        CleanUp();
        GetComponent<UIDocument>().enabled = false;
    }
}