// 202422170 주형준
using System;
using UnityEngine;

public class SelectHandsViewModel : ViewModelBase
{
    private readonly ISelectHandsRepository _repo;

    private string _playerId;
    private string _lobbyId;

    public Observable<string> CurrentHand { get; } = new Observable<string>();
    public Observable<bool>   CanSelect   { get; } = new Observable<bool>(true);
    public Observable<string> ErrorMsg    { get; } = new Observable<string>();

    public SelectHandsViewModel()
    {
        _repo = RepositoryFactory.Instance.Get<ISelectHandsRepository>();
    }

    public void SetPlayerInfo(string playerId, string lobbyId)
    {
        _playerId = playerId;
        _lobbyId  = lobbyId;
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public async void OnSelectHand(int slot)
    {
        if (!CanSelect.Value) return;
        if (slot < 1 || slot > 6) return;

        CanSelect.Value = false;
        try
        {
            var handType = ((HandElementalType)(slot - 1)).ToString(); // "FIRE", "WATER" ...
            var res = await _repo.PostSelectHand(_playerId, handType);
            if (!res.isSuccess)
            {
                ErrorMsg.Value  = res.error.message;
                CanSelect.Value = true;
                return;
            }
            CurrentHand.Value = handType;
            _ = _repo.PutAck(_playerId);
            EventBus.Publish(new PlaySfxEvent(SfxType.ButtonClick));
        }
        catch (Exception e)
        {
            ErrorMsg.Value  = e.Message;
            CanSelect.Value = true;
            Debug.LogException(e);
        }
    }
}