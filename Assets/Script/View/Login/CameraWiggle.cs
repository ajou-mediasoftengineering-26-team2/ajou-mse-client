using UnityEngine;


//202322158 이준상


//Define Camera Wiggle
public class CameraWiggle : MonoBehaviour
{
    [Header("Perlin Noise")]
    [Tooltip("Noise Mangnitude")]
    public float noiseMagnitude = 0.05f;
    [Tooltip("Noise Speed")]
    public float noiseSpeed = 5f;


    private Vector3 originalPosition;
    private float noiseTimer;

    void Start()
    {
        // When script start, Original camera position saved by local positiion.
        originalPosition = transform.localPosition;
        
    }

    void Update()
    {
        // 1. time
        noiseTimer += Time.deltaTime * noiseSpeed;
        // 2. Calculate Default Engine Perlin Noise 
        float noiseX = (Mathf.PerlinNoise(noiseTimer, 0f) - 0.5f) * noiseMagnitude;
        float noiseY = (Mathf.PerlinNoise(0f, noiseTimer) - 0.5f) * noiseMagnitude;
        Vector3 noiseOffset = new Vector3(noiseX, noiseY, 0f);



        // 3. Apply Final Position
        transform.localPosition = originalPosition + noiseOffset;
    }

}