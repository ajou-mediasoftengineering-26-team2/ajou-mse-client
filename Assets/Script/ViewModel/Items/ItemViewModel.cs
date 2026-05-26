using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// 202422170 주형준
public class ItemViewModel : ViewModelBase
{
    private IItemRepository _repo;
    private string _playerId;
    private string _lobbyId;
    private string _matchSubId;
    private string _playerSubId;
    private bool _ackSent = false;

    public Observable<bool>   IsVisible  { get; } = new Observable<bool>();
    public Observable<string> ItemRaw    { get; } = new Observable<string>();
    public Observable<string> ItemName   { get; } = new Observable<string>();
    public Observable<string> ItemDesc   { get; } = new Observable<string>();
    public Observable<string> Item1Raw   { get; } = new Observable<string>();
    public Observable<string> Item2Raw   { get; } = new Observable<string>();
    public Observable<string> Item3Raw   { get; } = new Observable<string>();

    public void SetPlayerInfo(string playerId, string lobbyId)
    {
        _playerId = playerId;
        _lobbyId  = lobbyId;
    }

    public override async void Initialize()
    {
        base.Initialize();
        RepositoryFactory.Instance.Register<IItemRepository, ItemRepository>();
        _repo = RepositoryFactory.Instance.Get<IItemRepository>();

        EventBus.Subscribe<ItemAnimationEndEvent>(OnAnimationEnd);

        try
        {
            await FirebaseInitializer.EnsureInitializedAsync();
            await SubscribeMatchStateAsync();
            await SubscribePlayerInfoAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async Task SubscribeMatchStateAsync()
    {
        _matchSubId = await FirebaseClient.Instance.SubscribeAsync<MatchInfoModel>(
            $"matches/{_lobbyId}",
            onValueChanged: match =>
            {
                if (match == null) return;
                if (!Enum.TryParse<LobbyState>(match.state, true, out var state)) return;

                bool isItemPhase = state == LobbyState.GAME_ITEM_ANIMATION;
                IsVisible.Value = isItemPhase;

                if (!isItemPhase)
                    _ackSent = false;
            },
            onError: err => Debug.LogError(err)
        );
    }

    private async Task SubscribePlayerInfoAsync()
    {
        _playerSubId = await FirebaseClient.Instance.SubscribeAsync<PlayerInfoModel>(
            $"matches/{_lobbyId}/players/{_playerId}",
            onValueChanged: player =>
            {
                if (player == null) return;
                
                var list = player.itemList;
                Item1Raw.Value = list != null && list.Count > 0 ? list[0] : null;
                Item2Raw.Value = list != null && list.Count > 1 ? list[1] : null;
                Item3Raw.Value = list != null && list.Count > 2 ? list[2] : null;
                
                if (string.IsNullOrEmpty(player.receviedItem)) return;
                if (!Enum.TryParse<ItemType>(player.receviedItem, out var itemType)) return;

                ItemRaw.Value  = player.receviedItem;
                ItemName.Value = ItemInfoProvider.GetDisplayName(itemType);
                ItemDesc.Value = ItemInfoProvider.GetDescription(itemType);
            },
            onError: err => Debug.LogError(err)
        );
    }

    private async void OnAnimationEnd(ItemAnimationEndEvent evt)
    {
        if (_ackSent || !IsVisible.Value) return;
        _ackSent = true;
        try
        {
            var res = await _repo.PutAck(_playerId);
            if (!res.isSuccess) Debug.LogError(res.error.message);
        }
        catch (Exception e) { Debug.LogException(e); }
    }

    public override void Dispose()
    {
        EventBus.Unsubscribe<ItemAnimationEndEvent>(OnAnimationEnd);
        if (!string.IsNullOrEmpty(_matchSubId))
            FirebaseClient.Instance.Unsubscribe(_matchSubId);
        if (!string.IsNullOrEmpty(_playerSubId))
            FirebaseClient.Instance.Unsubscribe(_playerSubId);
        base.Dispose();
    }
}