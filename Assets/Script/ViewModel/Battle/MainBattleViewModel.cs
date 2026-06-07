using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Script.Util;
using Unity.Mathematics;
using UnityEngine;

//202322158 이준상


/// <summary>
/// ViewModel for the main battle scene.
/// Maintains HP, rounds, timer, turn and selection state via observables.
/// Subscribes to Firebase match and player nodes to reflect real-time game state.
/// Sends player actions to server and exposes UI-friendly labels and countdown.
/// </summary>
public class MainBattleViewModel : ViewModelBase
{
    // Repository and runtime state (player/lobby ids, timers, subscriptions)
    private readonly IMainBattleRepository _repository;
    private readonly IRoundRepository _roundRepository;
    private readonly IElementalRepository _elementalRepository;
    private readonly IItemRepository _itemRepository;
    private string _playerId;
    private string _lobbyId;
    private string _enemyId;
    private bool _isTimerRunning = false;
    private bool _firebaseSubscribed;
    private CancellationTokenSource _timerCts;
    private PlayerInfoModel player1;
    private PlayerInfoModel player2;


    private PlayerInfoModel player1Snap;
    private PlayerInfoModel player2Snap;

    private bool _isFirstStart;
    private int _damageIndex = 0;

    // ── HP ──────────────────────────────────────────────────────────
    // Player HP observables (Left = local player, Right = remote player)
    public Observable<int> LeftHp { get; } = new Observable<int>();
    public Observable<int> RightHp { get; } = new Observable<int>();

    // ── round win maker ─────────────────────────────────────────────

    // Per-round win counters (used to display round wins)
    public Observable<int> LeftRoundWin { get; } = new Observable<int>();
    public Observable<int> RightRoundWin { get; } = new Observable<int>();


    // ── timer ──────────────────────────────────────────────────────
    // Countdown timer observable (used by UI for remaining seconds)
    public Observable<int> RemainingSeconds { get; } = new Observable<int>();

    // ── attacker? ─────────────────────────────────────────────────
    // Whether local player currently has attacking priority
    public Observable<bool> IsAttacker { get; } = new Observable<bool>();

    // ── station name ─────────────────────────────────────────────────────
    // Display name of the subway station for the match
    public Observable<string> StationName { get; } = new Observable<string>();

    // ──  game state 1 ───────────────────────────────────────────────────
    // Match-level state exposed to the UI
    public Observable<LobbyState> MatchState { get; } = new Observable<LobbyState>();
    public Observable<int> CurrentRound { get; } = new Observable<int>();
    public Observable<int> WinnerPlayerIdx { get; } = new Observable<int>(-1);


    //  ── game state 2 ───────────────────────────────────────────────────
    // Selection flags indicating if each player is currently selecting
    public Observable<bool> MySelecting { get; } = new Observable<bool>();
    public ObservableEvent<bool> MySelectingE { get; } = new ObservableEvent<bool>();
    public Observable<bool> EnemySelecting { get; } = new Observable<bool>();


    // ── money ──────────────────────────────────────────────────────────
    // Player currency
    public Observable<int> Money { get; } = new Observable<int>();

    // Status label displayed in UI (e.g., YOUR TURN, ENEMY TURN, GAME OVER)
    public Observable<string> LabelState { get; } = new Observable<string>();

    // ── name ──────────────────────────────────────────────────────────
    public Observable<String> MyName { get; } = new Observable<String>();

    public Observable<String> EnemyName { get; } = new Observable<string>();

    // current Turn
    // Current turn index
    public Observable<int> CurrentTurn { get; } = new Observable<int>();

    // Formatted countdown string (ss.ff)
    public Observable<string> CountDown { get; } = new Observable<string>();

    public Observable<string> HoverItem { get; } = new Observable<string>();
    public Observable<string> HoverItemTitle { get; } = new Observable<string>();
    public Observable<string> HoverItemDes { get; } = new Observable<string>();
    public Observable<string> HoverPerk { get; } = new Observable<string>();
    public Observable<string> HoverPerkTitle { get; } = new Observable<string>();
    public Observable<string> HoverPerkDes { get; } = new Observable<string>();

    // ── camera ──────────────────────────────────────────────────────────
    public Observable<CameraType> CameraPoint { get; } = new Observable<CameraType>();

