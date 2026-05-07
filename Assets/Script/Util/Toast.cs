using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
//202322158 이준상
public class Toast : MonoBehaviour
{
    private static Toast _instance;
    private static VisualElement _root;
    private static VisualElement _toastLayer;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // Resources/GlobalToast 프리팹을 로드해서 생성
        GameObject prefab = Resources.Load<GameObject>("ToastManager");
        if (prefab != null)
        {
            Instantiate(prefab);
            Debug.Log("글로벌 토스트 시스템 로드 완료");
        }

        RegisterRepo();
    }

    private static void RegisterRepo()
    {
        RepositoryFactory.Instance.Register<IIDRepository, IDRepository>();
        RepositoryFactory.Instance.Register<IMainBattleRepository, MainBattleRepository>();
    }

    void Awake()
    {
        // 1. 싱글톤 설정: 중복 생성을 방지하고 어디서든 접근 가능하게 함
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴 안 됨
            
            // 2. 내 몸에 붙은 UIDocument를 바로 바인딩
            var uiDoc = GetComponent<UIDocument>();
            if (uiDoc != null)
            {
                _root = uiDoc.rootVisualElement;
                // 토스트 전용 UIDocument이므로 여기서 pickingMode를 Ignore로 해서 
                // 다른 UI 클릭을 방해하지 않게 설정
                _root.pickingMode = PickingMode.Ignore; 
            }
        }
        else
        {
            Destroy(gameObject); // 이미 존재하면 새로 만들어진 건 삭제
        }
    }
    void OnEnable()
    {
        TryBindRoot(GetComponent<UIDocument>());
    }

    public static void Show(string message, float duration = 2.0f)
    {
        if (string.IsNullOrEmpty(message)) return;
        if (_root == null) return;
        EnsureToastLayer();

        Debug.Log(message);
        Label toast = new Label(message);
        toast.style.unityTextAlign = TextAnchor.MiddleCenter;
        toast.style.backgroundColor = new Color(0, 0, 0, 0.7f); // 반투명 검정
        toast.style.color = Color.white;
        toast.style.paddingLeft = 20;
        toast.style.paddingRight = 20;
        toast.style.paddingTop = 10;
        toast.style.paddingBottom = 10;
        toast.style.borderTopLeftRadius = 15;
        toast.style.borderTopRightRadius = 15;
        toast.style.borderBottomLeftRadius = 15;
        toast.style.borderBottomRightRadius = 15;
        toast.style.marginTop = 6;
        toast.style.marginBottom = -30;
        toast.pickingMode = PickingMode.Ignore;

        toast.style.opacity = 0;
        toast.style.transitionProperty = new List<StylePropertyName> { "opacity", "margin-bottom" };
        toast.style.transitionDuration = new List<TimeValue>
        {
            new TimeValue(0.35f, TimeUnit.Second),
            new TimeValue(0.35f, TimeUnit.Second)
        };

        _toastLayer.Add(toast);
        VisualElement currentLayer = _toastLayer;

        toast.schedule.Execute(() =>
        {
            toast.style.opacity = 1;
            toast.style.marginBottom = 6;
        }).StartingIn(500);

        toast.schedule.Execute(() => {
            toast.style.opacity = 0;
            toast.style.marginBottom = -30;
            toast.schedule.Execute(() => currentLayer?.Remove(toast)).StartingIn(350);
        }).StartingIn((int)(duration * 1000));
    }

    private static void EnsureRoot()
    {
        if (_root != null && _root.panel != null) return;

        _root = null;
        _toastLayer = null;

        UIDocument[] documents = Object.FindObjectsOfType<UIDocument>();
        UIDocument best = null;
        UIDocument fallback = null;

        foreach (UIDocument document in documents)
        {
            if (document == null || !document.isActiveAndEnabled) continue;
            if (document.rootVisualElement == null) continue;
            fallback ??= document;
            if (!IsUsableDocument(document)) continue;

            if (best == null || document.sortingOrder > best.sortingOrder)
            {
                best = document;
            }
        }

        TryBindRoot(best ?? fallback);
    }

    private static void TryBindRoot(UIDocument document)
    {
        if (!IsUsableDocument(document)) return;

        _root = document.rootVisualElement;
        _toastLayer = null;
    }

    private static bool IsUsableDocument(UIDocument document)
    {
        if (document == null || !document.isActiveAndEnabled) return false;
        VisualElement root = document.rootVisualElement;
        if (root == null) return false;

        
        return root.childCount > 0;
    }

    private static void EnsureToastLayer()
    {
        if (_toastLayer != null && _toastLayer.panel != null) return;

        _toastLayer = new VisualElement();
        _toastLayer.name = "ToastLayer";
        _toastLayer.style.position = Position.Absolute;
        _toastLayer.style.left = 0;
        _toastLayer.style.right = 0;
        _toastLayer.style.top = 0;
        _toastLayer.style.bottom = 0;
        _toastLayer.style.justifyContent = Justify.FlexEnd;
        _toastLayer.style.alignItems = Align.Center;
        _toastLayer.style.paddingBottom = 50;
        _toastLayer.pickingMode = PickingMode.Ignore;

        _root.Add(_toastLayer);
    }
}
