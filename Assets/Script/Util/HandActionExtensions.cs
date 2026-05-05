
//202322158 이준상
// public static class HandActionExtensions
//
// {
//     public static int GetDamage(this HandActionType action)
//     {
//         return action switch
//         {
//             HandActionType.SINGLE_HAND_FLIP_LEFT  => 1,
//             HandActionType.SINGLE_HAND_FLIP_RIGHT => 1,
//             HandActionType.BOTH_HANDS_FLIP  => 2,
//             HandActionType.INSERT_BETWEEN_HANDS  => 5,
//             HandActionType.SHAKE_OVER_HANDS      => 10, 
//             _ => 0
//         };
//     }
//
//     public static string GetName(this HandActionType action, bool isAttack)
//     {
//         return isAttack switch
//         {
//             // 공격 중일 때 (기존 로직)
//             true => action switch
//             {
//                 HandActionType.SINGLE_HAND_FLIP_LEFT  => "Left",
//                 HandActionType.SINGLE_HAND_FLIP_RIGHT => "Right",
//                 HandActionType.BOTH_HANDS_FLIP       => "Both",
//                 HandActionType.INSERT_BETWEEN_HANDS  => "Stab",
//                 HandActionType.SHAKE_OVER_HANDS      => "Wave",
//                 _ => "Waiting"
//             },
//
//             // 공격 중이 아닐 때 (대기 혹은 방어 등 다른 상태)
//             false => action switch
//             {
//                 HandActionType.None => "None",
//                 HandActionType.SingleHandFlipLeft  => "Left",
//                 HandActionType.SingleHandFlipRight => "Right",
//                 HandActionType.BothHandsFlip       => "Both",
//                 HandActionType.InsertBetweenHands  => "Defense",
//                 HandActionType.ShakeOverHands      => "Pause",
//                 _ => "Waiting"
//             },
//         };
//     }
// }