using UnityEngine;
using UnityEngine.UIElements;

// 202422170 주형준

/// <summary>
/// Singleton overlay that applies a dark tint across the entire screen to simulate brightness adjustment.
/// Persists across scene loads via DontDestroyOnLoad.
/// The overlay panel has PickingMode.Ignore so it does not block any UI interaction.
/// </summary>
public class BrightnessOverlayView : MonoBehaviour
{
    public static BrightnessOverlayView Instance { get; private set; }

    private VisualElement _overlay;

    private void Awake()
    {
        if (Instance == null)
       {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        //tooptip 부분 수정하였습니다.
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.pickingMode = PickingMode.Ignore;
        _overlay = GetComponent<UIDocument>().rootVisualElement
                       .Q<VisualElement>("BrightnessBackGround");

        int saved = PlayerPrefs.GetInt("Brightness", 8);
        SetBrightness(saved);
    }

    /// <summary>
    /// Adjusts the overlay alpha based on the given brightness value (0–10).
    /// Higher value = brighter screen (lower alpha on the dark overlay).
    /// Formula: alpha = (1 - value/10) * 0.85, so value 10 gives alpha 0 (fully bright)
    /// and value 0 gives alpha 0.85 (darkest).
    /// </summary>
    public void SetBrightness(int value)
    {
        float alpha = (1f - value / 10f) * 0.85f;
        _overlay.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, alpha));
    }
}