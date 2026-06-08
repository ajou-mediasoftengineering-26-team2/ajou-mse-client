using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameEndView : MonoBehaviour
{
    private Label  _player1Id, _player2Id;
    private Label  _player1Score, _player2Score;
    private Label  _winnerIndex;
    private Button _homeButton;
    
    // 애니메이션 대상 요소
    private VisualElement _gameEndCard;

    private void OnDisable()
    {
        if (_homeButton != null)
            _homeButton.clicked -= OnHomeButtonClicked;
    }

    public void ShowResult(GameEndEvent evt)
    {
        StartCoroutine(ShowResultCoroutine(evt));
    }

    private IEnumerator ShowResultCoroutine(GameEndEvent evt)
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        _player1Id    = root.Q<Label>("Player1Id");
        _player2Id    = root.Q<Label>("Player2Id");
        _player1Score = root.Q<Label>("Player1Score");
        _player2Score = root.Q<Label>("Player2Score");
        _winnerIndex  = root.Q<Label>("WinnerIndex");
        _homeButton   = root.Q<Button>("HomeButton");
        _gameEndCard  = root.Q<VisualElement>("GameEnd");

        _homeButton.clicked -= OnHomeButtonClicked;
        _homeButton.clicked += OnHomeButtonClicked;

        // 데이터 세팅
        _player1Id.text    = evt.player1.username;
        _player2Id.text    = evt.player2.username;
        _player1Score.text = evt.player1.wins.ToString();
        _player2Score.text = evt.player2.wins.ToString();
        _winnerIndex.text  = evt.isPlayer1Winner
            ? $"Winner is {evt.player1.username}"
            : $"Winner is {evt.player2.username}";

        // 1. 초기 상태: 카드 위쪽 + 투명, WinnerIndex/HomeButton 투명
        SetInstant(_gameEndCard);
        _gameEndCard.style.opacity   = 0f;
        _gameEndCard.style.translate = new StyleTranslate(new Translate(0, -40, 0));

        SetInstant(_winnerIndex);
        _winnerIndex.style.opacity = 0f;

        SetInstant(_homeButton);
        _homeButton.style.opacity = 0f;

        yield return null; // 레이아웃 계산 대기
        yield return null; // 한 프레임 더 (확실히)

        // 2. 카드 슬라이드 인
        SetTransition(_gameEndCard, 0.4f);
        _gameEndCard.style.opacity   = 1f;
        _gameEndCard.style.translate = new StyleTranslate(new Translate(0, 0, 0));

        // 3. 0.5s 후 WinnerIndex 등장
        yield return new WaitForSeconds(0.5f);
        SetTransition(_winnerIndex, 0.4f);
        _winnerIndex.style.opacity = 1f;

        // 4. 0.2s 후 HomeButton 등장
        yield return new WaitForSeconds(0.2f);
        SetTransition(_homeButton, 0.3f);
        _homeButton.style.opacity = 1f;
    }

    // transition 즉시 제거 (순간 상태 변경용)
    private void SetInstant(VisualElement el)
    {
        el.style.transitionDuration = new StyleList<TimeValue>(
            new List<TimeValue> { new TimeValue(0f, TimeUnit.Second) }
        );
    }

    // transition 설정
    private void SetTransition(VisualElement el, float duration)
    {
        el.style.transitionDuration = new StyleList<TimeValue>(
            new List<TimeValue> { new TimeValue(duration, TimeUnit.Second) }
        );
        el.style.transitionTimingFunction = new StyleList<EasingFunction>(
            new List<EasingFunction> { new EasingFunction(EasingMode.EaseOut) }
        );
    }

    private async void OnHomeButtonClicked()
    {
        Debug.Log("씬으로 로드 1");
        SceneManager.LoadScene("LoginScene");
        try
        {
            var repo = RepositoryFactory.Instance.Get<ILoginRepository>();
            await repo.DeletePlayer(SceneDataBridge.playerId);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        Debug.Log("씬으로 로드 2");
        SceneManager.LoadScene("LoginScene");
    }
}