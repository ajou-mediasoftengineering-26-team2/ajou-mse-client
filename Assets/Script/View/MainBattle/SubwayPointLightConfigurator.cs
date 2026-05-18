using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteAlways]
[DisallowMultipleComponent]
public class SubwayPointLightConfigurator : MonoBehaviour
{
    [SerializeField] private Transform anchorTransform;
    [SerializeField] private Vector3 pointLightOffset = new(0f, 2.5f, 0f);
    [SerializeField] private string pointLightName = "Subway Ceiling Point Light";
    [SerializeField] private bool autoApplyInEditMode = true;

    private static readonly Color WarmSubwayLightColor = new(1f, 0.95f, 0.82f, 1f);

    private void OnEnable()
    {
        TryApplyInEditor();
    }

    private void OnValidate()
    {
        TryApplyInEditor();
    }

    [ContextMenu("Apply Subway Point Light Setup")]
    public void ApplySubwayPointLightSetup()
    {
        Light pointLight = FindOrCreatePointLight();
        ConfigurePointLight(pointLight);
        DimDirectionalLight();
        MarkSceneDirty();
    }

    private void TryApplyInEditor()
    {
#if UNITY_EDITOR
        if (Application.isPlaying || !autoApplyInEditMode)
        {
            return;
        }

        ApplySubwayPointLightSetup();
#endif
    }

    private Light FindOrCreatePointLight()
    {
        Light pointLight = null;
        Light[] lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i].type == LightType.Point && lights[i].name == pointLightName)
            {
                pointLight = lights[i];
                break;
            }
        }

        if (pointLight != null)
        {
            return pointLight;
        }

        var go = new GameObject(pointLightName);
        go.transform.SetPositionAndRotation(GetAnchorPosition() + pointLightOffset, Quaternion.identity);
        pointLight = go.AddComponent<Light>();

#if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(go, "Create Subway Ceiling Point Light");
#endif
        return pointLight;
    }

    private void ConfigurePointLight(Light pointLight)
    {
#if UNITY_EDITOR
        Undo.RecordObject(pointLight, "Configure Subway Ceiling Point Light");
#endif
        pointLight.transform.position = GetAnchorPosition() + pointLightOffset;
        pointLight.type = LightType.Point;
        pointLight.color = WarmSubwayLightColor;
        pointLight.intensity = 2f;
        pointLight.range = 10f;
        pointLight.shadows = LightShadows.Soft;
    }

    private void DimDirectionalLight()
    {
        Light directional = RenderSettings.sun;
        if (directional == null || directional.type != LightType.Directional)
        {
            Light[] lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
            for (int i = 0; i < lights.Length; i++)
            {
                if (lights[i].type == LightType.Directional)
                {
                    directional = lights[i];
                    break;
                }
            }
        }

        if (directional == null)
        {
            return;
        }

#if UNITY_EDITOR
        Undo.RecordObject(directional, "Dim Directional Light");
#endif
        directional.intensity = 0.3f;
    }

    private Vector3 GetAnchorPosition()
    {
        if (anchorTransform != null)
        {
            return anchorTransform.position;
        }

        if (Camera.main != null)
        {
            return Camera.main.transform.position;
        }

        return Vector3.zero;
    }

    private void MarkSceneDirty()
    {
#if UNITY_EDITOR
        Scene scene = gameObject.scene;
        if (scene.IsValid())
        {
            EditorSceneManager.MarkSceneDirty(scene);
        }
#endif
    }
}
