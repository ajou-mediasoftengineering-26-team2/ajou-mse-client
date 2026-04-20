using System;
using UnityEngine;

// BattleViewModel = 배틀 씬 데이터 관리
// Firebase에서 데이터 받아서 Observable에 넣어줌
// View(BattleUIController)는 이걸 구독해서 표시만 함
public class BattleViewModel : ViewModelBase
{
    // ─── Observable 선언 ───────────────────────────────────
    // 플레이어 1 (왼쪽)
    public Observable<int>   LeftHP         { get; } = new Observable<int>(0);
    public Observable<int>   LeftWins       { get; } = new Observable<int>(0);
    public Observable<int>   LeftHandInfo   { get; } = new Observable<int>(0);
    public Observable<int>   LeftCoin       { get; } = new Observable<int>(0);
    public Observable<int[]> LeftItems      { get; } = new Observable<int[]>(new int[3]);
    public Observable<int[]> LeftPerks      { get; } = new Observable<int[]>(new int[3]);
    public Observable<int[]> LeftStatuses   { get; } = new Observable<int[]>(new int[4]);

    // 플레이어 2 (오른쪽)
    public Observable<int>   RightHP        { get; } = new Observable<int>(0);
    public Observable<int>   RightWins      { get; } = new Observable<int>(0);
    public Observable<int>   RightHandInfo  { get; } = new Observable<int>(0);
    public Observable<int>   RightCoin      { get; } = new Observable<int>(0);
    public Observable<int[]> RightItems     { get; } = new Observable<int[]>(new int[3]);
    public Observable<int[]> RightPerks     { get; } = new Observable<int[]>(new int[3]);
    public Observable<int[]> RightStatuses  { get; } = new Observable<int[]>(new int[4]);

    // 방 정보
    public Observable<string> StationName      { get; } = new Observable<string>("");
    public Observable<int>    CurrentTurn      { get; } = new Observable<int>(0);
    public Observable<int>    CurrentRound     { get; } = new Observable<int>(0);
    public Observable<int>    AttackingPlayer  { get; } = new Observable<int>(0);
    public Observable<string> TimeEnd          { get; } = new Observable<string>("");
    public Observable<bool>   IsDefenseSuccess { get; } = new Observable<bool>(false);

    // ─── 초기화 ────────────────────────────────────────────
    // ViewModelLocator.Get<BattleViewModel>() 할 때 자동 호출
    public override async void Initialize()
    {
        try
        {
            // Firebase 연결 준비될 때까지 기다림
            bool initialized = await FirebaseInitializer.EnsureInitializedAsync();
            if (!initialized) return;

            // 현재 역 이름 구독
            // "currentTrainStation" 경로 값 바뀔 때마다 자동으로 StationName에 넣어줌
            await FirebaseClient.Instance.SubscribeAsync<StationModel>(
                "currentTrainStation",
                onValueChanged: (data) => StationName.Value = data.currentTrainStation,
                onError: (error) => Debug.LogError(error)
            );

            // 방 정보 구독
            string matchId = "test";
            await FirebaseClient.Instance.SubscribeAsync<RoomInfoModel>(
                $"rooms/{matchId}",
                onValueChanged: (data) =>
                {
                    // 방 정보
                    CurrentTurn.Value      = data.currentTurn;
                    CurrentRound.Value     = data.currentRound;
                    AttackingPlayer.Value  = data.attackingPlayer;
                    TimeEnd.Value          = data.timeEnd;
                    IsDefenseSuccess.Value = data.isDefenseSuccess;

                    // 플레이어 1 정보
                    LeftHP.Value       = data.player1Info.hp;
                    LeftHandInfo.Value = data.player1Info.handInfo;
                    LeftCoin.Value     = data.player1Info.coin;
                    LeftItems.Value    = data.player1Info.items;
                    LeftPerks.Value    = data.player1Info.perks;
                    LeftStatuses.Value = data.player1Info.statuses;

                    // 플레이어 2 정보
                    RightHP.Value       = data.player2Info.hp;
                    RightHandInfo.Value = data.player2Info.handInfo;
                    RightCoin.Value     = data.player2Info.coin;
                    RightItems.Value    = data.player2Info.items;
                    RightPerks.Value    = data.player2Info.perks;
                    RightStatuses.Value = data.player2Info.statuses;
                },
                onError: (error) => Debug.LogError(error)
            );
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}