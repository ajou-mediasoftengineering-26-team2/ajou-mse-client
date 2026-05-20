using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
//202322158 이준상
public class Toast : MonoBehaviour
{
    private static Toast _instance;
    private static VisualElement _root;
    private static VisualElement _toastLayer;
    
    // Automatically initializes the toast system before the first scene loads.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // Loads the ToastManager prefab from the Resources folder to create a global instance.
        GameObject prefab = Resources.Load<GameObject>("ToastManager");
        if (prefab != null)
        {
            Instantiate(prefab);
            Debug.Log("Global Toast System Loaded.");
        }

        // Register repositories at the start of the application.
        RegisterRepo();
    }

    private static void RegisterRepo()
    {
        RepositoryFactory.Instance.Register<ILoginRepository, LoginRepository>();
        RepositoryFactory.Instance.Register<IMainBattleRepository, MainBattleRepository>();
        RepositoryFactory.Instance.Register<IPerkAndShopRepository, PerkAndShopRepository>();
    }

    
    void Awake()
    {
        // Singleton Setup: Ensures only one Toast instance exists throughout the game.
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // Persists across scene changes.
            
            var uiDoc = GetComponent<UIDocument>();
            if (uiDoc != null)
            {
                _root = uiDoc.rootVisualElement;
                // Set pickingMode to Ignore so the toast layer doesn't block UI interactions.
                _root.pickingMode = PickingMode.Ignore; 
            }
        }
        else
        {
            Destroy(gameObject); // Destroys duplicate instances.
        }
    }

    void OnEnable()
    {
        //Bind Toast at Scene.
        TryBindRoot(GetComponent<UIDocument>());
    }

    /// <summary>
    /// Displays a toast notification on the screen.
    /// * Note: The UI styling and animation logic were implemented with the assistance of AI.
    /// </summary>
    /// <param name="message">The text to display.</param>
    /// <param name="duration">How long the toast remains visible (in seconds).</param>
    public static void Show(string message, float duration = 2.0f)
    {
        if (string.IsNullOrEmpty(message)) return;
        if (_root == null) return;
        EnsureToastLayer();

        Debug.Log(message);
        
        // Create label and apply styles (Assisted by AI)
        Label toast = new Label(message);
        toast.style.unityTextAlign = TextAnchor.MiddleCenter;
        toast.style.backgroundColor = new Color(0, 0, 0, 0.7f);
        toast.style.color = Color.white;
        toast.style.paddingLeft = 20; toast.style.paddingRight = 20;
        toast.style.paddingTop = 10; toast.style.paddingBottom = 10;
        toast.style.borderTopLeftRadius = 15; toast.style.borderTopRightRadius = 15;
        toast.style.borderBottomLeftRadius = 15; toast.style.borderBottomRightRadius = 15;
        toast.style.marginTop = 6;
        toast.style.marginBottom = -30;
        toast.pickingMode = PickingMode.Ignore;

        // Configure fade-in/out and slide animations (Assisted by AI)
        toast.style.opacity = 0;
        toast.style.transitionProperty = new List<StylePropertyName> { "opacity", "margin-bottom" };
        toast.style.transitionDuration = new List<TimeValue>
        {
            new TimeValue(0.35f, TimeUnit.Second),
            new TimeValue(0.35f, TimeUnit.Second)
        };

        _toastLayer.Add(toast);
        VisualElement currentLayer = _toastLayer;

        // Play enter animation
        toast.schedule.Execute(() =>
        {
            toast.style.opacity = 1;
            toast.style.marginBottom = 6;
        }).StartingIn(500);

        // Schedule exit animation and removal
        toast.schedule.Execute(() => {
            toast.style.opacity = 0;
            toast.style.marginBottom = -30;
            toast.schedule.Execute(() => currentLayer?.Remove(toast)).StartingIn(350);
        }).StartingIn((int)(duration * 1000));
    }

    /// <summary>
    /// Decides whether to use the provided UIDocument as the canvas (_root) for toasts.
    /// </summary>
    private static void TryBindRoot(UIDocument document)
    {
        // First, check if the document is in a usable state.
        if (!IsUsableDocument(document)) return;
    
        // If usable, set the document's rootVisualElement as our main UI canvas.
        _root = document.rootVisualElement;
    
        // Since the root has changed, reset the toast layer to force a new one to be created.
        _toastLayer = null;
    }

    /// <summary>
    /// Validates if the UIDocument is active and capable of rendering UI.
    /// </summary>
    private static bool IsUsableDocument(UIDocument document)
    {
        // 1. Returns false if the document is null or the GameObject is disabled.
        if (document == null || !document.isActiveAndEnabled) return false;
    
        // 2. Returns false if there is no root visual element to attach UI to.
        VisualElement root = document.rootVisualElement;
        if (root == null) return false;

        // 3. Crucial: It is considered usable only if the root already contains at least one child element,
        // ensuring the UI system is fully initialized and ready to display content.
        return root.childCount > 0;
    }

    // Ensures a dedicated layer exists for displaying toasts.
    // Use AI
    private static void EnsureToastLayer()
    {
        if (_toastLayer != null && _toastLayer.panel != null) return;

        _toastLayer = new VisualElement();
        _toastLayer.name = "ToastLayer";
        _toastLayer.style.position = Position.Absolute;
        _toastLayer.style.left = 0; _toastLayer.style.right = 0;
        _toastLayer.style.top = 0; _toastLayer.style.bottom = 0;
        _toastLayer.style.justifyContent = Justify.FlexEnd;
        _toastLayer.style.alignItems = Align.Center;
        _toastLayer.style.paddingBottom = 50;
        _toastLayer.pickingMode = PickingMode.Ignore;

        _root.Add(_toastLayer);
    }
}
