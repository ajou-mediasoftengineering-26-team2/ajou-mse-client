using System.Collections.Generic;
using UnityEngine;

namespace Script.Util
{
    public class BattleConverter
    {
        // 💡 1. HandActionType ➡️ HitActionType 매핑 딕셔너리 선언 및 초기화
        private static readonly Dictionary<HandActionType, HitActionType> ActionToHitMap = new()
        {
            // [형식] { HandActionType.내행동, HitActionType.보여줄피격연출 }
            { HandActionType.SINGLE_HAND_FLIP_LEFT,  HitActionType.Left },
            { HandActionType.SINGLE_HAND_FLIP_RIGHT,   HitActionType.Right },    // 예시: 무거운 공격은 Both5 연출로 연결
            { HandActionType.BOTH_HANDS_FLIP,     HitActionType.Both1 }, // 예시: 불 속성 스킬은 크리티컬 연출로 연결
            { HandActionType.INSERT_BETWEEN_HANDS,  HitActionType.Both5 },
            { HandActionType.SHAKE_OVER_HANDS, HitActionType.Both7}
        
            // ⚠️ 프로젝트에 선언된 Enum 타입에 맞춰서 아래에 계속 추가하시면 됩니다!
        };

        /// <summary>
        /// 💡 HandActionType을 받아 안전하게 HitActionType으로 변환해주는 함수
        /// </summary>
        public static HitActionType GetHitType(HandActionType handAction)
        {
            // 딕셔너리에서 매칭되는 연출 타입을 찾아서 반환합니다.
            if (ActionToHitMap.TryGetValue(handAction, out var hitType))
            {
                return hitType;
            }

            Debug.LogWarning($"[Converter] {handAction}에 매정된 HitActionType이 없습니다! 기본값을 반환합니다.");
            return HitActionType.Left; // 👈 예외 상황용 기본 피격 연출 설정
        }
        
        
        private readonly Dictionary<HitActionType, int> _hitDelayMap = new()
        {
            { HitActionType.Both5,    6000 }, // Both5는 6초
            { HitActionType.Both7,   10000 }, // 일반 타격은 3초
            { HitActionType.Both1, 2500 },  // 크리티컬은 4.5초
            { HitActionType.Left, 2500 },  // 크리티컬은 4.5초
            { HitActionType.Right, 2500 }  // 크리티컬은 4.5초
        };
        
        
    }
}