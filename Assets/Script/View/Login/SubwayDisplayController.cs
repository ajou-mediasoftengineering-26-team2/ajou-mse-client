using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

//202322158 이준상
public class SubwayDisplayController : MonoBehaviour
{
    private Label nicknameLabel;
    private Label waitTimeLabel;
    private VisualElement spinner;
    private float timer;
    
    private Coroutine _displayCoroutine;

    void OnEnable()
    {
        var root = GameObject.Find("LobbyWaitingUI").GetComponent<UIDocument>().rootVisualElement;
        
        waitTimeLabel = root.Q<Label>("WaitTimeLabel");
        spinner = root.Q<VisualElement>("Spinner");
    }

    void Update()
    {
        timer += Time.deltaTime;
        int seconds = Mathf.FloorToInt(timer);
        waitTimeLabel.text = seconds.ToString("D2") + " SEC";

        if (spinner != null)
        {
            spinner.style.rotate = new Rotate(new Angle(Time.time * 500f));
        }
    }
    public void StartDisplay()
    {
        // 중복 실행 방지: 이미 돌고 있다면 끄고 다시 시작
        StopDisplay();
        _displayCoroutine = StartCoroutine(UpdateDisplayRoutine());
    }
    
    public void StopDisplay()
    {
        if (_displayCoroutine != null)
        {
            StopCoroutine(_displayCoroutine);
            _displayCoroutine = null;
        }
    }
    
    
    private IEnumerator UpdateDisplayRoutine()
    {
        float timer = 0f;

        while (true) // 멈추라고 할 때까지 무한 반복
        {
            timer += Time.deltaTime;
            int seconds = Mathf.FloorToInt(timer);
            
            if (waitTimeLabel != null)
                waitTimeLabel.text = seconds.ToString("D2") + " SEC";

            if (spinner != null)
                spinner.style.rotate = new Rotate(new Angle(Time.time * 500f));

            // 다음 프레임까지 대기 (Update와 동일한 주기)
            yield return null; 
        }
    }
}