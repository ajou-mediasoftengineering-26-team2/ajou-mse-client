using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ChoiceRevealView : MonoBehaviour
{
    private VisualElement _container;
    private VisualElement _leftPlayerGroup;
    private VisualElement _rightPlayerGroup;

    private VisualElement _leftChoiceImage;
    private VisualElement _rightChoiceImage;

    private Coroutine _animationCoroutine; // 실행 중인 코루틴을 제어하기 위한 변수

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        _container = root.Q<VisualElement>(className: "display-container");

        var playerGroups = root.Query<VisualElement>(className: "player-group").ToList();
        if (playerGroups.Count >= 2)
        {
            _leftPlayerGroup = playerGroups[0];
            _rightPlayerGroup = playerGroups[1];
        }

        _leftChoiceImage = root.Q<VisualElement>("left-choice-image");
        _rightChoiceImage = root.Q<VisualElement>("right-choice-image");

        // 켜질 때 트랜지션을 끄고 "즉시" 투명 상태로 스냅 초기화
        SnapToInitialState();
    }

    void OnDisable()
    {
        // 오브젝트가 꺼질 때 혹시 돌고 있을지 모를 코루틴을 확실히 정지
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }
    }

    /// <summary>
    /// 외부 테스트용 메서드 (무작위 스프라이트 트랩 방지 로그 추가)
    /// </summary>
    public void StartChoiceReveal()
    {
        var sprites = Resources.FindObjectsOfTypeAll<Sprite>();
        Debug.Log($"[ChoiceReveal] 찾아낸 스프라이트 개수: {sprites.Length}");

        if (sprites.Length >= 2)
        {
            RevealChoices(sprites[0], sprites[1]);
        }
        else
        {
            Debug.LogError("[ChoiceReveal] 메모리에 스프라이트가 2개 이상 없습니다! 테스트용 스프라이트를 직접 넣어주세요.");
        }
    }

    /// <summary>
    /// 트랜지션 없이 부드러운 연출 준비 단계로 즉시 스냅(Snap)하는 메서드
    /// </summary>
    private void SnapToInitialState()
    {
        if (_container != null)
        {
            _container.style.transitionProperty = StyleKeyword.Null; // 트랜지션 일시 제거
            _container.style.opacity = 1f; // 컨테이너는 항상 보이게
        }

        SnapElementZero(_leftPlayerGroup);
        SnapElementZero(_rightPlayerGroup);
        ResetBorders(_leftChoiceImage);
        ResetBorders(_rightChoiceImage);
    }

    private void SnapElementZero(VisualElement element)
    {
        if (element == null) return;
        element.style.transitionProperty = StyleKeyword.Null; // 트랜지션 없이 즉시 반영하기 위함
        element.style.opacity = 0f;
        element.style.translate = new StyleTranslate(new Translate(0, 50, 0));
    }

    private void ResetBorders(VisualElement image)
    {
        if (image == null) return;
        image.style.borderTopColor = StyleKeyword.Null;
        image.style.borderBottomColor = StyleKeyword.Null;
        image.style.borderLeftColor = StyleKeyword.Null;
        image.style.borderRightColor = StyleKeyword.Null;
    }

    /// <summary>
    /// 실제 애니메이션을 트리거하는 메인 진입점
    /// </summary>
    public void RevealChoices(Sprite leftPlayerSprite, Sprite rightPlayerSprite)
    {
        // 1. 혹시 이미 돌고 있는 애니메이션이 있다면 중복 실행 방지를 위해 처단
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }

        // 2. 완벽하게 초기 상태로 클린 세팅
        SnapToInitialState();

        if (leftPlayerSprite != null && _leftChoiceImage != null)
            _leftChoiceImage.style.backgroundImage = new StyleBackground(leftPlayerSprite);

        if (rightPlayerSprite != null && _rightChoiceImage != null)
            _rightChoiceImage.style.backgroundImage = new StyleBackground(rightPlayerSprite);

        // 3. 안정적인 유니티 코루틴 시퀀스 시작
        _animationCoroutine = StartCoroutine(PlaySequenceAnimationCoroutine());
    }

    /// <summary>
    /// schedule.Execute를 완벽하게 대체하는 타임라인 코루틴
    /// </summary>
    private IEnumerator PlaySequenceAnimationCoroutine()
    {
        // UI Toolkit 트랜지션 규칙을 안전하게 다시 심어줍니다.
        ApplyTransitionRules(_leftPlayerGroup);
        ApplyTransitionRules(_rightPlayerGroup);

        // 한 프레임 쉬어서 UI Toolkit 엔진이 변경된 트랜지션 규칙을 인지하도록 보장합니다.
        yield return null; 

        // --- 1. Player 1 등장 (2.0초 대기 후) ---
        yield return new WaitForSeconds(2.0f);
        if (_leftPlayerGroup != null)
        {
            _leftPlayerGroup.style.opacity = 1f;
            _leftPlayerGroup.style.translate = new StyleTranslate(new Translate(0, 0, 0));

            if (_leftChoiceImage != null)
            {
                Color color = new Color32(255, 200, 0, 255);
                _leftChoiceImage.style.borderTopColor = new StyleColor(color);
                _leftChoiceImage.style.borderBottomColor = new StyleColor(color);
                _leftChoiceImage.style.borderLeftColor = new StyleColor(color);
                _leftChoiceImage.style.borderRightColor = new StyleColor(color);
            }
        }

        // --- 2. Player 2 등장 (그로부터 1.5초 뒤, 총 3.5초 시점) ---
        yield return new WaitForSeconds(1.5f);
        if (_rightPlayerGroup != null)
        {
            _rightPlayerGroup.style.opacity = 1f;
            _rightPlayerGroup.style.translate = new StyleTranslate(new Translate(0, 0, 0));

            if (_rightChoiceImage != null)
            {
                Color orangeColor = new Color32(255, 100, 0, 255);
                _rightChoiceImage.style.borderTopColor = new StyleColor(orangeColor);
                _rightChoiceImage.style.borderBottomColor = new StyleColor(orangeColor);
                _rightChoiceImage.style.borderLeftColor = new StyleColor(orangeColor);
                _rightChoiceImage.style.borderRightColor = new StyleColor(orangeColor);
            }
        }

        // --- 3. 연출 종료 및 전체 UI 페이드아웃 시작 (그로부터 2.5초 뒤, 총 6.0초 시점) ---
        yield return new WaitForSeconds(2.5f);
        if (_container != null)
        {
            _container.style.transitionProperty = new List<StylePropertyName> { "opacity" };
            _container.style.transitionDuration = new List<TimeValue> { new TimeValue(0.5f, TimeUnit.Second) };
            _container.style.opacity = 0f;
        }

        // --- 4. 페이드아웃 애니메이션 시간(0.5초)만큼 기다린 뒤 오브젝트 비활성화 ---
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
        _animationCoroutine = null;
    }

    private void ApplyTransitionRules(VisualElement element)
    {
        if (element == null) return;
        element.style.transitionProperty = new List<StylePropertyName> { "opacity", "translate" };
        element.style.transitionDuration = new List<TimeValue> 
            { new TimeValue(0.5f, TimeUnit.Second), new TimeValue(0.5f, TimeUnit.Second) };
        element.style.transitionTimingFunction = new List<EasingFunction> 
            { new EasingFunction(EasingMode.EaseOutBack) };
    }
}