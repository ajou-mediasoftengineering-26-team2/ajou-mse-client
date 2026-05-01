using System;
using UnityEngine;

public class MainBattleViewModel : ViewModelBase
{
    private readonly IMainBattleRepository _repository;

    private string _playerId;
    private string _lobbyId;
    
    public Observable<int> LeftHp { get; } =  new Observable<int>();
    public Observable<int> RightHp { get; } =  new Observable<int>();
    public Observable<int> LeftRoundWin { get; } =  new Observable<int>();
    public Observable<int> RightRoundWin { get; } = new Observable<int>();
    public Observable<long> TimeEnd { get; } = new Observable<long>();
    public Observable<bool> IsAttacker { get; } = new Observable<bool>();
    public Observable<string> StationName { get; } = new Observable<string>();
    public Observable<int[]> Items { get; } = new Observable<int[]>();
    public Observable<int[]> Perks { get; } = new Observable<int[]>();
    public Observable<int[]> StatusEffects { get; } = new Observable<int[]>();
    public Observable<int> Money { get; } = new Observable<int>();

    public MainBattleViewModel(string playerId, string lobbyId)
    {
        _playerId = playerId;
        _lobbyId = lobbyId;
        
        RepositoryFactory.Instance.Register<IMainBattleRepository, MainBattleRepository>();
        _repository = RepositoryFactory.Instance.Get<IMainBattleRepository>();
    }

    public override async void Initialize()
    {
        base.Initialize();
        try
        {
            bool initialized = await FirebaseInitializer.EnsureInitializedAsync();
            if (!initialized) return;

            // RoomInfo 구독
            await FirebaseClient.Instance.SubscribeAsync<RoomInfoModel>(
                $"rooms/{_lobbyId}",
                onValueChanged: (roomInfo) =>
                {
                    if (roomInfo == null) return;

                    LeftHp.Value            = roomInfo.player1Info.hp;
                    RightHp.Value           = roomInfo.player2Info.hp;
                    LeftRoundWin.Value      = roomInfo.player1Info.roundWin;
                    RightRoundWin.Value     = roomInfo.player2Info.roundWin;
                    Items.Value         = roomInfo.player1Info.items;
                    Perks.Value         = roomInfo.player1Info.perks;
                    StatusEffects.Value = roomInfo.player1Info.statusEffects;
                    TimeEnd.Value           = roomInfo.timeEnd;
                    Money.Value           = roomInfo.player1Info.money;
                    
                    // 내가 공격자인지 판단
                    IsAttacker.Value = (roomInfo.attackingPlayer == _playerId);
                },
                onError: (error) => Debug.LogError(error)
            );

            // 스테이션 구독
            await FirebaseClient.Instance.SubscribeAsync<StationModel>(
                "currentTrainStation",
                onValueChanged: (station) =>
                {
                    if (station == null) return;
                    StationName.Value = station.stationName;
                },
                onError: (error) => Debug.LogError(error)
            );
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    
    public async void OnHandAction(int moveType)
    {
        try
        {
            await _repository.PostHandAction(_playerId, moveType);
            EventBus.Publish(new AttackStartedEvent(isPlayer: true));
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
