using UnityEngine;

//202322158 이준상

//Camera Manager
public class CameraTurnManager : MonoBehaviour
{
    [Header("Camera")]
    public GameObject playerCamera; // Player
    public GameObject enemyCamera;  // EnemyPlayer 

    /// <summary>
    /// Camera Setting
    /// </summary>
    /// <param name="isPlayerTurn"></param>
    public void SetCameraTarget(bool isPlayerTurn)
    {
        playerCamera.SetActive(isPlayerTurn);
        enemyCamera.SetActive(!isPlayerTurn);
    }
}