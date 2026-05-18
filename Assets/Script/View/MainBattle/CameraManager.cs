using UnityEngine;

//202322158 이준상

//Camera Manager
public class CameraTurnManager : MonoBehaviour
{
    public static CameraTurnManager Instance { get; private set; }

    [Header("Camera")]
    public GameObject Camera1; // Player
    public GameObject Camera2;  // EnemyPlayer 

    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
        }
    }
    /// <summary>
    /// Camera Setting
    /// </summary>
    /// <param name="isCamera1"></param>
    public void SetCameraTarget(CameraType isCamera1)
    {
        switch (isCamera1)
        {
            case CameraType.Camera1:
                Camera1.SetActive(true);
                Camera2.SetActive(false);
                break;
            case CameraType.Camera2:
                Camera2.SetActive(true);
                Camera1.SetActive(false);
                break;
        }
    }
}