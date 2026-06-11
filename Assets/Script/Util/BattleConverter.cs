using System.Collections.Generic;
using UnityEngine;

//202322158 이준상
namespace Script.Util
{
    /// <summary>
    /// Converter Battle Converter
    /// </summary>
    public class BattleConverter
    {
        private static readonly Dictionary<HandActionType, HitActionType> ActionToHitMap = new()
        {
            { HandActionType.SINGLE_HAND_FLIP_LEFT,  HitActionType.Left },
            { HandActionType.SINGLE_HAND_FLIP_RIGHT,   HitActionType.Right },    // 예시: 무거운 공격은 Both5 연출로 연결
            { HandActionType.BOTH_HANDS_FLIP,     HitActionType.Both1 }, // 예시: 불 속성 스킬은 크리티컬 연출로 연결
            { HandActionType.INSERT_BETWEEN_HANDS,  HitActionType.Both5 },
            { HandActionType.SHAKE_OVER_HANDS, HitActionType.Both7}
        };

        /// <summary>
        /// A function that safely converts HandActionType into HitActionType
        /// </summary>
        public static HitActionType GetHitType(HandActionType handAction)
        {
            if (ActionToHitMap.TryGetValue(handAction, out var hitType))
            {
                return hitType;
            }

            return HitActionType.Left; // default point
        }
        
        
        // private readonly Dictionary<HitActionType, int> _hitDelayMap = new()
        // {
        //     { HitActionType.Both5,    6000 }, // Both5 is 6 second
        //     { HitActionType.Both7,   10000 }, // Both7 is 19 second
        //     { HitActionType.Both1, 2500 },  // Both1 is  2.5 second
        //     { HitActionType.Left, 2500 },  // 크리티컬은 4.5초
        //     { HitActionType.Right, 2500 }  // 크리티컬은 4.5초
        // };
    }
}