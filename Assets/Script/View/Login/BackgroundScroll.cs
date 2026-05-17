using UnityEngine;

using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    public float scrollSpeed = 20f;
    private Material targetMaterial;

    void Start()
    {
        // Quad에 적용된 머티리얼을 가져옵니다.
        targetMaterial = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // 매 프레임마다 X축 방향으로 오프셋을 계산합니다.
        float offset = Time.time * scrollSpeed;
        
        // 머티리얼의 텍스처 위치를 이동시켜 무한 루프 효과를 줍니다.
        // "_MainTex"는 기본 셰이더의 텍스처 속성 이름입니다.
        targetMaterial.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }
}