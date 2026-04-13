using UnityEngine;

public class SampleScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private string _subscriptionId;
    
    private async void Start()
    {
        bool initialized = await FirebaseInitializer.EnsureInitializedAsync();
        Debug.Log($"Firebase 초기화: {initialized}"); 

        _subscriptionId = await
            FirebaseClient.Instance.SubscribeAsync<TestModel>(
                "test/",
                onValueChanged: (playerData) =>
                {
                    Debug.Log(playerData.message);
                },
                onError: (error) =>
                {
                    Debug.LogError($"error 가 났음.{error}");
                }
            );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
