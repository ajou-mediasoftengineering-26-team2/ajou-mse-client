using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.03f;
    private Material targetMaterial;
    private float xOffset;

    private static readonly int MainTexProperty = Shader.PropertyToID("_MainTex");
    private static readonly int BaseMapProperty = Shader.PropertyToID("_BaseMap");

    private void Start()
    {
        targetMaterial = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        xOffset = (xOffset + (scrollSpeed * Time.deltaTime)) % 1f;
        var offset = new Vector2(xOffset, 0f);

        targetMaterial.SetTextureOffset(MainTexProperty, offset);
        targetMaterial.SetTextureOffset(BaseMapProperty, offset);
    }
}