    // ── Current action ──────────────────────────────────────────────────────────
    public Observable<HandActionType> CurrentHandAction { get; } =
        new Observable<HandActionType>(HandActionType.SINGLE_HAND_FLIP_LEFT);

    public Observable<string> CurrentHandActionText { get; } = new Observable<string>("Left");
    
    
    public Observable<List<ItemType>> ItemLists { get; } = new Observable<List<ItemType>>();
    public Observable<List<ItemType>> EnemyItemLists { get; } = new Observable<List<ItemType>>();
 

    public Observable<HandElementalType> MyHandElemental { get; } = new Observable<HandElementalType>();
    public Observable<HandElementalType> EnemyHandElemental { get; } = new Observable<HandElementalType>();

    public Observable<List<PerkType>> MyPerkList { get; } = new Observable<List<PerkType>>();
    public Observable<List<PerkType>> EnemyPerkList { get; } = new Observable<List<PerkType>>();

    public Observable<List<Damage>> DamageList { get; } = new Observable<List<Damage>>();


    public Observable<List<StatusType>> MyStatusList { get; } = new Observable<List<StatusType>>();
    public Observable<List<StatusType>> EnemyStatusList { get; } = new Observable<List<StatusType>>();
    
    private readonly Dictionary<HitActionType, int> _hitDelayMap = new()
    {
        { HitActionType.Both5,    6000 }, 
        { HitActionType.Both7,   6000 }, 
        { HitActionType.Both1, 2500 }, 
        { HitActionType.Left, 2500 },  
        { HitActionType.Right, 2500 }  
    };
    public MainBattleViewModel()
    {
        // _playerId = playerId;
        // _lobbyId = lobbyId;
        _repository = RepositoryFactory.Instance.Get<IMainBattleRepository>();
        _roundRepository = RepositoryFactory.Instance.Get<IRoundRepository>();
        _elementalRepository = RepositoryFactory.Instance.Get<IElementalRepository>();
        _itemRepository = RepositoryFactory.Instance.Get<IItemRepository>();
    }

    public override void Initialize()
    {
        if (IsInitialized) return;
        base.Initialize();
        CurrentHandActionText.Value = "Left";
        TryStartFirebaseSubscriptions();

        eventJunsang();
        _isFirstStart = true;
    }

    private void eventJunsang()
    {
        EventBus.Publish(new MainBattleEvent());
        EventBus.Subscribe<HardHitEvent>(obj =>
        {
            GetAnimatorByPlayer(SceneDataBridge.myPlayer,
                IsAttacker.Value ? BattleRole.Attack : BattleRole.Defense, DamageList.Value[_damageIndex]);
        });
        EventBus.Subscribe<SortHitEvent>(obj =>
        {
            GetAnimatorByPlayer(SceneDataBridge.myPlayer,
                IsAttacker.Value ? BattleRole.Attack : BattleRole.Defense, DamageList.Value[_damageIndex]);
        });
        EventBus.Subscribe<HitEndAction>(action =>
        {
            _damageIndex = 0;
        });
    }
    
    
    private void GetAnimatorByPlayer(Player player, BattleRole role, Damage damage)
    {
        bool isLeftOwner = false;
        
        
        switch (player, role)
        {
            case (Player.First, BattleRole.Attack):
                Toast.ShowDamagePopupRight(damage.damage);
                RightHp.Value -= damage.damage;
                LeftHp.Value += damage.recoveredHp;
                isLeftOwner = true;
                SetStaus(damage, true);
                if (damage.attackType != null)
                {
                    EventBus.Publish(new StatusEvent(damage.attackType, Player.Second));
                }
                break;
            case (Player.First, BattleRole.Defense):
                Toast.ShowDamagePopupLeft(damage.damage);
                LeftHp.Value -= damage.damage;
                RightHp.Value += damage.recoveredHp;
                isLeftOwner = false;
                SetStaus(damage, false);
                if (damage.attackType != null)
                {
                    EventBus.Publish(new StatusEvent(damage.attackType, Player.First));
                }
                break;
            case (Player.Second, BattleRole.Attack):
                Toast.ShowDamagePopupRight(damage.damage);
                RightHp.Value -= damage.damage;
                LeftHp.Value += damage.recoveredHp;
                isLeftOwner = false;
                SetStaus(damage, true);
                if (damage.attackType != null)
                {
                    EventBus.Publish(new StatusEvent(damage.attackType, Player.First));
                }
                break;
            case (Player.Second, BattleRole.Defense):
                Toast.ShowDamagePopupLeft(damage.damage);
                LeftHp.Value -= damage.damage;
                RightHp.Value += damage.recoveredHp;
                isLeftOwner = true;
                SetStaus(damage, false);
                if (damage.attackType != null)
                {
                    EventBus.Publish(new StatusEvent(damage.attackType, Player.Second));
                }
                break;
        }
    
        _damageIndex++;

        EventBus.Publish(new HitDamageEvent(damage, isLeftOwner));
    }

