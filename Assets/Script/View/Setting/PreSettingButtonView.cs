using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

// 202422170 주형준
public class PreSettingButtonView : MonoBehaviour
{
    [SerializeField] private UIDocument settingsUI;

    private Button _settingButton;
    private Image  _settingImg;

    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _settingButton     = root.Q<Button>("SettingButton");
        _settingImg        = root.Q<Image>("SettingImg");
        _settingImg.sprite = Resources.Load<Sprite>("Settings/Gear");
        _settingButton.clicked += OnSettingClicked;
    }

    private void OnDisable()
    {
        if (_settingButton != null)
            _settingButton.clicked -= OnSettingClicked;
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            ToggleSettings();
    }

    private void OnSettingClicked() => ToggleSettings();

    private void ToggleSettings()
    {
        bool open = !settingsUI.enabled;

        if (!open)
            settingsUI.GetComponent<SettingsView>().CleanUp();

        settingsUI.enabled = open;

        if (open)
            StartCoroutine(InitNextFrame());
    }

    private IEnumerator InitNextFrame()
    {
        yield return null;
        settingsUI.GetComponent<SettingsView>().InitUI();
    }
}