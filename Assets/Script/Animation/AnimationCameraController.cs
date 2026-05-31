using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCameraController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("camera settings")]
    public Camera animationCamera;
    public float normalFOV = 60f;     // 기본 FOV (현재 인스펙터에 있던 60)
    public float zoomedFOV = 35f;
    

    [Header("시간 설정 (초 단위)")]
    public float zoomSpeed = 0.1f;      // 줌인/아웃되는 데 걸리는 시간
    public float maintainTime = 1.5f; // 줌인된 상태를 유지할 시간


    private void OnEnable()
    {
        EventBus.Subscribe<HitAnimationZoomIn>(AnimationCamera);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<HitAnimationZoomIn>(AnimationCamera);
    }

    public void AnimationCamera(HitAnimationZoomIn evt)
    {
        StartCoroutine(ZoomSequenceRoutine());
    }
    // 줌인했다가 대기 후 다시 돌아오는 마스터 코루틴
    private IEnumerator ZoomSequenceRoutine()
    {
        // 1. 부드럽게 줌인 (기본값 -> 줌인값)
        
        yield return StartCoroutine(ChangeFOVRoutine(normalFOV, zoomedFOV, zoomSpeed));

        // 2. 줌인된 상태에서 잠시 대기 (예: 라운드 스코어 UI 띄워주는 타이밍)
        yield return new WaitForSeconds(maintainTime);

        // 3. 다시 부드럽게 원래대로 줌아웃 (줌인값 -> 기본값)
        yield return StartCoroutine(ChangeFOVRoutine(zoomedFOV, normalFOV, zoomSpeed));
    }

    // 실제 FOV 값을 시간에 따라 부드럽게 변경해주는 핵심 코루틴 함수
    private IEnumerator ChangeFOVRoutine(float startFOV, float endFOV, float duration)
    {
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            
            // 경과 시간에 따른 비율(0 ~ 1) 계산
            float progress = timeElapsed / duration; 
            
            progress = Mathf.Clamp01(progress);
            // SmoothStep을 쓰면 시작과 끝이 더 부드러워집니다 (감속 연출)
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            // 카메라의 FOV 값을 부드럽게 보간
            animationCamera.fieldOfView = Mathf.Lerp(startFOV, endFOV, smoothProgress);
            
            yield return null; // 다음 프레임까지 대기
        }

        // 오차 보정 위해 최종 목표값으로 강제 고정
        animationCamera.fieldOfView = endFOV;
    }
}