    private void SetStaus(Damage damage, bool isRight)
    {
        List<StatusType> status = new List<StatusType>();
        if (damage.statusEffects.Count != 0 || damage.statusEffects == null)
        {
            for (int i = 0; i <damage.statusEffects.Count; i++)
            {
                if (!Enum.TryParse<StatusType>(damage.statusEffects[i], out var type))
                {
                    Debug.LogError($"[ItemView] Unknown item code: {damage.statusEffects[i]}");
                    continue;
                }
                status.Add(type);
            }

            if (isRight)
            {
                EnemyStatusList.Value = status;
            }
            else
            {
                MyStatusList.Value = status;   
            }
        }
    }

    public void ChangeValue()
    {
        LeftRoundWin.Value = 2;
        Debug.Log(LeftRoundWin.Value + "Teststest");
    }

    /// <summary>
    /// Configure this ViewModel with the local player id, match id, and enemy id.
    /// Triggers starting of Firebase subscriptions when all ids are provided.
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="matchId"></param>
    /// <param name="enemyId"></param>
    public void HoverEventItem(string test)
    {
        if (Enum.TryParse<ItemType>(test, true, out ItemType itemtype))
        {
            HoverItemTitle.Value = ItemInfoProvider.GetDisplayName(itemtype);
            HoverItemDes.Value = ItemInfoProvider.GetDescription(itemtype);
        }
        HoverItem.Value = test;
    }
    
    public void HoverEventPerk(string test)
    {
        HoverPerkTitle.Value = PerkInfoProvider.GetDisplayName(PerkInfoProvider.GetPerkType(test));
        HoverPerkDes.Value = PerkInfoProvider.GetDescription(PerkInfoProvider.GetPerkType(test));
        HoverPerk.Value = test;
    }

