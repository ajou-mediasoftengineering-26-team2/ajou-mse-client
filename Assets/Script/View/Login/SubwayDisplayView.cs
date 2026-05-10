using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

//202322158 이준상
/// <summary>
/// Handles the visual display of waiting time and spinner rotation in the lobby UI.
/// In this class, I don't use view models.
/// I decided that if I use view models, I will pay more attention to them.
/// </summary>
public class SubwayDisplayView : MonoBehaviour
{
    private Label _nicknameLabel;
    private Label _waitTimeLabel;
    private VisualElement _spinner;
    
    // Stores a reference to the coroutine for precise control (start/stop).
    private Coroutine _displayCoroutine;

    void OnEnable()
    {
        // Locates the "LobbyWaitingUI" object and retrieves its root visual element.
        var root = GameObject.Find("LobbyWaitingUI").GetComponent<UIDocument>().rootVisualElement;
        
        // Assign UI controls via UQuery (Q).
        _waitTimeLabel = root.Q<Label>("WaitTimeLabel");
        _spinner = root.Q<VisualElement>("Spinner");
    }

    /// <summary>
    /// Starts the wait timer count and spinner rotation.
    /// </summary>
    public void StartDisplay()
    {
        // Prevention of duplicate execution: Stop any existing routine before starting a new one.
        StopDisplay();
        _displayCoroutine = StartCoroutine(UpdateDisplayRoutine());
    }
    
    /// <summary>
    /// Stops the UI update coroutine.
    /// </summary>
    public void StopDisplay()
    {
        if (_displayCoroutine != null)
        {
            StopCoroutine(_displayCoroutine);
            _displayCoroutine = null;
        }
    }
    
    /// <summary>
    /// Coroutine routine that updates UI elements every frame.
    /// </summary>
    private IEnumerator UpdateDisplayRoutine()
    {
        float timer = 0f;

        while (true) // Runs indefinitely until StopDisplay() is called.
        {
            timer += Time.deltaTime;
            int seconds = Mathf.FloorToInt(timer);
            
            // 1. Updates the waiting time label with "00 SEC" format.
            if (_waitTimeLabel != null)
                _waitTimeLabel.text = seconds.ToString("D2") + " SEC";

            // 2. Rotates the spinner element based on time at a constant speed.
            if (_spinner != null)
                _spinner.style.rotate = new Rotate(new Angle(Time.time * 500f));

            // Wait until the next frame (Equivalent to Update cycle).
            yield return null; 
        }
    }
}