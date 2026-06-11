using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

// 202422170 주형준
/// <summary>
/// Manages the gear button that opens and closes the settings panel.
/// Also listens for the Escape key as a keyboard shortcut to toggle settings.
/// Defers SettingsView initialization by one frame after enabling the UIDocument
/// to ensure the root visual element is fully ready before querying elements.
/// </summary>
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

    /// <summary>
    /// Toggles the settings panel open or closed.
    /// Calls CleanUp() before closing to unregister all button listeners inside SettingsView,
    /// preventing duplicate event subscriptions on the next open.
    /// Uses a one-frame coroutine delay after enabling the UIDocument
    /// because the rootVisualElement is not yet ready on the same frame as UIDocument.enabled = true.
    /// </summary>
    private void ToggleSettings()
    {
        bool open = !settingsUI.enabled;

        if (!open)
            settingsUI.GetComponent<SettingsView>().CleanUp();

        settingsUI.enabled = open;

        if (open)
            StartCoroutine(InitNextFrame());
    }

    /// <summary>
    /// Waits one frame before calling InitUI so the UIDocument's rootVisualElement
    /// is fully constructed and all Q() queries return valid results.
    /// </summary>
    private IEnumerator InitNextFrame()
    {
        yield return null;
        settingsUI.GetComponent<SettingsView>().InitUI();
    }
}