    /// <summary>
    /// Ensure Firebase is initialized then subscribe to match and player nodes to keep
    /// observables up-to-date with server state (station, hp, round, countdown, etc.).
    /// </summary>
    private async Task FirebaseSetting()
    {
        try
        {
            bool initialized = await FirebaseInitializer.EnsureInitializedAsync();
            if (!initialized)
            {
                _firebaseSubscribed = false;
                return;
            }

            
            // matches/{lobbyId} subscribe
            await FirebaseClient.Instance.SubscribeAsync<MatchInfoModel>(
                $"matches/{_lobbyId}",
                onValueChanged: (match) =>
                {
                    if (match == null) return;
                    StationName.Value = match.station;
                    string stateText = match.state?.Trim();
                    if (Enum.TryParse(stateText, true, out LobbyState result))
                    {
                        MatchState.Value = result;
                    }
                    else
                    {
                        Debug.LogWarning($"[MainBattleViewModel] Unknown lobby state: '{match.state}'");
                    }

                    Debug.Log("current state = " + MatchState.Value.ToString());
                    ChangePlayByState(match);
                    GetStatusText();
                    CurrentRound.Value = match.currentRound;
                    WinnerPlayerIdx.Value = match.winnerPlayerIdx;
                    CurrentTurn.Value = match.currentTurn;

                    DamageList.Value = match.damageList;
                    StationName.Value = StationConverter.GetDisplayName(StationConverter.GetType(match.station));
                    //lobby data changing mean timer start again.
                },
                onError: (error) => Debug.LogError(error)
            );

            // my player subscribe -> left
            await FirebaseClient.Instance.SubscribeAsync<PlayerInfoModel>(
                $"matches/{_lobbyId}/players/{_playerId}",
                onValueChanged: (player) =>
                {
                    if (player == null) return;
                    //LeftHp.Value = player.hp;
                    IsAttacker.Value = player.attacking;
                    MySelecting.Value = player.selecting;
                    MySelectingE.Value = player.selecting;
                    MyName.Value = player.username;
                    LeftRoundWin.Value = player.wins;
                    player1 = player;
                    Debug.Log(player.hp + " " + player.username + player.hp + "Player(ME)");


                    MyHandElemental.Value = HandInfoProvider.FromString(player.handElemental);
                    ItemLists.Value = new List<ItemType>();
                    
                    List<ItemType> itms = new  List<ItemType>();

                    if (player.itemList == null)
                    {
                        itms = new  List<ItemType>();
                        ItemLists.Value = itms;
                    }
                    else
                    {
                        for (int i = 0; i < player.itemList.Count; i++)
                        {
                            if (!Enum.TryParse<ItemType>(player.itemList[i], out var itemType))
                            {
                                Debug.LogError($"[ItemView] Unknown item code: {player.itemList[i]}");
                                return;
                            }
                            
                        
                            itms.Add(itemType);
                        }
                        ItemLists.Value = itms;
                    }
                    
                    
                    List<PerkType> perks =new  List<PerkType>();

                    if (player.perkList != null)
                    {
                        for (int i = 0; i < player.perkList.Count; i++)
                        {
                            var data = PerkInfoProvider.GetPerkType(player.perkList[i]);
                        
                            perks.Add(data);
                        }
                        
                        MyPerkList.Value = perks;   
                    }
                    
                    
                    // List<StatusType> status = new List<StatusType>();
                    // if (player.statusEffectList != null)
                    // {
                    //     for (int i = 0; i < player.statusEffectList.Count; i++)
                    //     {
                    //         if (!Enum.TryParse<StatusType>(player.statusEffectList[i], out var type))
                    //         {
                    //             Debug.LogError($"[ItemView] Unknown status code: {player.statusEffectList[i]}");
                    //             continue;
                    //         }
                    //         status.Add(type);
                    //     }
                    // }
                    // MyStatusList.Value = status;
                },
                onError: (error) => Debug.LogError(error)
            );

            // enemy player subscribe -> right
            await FirebaseClient.Instance.SubscribeAsync<PlayerInfoModel>(
                $"matches/{_lobbyId}/players/{_enemyId}",
                onValueChanged: (player) =>
                {
                    if (player == null) return;
                    //RightHp.Value = player.hp;
                    EnemySelecting.Value = player.selecting;
                    EnemyName.Value = player.username;
                    RightRoundWin.Value = player.wins;
                    player2 = player;
                    EnemyHandElemental.Value = HandInfoProvider.FromString(player.handElemental);
                    Debug.Log(player.hp + " " + player.username + player.hp + "Enemy");
                    
                    if (player.itemList == null)
                    {
                        EnemyItemLists.Value = new List<ItemType>();
                    }
                    else
                    {
                        List<ItemType> enemyItems = new List<ItemType>();
                        for (int i = 0; i < player.itemList.Count; i++)
                        {
                            if (!Enum.TryParse<ItemType>(player.itemList[i], out var itemType))
                            {
                                Debug.LogError($"[EnemyItemView] Unknown item code: {player.itemList[i]}");
                                continue; 
                            }
        
                            enemyItems.Add(itemType);
                        }
                        
                        
                        EnemyItemLists.Value = enemyItems;
                        if (enemyItems.Count > 0)
                        {
                            Debug.Log(EnemyItemLists.Value[0] + "enemyItemList 구조");
                        }
                    }
                    
                    
                    
                    
                    List<PerkType> perks = new List<PerkType>();
                    if (player.perkList != null)
                    {
                        for (int i = 0; i < player.perkList.Count; i++)
                        {
                            var data = PerkInfoProvider.GetPerkType(player.perkList[i]);
                            perks.Add(data);
                        }
                    }
                    EnemyPerkList.Value = perks;

                    
                    // List<StatusType> status = new List<StatusType>();
                    // if (player.statusEffectList != null)
                    // {
                    //     for (int i = 0; i < player.statusEffectList.Count; i++)
                    //     {
                    //         if (!Enum.TryParse<StatusType>(player.statusEffectList[i], out var type))
                    //         {
                    //             Debug.LogError($"[ItemView] Unknown status code: {player.statusEffectList[i]}");
                    //             continue;
                    //         }
                    //         status.Add(type);
                    //     }
                    // }
                    // EnemyStatusList.Value = status;
                },
                onError: (error) => Debug.LogError(error)
            );
        }
        catch (Exception e)
        {
            _firebaseSubscribed = false;
            Debug.LogException(e);
        }
    }

