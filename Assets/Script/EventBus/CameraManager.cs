using System;
using UnityEngine;

//202322158 이준상

//Camera Manager. The state of the camera varies for each Lobby State.
public class CameraTurnManager : MonoBehaviour
{
    public static CameraTurnManager Instance { get; private set; }

    [Header("Camera")]
    public GameObject Camera1; // Player
    public GameObject Camera2;  // EnemyPlayer 
    public GameObject ActionCamera; //Action Camera
    public GameObject ActionCamera2;

    
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


    private void OnEnable()
    {
        EventBus.Subscribe<HitEndAction>(SetCameraTarget);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<HitEndAction>(SetCameraTarget);
    }

    /// <summary>
    /// Camera Setting
    /// </summary>
    /// <param name="isCamera1"></param>
    public void SetCameraTarget(HitEndAction evt)
    {
        switch (evt.ActionCode)
        {
            case CameraType.Camera1:
                Camera1.SetActive(true);
                Camera2.SetActive(false);
                ActionCamera2.SetActive(false);
                ActionCamera.SetActive(false);

                break;
            case CameraType.Camera2:
                Camera2.SetActive(true);
                Camera1.SetActive(false);
                ActionCamera.SetActive(false);
                ActionCamera2.SetActive(false);
                break;
            case CameraType.Action:
                ActionCamera.SetActive(true);
                Camera1.SetActive(false);
                Camera2.SetActive(false);
                ActionCamera2.SetActive(false);
                break;
            case CameraType.Action2 :
                ActionCamera2.SetActive(true);
                Camera1.SetActive(false);
                Camera2.SetActive(false);
                ActionCamera.SetActive(false);
                break;
        }
    }
}