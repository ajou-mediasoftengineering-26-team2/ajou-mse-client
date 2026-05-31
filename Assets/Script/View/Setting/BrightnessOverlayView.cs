using UnityEngine;
using UnityEngine.UIElements;

// 202422170 주형준
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
        _overlay = GetComponent<UIDocument>().rootVisualElement
                       .Q<VisualElement>("BrightnessBackGround");

        int saved = PlayerPrefs.GetInt("Brightness", 8);
        SetBrightness(saved);
    }

    public void SetBrightness(int value)
    {
        float alpha = (1f - value / 10f) * 0.85f;
        _overlay.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, alpha));
    }
}