    private async Task ChangePlayByState(MatchInfoModel match)
    {
        Func<Task> action = MatchState.Value switch
        {
            LobbyState.GAME_PLAYER_CHOICE => () =>
            {
                StartTimer(match.countdownStartTime, match.countdownSec);
                return Task.CompletedTask;
            },

            LobbyState.GAME_CHOICE_FINISHED => async () =>
            {
                //await Task.Delay(GameSetting.DELAY_MAP[SceneDataBridge.playerCamera]);
                await _repository.PutChoice(_playerId, CurrentHandAction.Value.ToString());
            },

            LobbyState.GAME_TURN_ANIMATION => async () =>
            {
                if (!GameSetting.TryParseHandAction(player1.handChoice, out HandActionType player1Action)) ;
                if (!GameSetting.TryParseHandAction(player2.handChoice, out HandActionType player2Action)) ;
                EventBus.Publish(new ChoiceAnimation(player1, player2));
                if (player1 == null || player2 == null)
                {
                    Debug.LogWarning("[MainBattleViewModel] ChoiceAnimation: player info not ready.");
                }
                await Task.Delay(7000);
                if (player1 != null && player2 != null)
                {
                    EventBus.Publish(new ActionSelectedEvent(player1, player2));
                }
                else
                {
                    Debug.LogWarning("[MainBattleViewModel] ActionSelectedEvent skipped: player info not ready.");
                }
                
                //if two player action is same, animation is not load.
                if (player1Action == player2Action)
                {
                    //await Task.Delay(GameSetting.DELAY_MAP[SceneDataBridge.playerCamera]);
                    await _repository.PutAck(_playerId);
                    return;
                }
                
                
                //if two player action is different, animation will be load.
                EventBus.Publish(new HitAnimation(
                    IsAttacker.Value ? BattleRole.Attack : BattleRole.Defense,
                    SceneDataBridge.myPlayer,
                    //SceneDataBridge.playerCamera == CameraType.Camera1 ? Player.First : Player.Second,
                    BattleConverter.GetHitType(IsAttacker.Value ? player1Action : player2Action),
                    null));
                if (!_hitDelayMap.TryGetValue(BattleConverter.GetHitType(IsAttacker.Value ? player1Action : player2Action), out int additionalDelay))
                {
                    additionalDelay = 6000; 
                }
                
                //await Task.Delay(GameSetting.DELAY_MAP[SceneDataBridge.playerCamera] + additionalDelay);
                await _repository.PutAck(_playerId);
                await GetHPByFirebase();
            },

            LobbyState.END_RESULT => () =>
            {
                /*EventBus.Publish(new RoundOver(true));
                return Task.CompletedTask;*/
                //게임 엔드로 수정했슴다
                bool isP1Winner = player1.finalWinner;
                EventBus.Publish(new GameEndEvent(player1, player2, isP1Winner));
                return Task.CompletedTask;
            },

            LobbyState.LOBBY_START_COUNTDOWN or LobbyState.GAME_ROUND_START_ANIMATION => async () => 
            {
                await GetHPByFirebase();
                if (_isFirstStart)
                {
                    EventBus.Publish(new IntroduceStationEvent(StationConverter.GetDisplayName(StationConverter.GetType(match.station)),
                        player1, 
                        player2));
                    _isFirstStart = false;
                }
                else
                {
                    EventBus.Publish(new MatchStartEvent());
                }
            },

            LobbyState.GAME_ROUND_END_PLAYER_KO => async () =>
            {
                EventBus.Publish(new RoundOver(true));
                EventBus.Publish(new RoundResultEvent(isWin: player1.hp > 0, currentRound: match.currentRound, coin: player1.coin));
                if (!GameSetting.DELAY_MAP.TryGetValue(SceneDataBridge.playerCamera, out int cameraDelay))
                {
                    Debug.LogWarning($"[MainBattleViewModel] Unknown camera: {SceneDataBridge.playerCamera}. Using 0ms delay.");
                    cameraDelay = 0;
                }
                await Task.Delay( 5000);
                
                Debug.Log("이게 왜 안뜨지");
                if (string.IsNullOrWhiteSpace(SceneDataBridge.playerId))
                {
                    Debug.LogError("[MainBattleViewModel] endAck skipped: playerId is empty.");
                    return;
                }

                var response = await _roundRepository.endAck(SceneDataBridge.playerId);
                if (!response.isSuccess)
                {
                    Debug.LogError($"[MainBattleViewModel] endAck failed: code={response.error?.code}, msg={response.error?.message}");
                }
            },

            LobbyState.GAME_ELEMENTAL_CHOICE => async () =>
            {
                EventBus.Publish(new HandElementalChoice());
                //await Task.Delay(GameSetting.DELAY_MAP[SceneDataBridge.playerCamera] + 5000);
                //_elementalRepository.PutChoice(_playerId, ElementalHand.FIRE.ToString());
            },

            LobbyState.GAME_ELEMENTAL_RECEIVING => async () =>
            {
                //await Task.Delay(GameSetting.DELAY_MAP[SceneDataBridge.playerCamera]);//추가
                EventBus.Publish(new HandElementalChoiceResult(player1, player2));
                //await Task.Delay(GameSetting.DELAY_MAP[SceneDataBridge.playerCamera] + 6500);
                await Task.Delay(6500);
                _elementalRepository.PutAck(_playerId);
            },
            LobbyState.GAME_PERK_ITEM_RECEIVING => async () =>
            {
                Debug.Log("player.receivedItemList : " + player1.receivedItemList.Count);
                if (player1?.receivedItemList != null && player1.receivedItemList.Count > 0)
                {
                    for (int i = 0; i < player1.receivedItemList.Count; i++)
                        EventBus.Publish(new ItemReceivedEvent(player1.receivedItemList[i]));
                    return;
                }
                await _itemRepository.PutAck(SceneDataBridge.playerId);
            },
            LobbyState.GAME_PERK_CHOICE => () =>
            {
                EventBus.Publish(new PerkChoiceEvent());
                return Task.CompletedTask;
            },

            _ => () => Task.CompletedTask // 매칭되는 상태가 없을 때 기본 동작 (예외 처리 필요 시 throw 가능)
        };

        // 매칭된 비동기/동기 액션 실행
        await action();
    }

