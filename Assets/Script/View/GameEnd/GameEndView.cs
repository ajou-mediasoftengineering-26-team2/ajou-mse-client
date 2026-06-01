using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

// 202422170 주형준
public class GameEndView : MonoBehaviour
{
    private Label _player1Id;
    private Label _player2Id;
    private Label _player1Score;
    private Label _player2Score;
    private Label _winnerIndex;
    private Button _homeButton;

    private void OnEnable()
    {
        var root     = GetComponent<UIDocument>().rootVisualElement;
        _player1Id   = root.Q<Label>("Player1Id");
        _player2Id   = root.Q<Label>("Player2Id");
        _player1Score = root.Q<Label>("Player1Score");
        _player2Score = root.Q<Label>("Player2Score");
        _winnerIndex = root.Q<Label>("WinnerIndex");
        _homeButton  = root.Q<Button>("HomeButton");

        _homeButton.clicked += OnHomeButtonClicked;
    }

    private void OnDisable()
    {
        _homeButton.clicked -= OnHomeButtonClicked;
    }

    public void ShowResult(GameEndEvent evt)
    {
        _player1Id.text    = evt.player1.username;
        _player2Id.text    = evt.player2.username;
        _player1Score.text = evt.player1.wins.ToString();
        _player2Score.text = evt.player2.wins.ToString();
        _winnerIndex.text  = evt.isPlayer1Winner
            ? $"Winner is {evt.player1.username}"
            : $"Winner is {evt.player2.username}";
    }

    private void OnHomeButtonClicked()
    {
        SceneManager.LoadScene("LoginScene");
    }
}