    private async Task GetHPByFirebase()
    {
        string myPath = $"matches/{_lobbyId}/players/{_playerId}";
        string enemyPath = $"matches/{_lobbyId}/players/{_enemyId}";
    
        // GetAsync를 통해 딱 한 번만 스냅샷을 찍어옴
        player1 = await FirebaseClient.Instance.GetAsync<PlayerInfoModel>(myPath);
        player2 = await FirebaseClient.Instance.GetAsync<PlayerInfoModel>(enemyPath);

        if (player1 != null)
        {
            LeftHp.Value = player1.hp;
        }

        if (player2 != null)
        {
            RightHp.Value = player2.hp;
        }
    }

    /// <summary>
    /// Runs a high-frequency countdown that updates CountDown observable until end time.
    /// Uses CancellationToken to stop the timer when needed.
    /// Some logics were written with the help of ai.
    /// </summary>
    /// <param name="startTimeStr"></param>
    /// <param name="durationSec"></param>
    private async void StartTimer(string startTimeStr, int durationSec)
    {
        _timerCts?.Cancel();
        _timerCts?.Dispose();
        _timerCts = new CancellationTokenSource();
        var token = _timerCts.Token;

        //setting format and convert to DataTime
        string format = "yyyy-MM-dd'T'HH:mm:ss.fff";
        if (!DateTime.TryParseExact(startTimeStr, format, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out DateTime startTime))
        {
            return;
        }

        DateTime endTime = startTime.AddSeconds(durationSec);

        Debug.Log(startTime.ToString() + "*****************" + durationSec);
        //Show CountDown Value
        try
        {
            while (!token.IsCancellationRequested)
            {
                TimeSpan remaining = endTime - DateTime.Now;
                double totalSeconds = remaining.TotalSeconds;

                if (totalSeconds <= 0)
                {
                    CountDown.Value = "00.00";
                    break;
                }

                int sec = (int)totalSeconds;
                int ms = (int)((totalSeconds - sec) * 100);
                CountDown.Value = string.Format("{0:D2}.{1:D2}", sec, ms);

                await Task.Delay(10, token);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    /// <summary>
    /// Starts Firebase subscriptions if ViewModel initialized and ids are set.
    /// Guards against duplicate subscription setup.
    /// </summary>
    private void TryStartFirebaseSubscriptions()
    {
        if (!IsInitialized || _firebaseSubscribed) return;
        if (string.IsNullOrWhiteSpace(_playerId) ||
            string.IsNullOrWhiteSpace(_lobbyId) ||
            string.IsNullOrWhiteSpace(_enemyId))
            return;

        _firebaseSubscribed = true;
        _ = FirebaseSetting();
    }

    /// <summary>
    /// Compute human-friendly LabelState based on MatchState and selecting flags.
    /// </summary>
    private void GetStatusText()
    {
        switch (MatchState.Value)
        {
            // 1. 로비 & 카운트다운
            case LobbyState.LOBBY_WAITING:
                LabelState.Value = "WAITING FOR PLAYERS..";
                break;

            case LobbyState.GAME_ROUND_START_ANIMATION:
            case LobbyState.LOBBY_START_COUNTDOWN:
                LabelState.Value = "START SOON..";
                break;

            case LobbyState.MATCH_START:
            case LobbyState.GAME_ITEM_ANIMATION:
            case LobbyState.GAME_PERK_ITEM_RECEIVING:
            case LobbyState.GAME_ELEMENTAL_RECEIVING:
                LabelState.Value = "READY.."; 
                break;

            case LobbyState.GAME_TURN_ANIMATION:
                LabelState.Value = "AHHHH!!!!!!";
                break;
            // 3. 실제 플레이어 행동 선택 구간 (여기서 턴을 체크합니다)
            case LobbyState.GAME_PLAYER_CHOICE:
            case LobbyState.GAME_ATK_CHOICE:
            case LobbyState.GAME_DEF_CHOICE:
                LabelState.Value = "CHOOSE YOUR ACTION!";
                break;

            case LobbyState.GAME_CHOICE_FINISHED:
                LabelState.Value = "WAITING FOR OTHER PLAYER..";
                break;

            // 4. 퍽(특성) 및 원소 선택 구간
            case LobbyState.GAME_PERK_CHOICE:
                LabelState.Value = "SELECT YOUR PERK";
                break;

            case LobbyState.GAME_ELEMENTAL_CHOICE:
                LabelState.Value = "SELECT YOUR ELEMENT";
                break;

            // 5. 게임 종료 및 아웃
            case LobbyState.END_RESULT:
                LabelState.Value = "GAME OVER!";
                break;

            case LobbyState.END_PLAYER_DISCONNECTED:
                LabelState.Value = "PLAYER DISCONNECTED";
                break;

            case LobbyState.GAME_ROUND_END_PLAYER_KO:
                LabelState.Value = "ROUND END!";
                break;
            default:
                LabelState.Value = "UNKNOWN STATE";
                break;
        }
    }


    /// <summary>
    /// Cleanup timers and unsubscribe flags when ViewModel is disposed.
    /// </summary>
    public override void Dispose()
    {
        _timerCts?.Cancel();
        _timerCts?.Dispose();
        _firebaseSubscribed = false;
        base.Dispose();
    }

    public void SetPlayerAndMatchId(string playerId, string matchId, string enemyId, CameraType playerCamera,
        CameraType enemyCamera)
    {
        _playerId = playerId;
        _lobbyId = matchId;
        _enemyId = enemyId;
        CameraPoint.Value = playerCamera;
        TryStartFirebaseSubscriptions();
    }

    public void OnChangeActionIndex(HandActionType actionIndex, string actionText)
    {
        CurrentHandAction.Value = actionIndex;
        CurrentHandActionText.Value = actionText;
    }


    public async void PutRoundStartAck()
    {
        Debug.Log("put round start ack 보내고 있음!");
        //await Task.Delay(GameSetting.DELAY_MAP[SceneDataBridge.playerCamera]);
        await _roundRepository.startAck(SceneDataBridge.playerId);
    }
}