using System;
using UnityEngine;

public class ShopViewModel : ViewModelBase
{
    private readonly IShopRepository _repository;
    private readonly string _playerId;
    private readonly string _lobbyId;

    public Observable<int>    CurrentRound { get; } = new Observable<int>();
    public Observable<int>    Money        { get; } = new Observable<int>();
    public Observable<int>    HandLevel    { get; } = new Observable<int>(1);
    public Observable<int>    UpgradeCost  { get; } = new Observable<int>();
    public Observable<string> BeforeStat   { get; } = new Observable<string>();
    public Observable<string> AfterStat    { get; } = new Observable<string>();
    public Observable<bool>   CanUpgrade   { get; } = new Observable<bool>();
    public Observable<string> ErrorMsg     { get; } = new Observable<string>();

    public ShopViewModel(string playerId, string lobbyId)
    {
        _playerId = playerId;
        _lobbyId  = lobbyId;

        RepositoryFactory.Instance.Register<IShopRepository, ShopRepository>();
        _repository = RepositoryFactory.Instance.Get<IShopRepository>();
    }

    public override async void Initialize()
    {
        base.Initialize();
        try
        {
            await FirebaseInitializer.EnsureInitializedAsync();

            await FirebaseClient.Instance.SubscribeAsync<MatchInfoModel>(
                $"matches/{_lobbyId}",
                onValueChanged: match =>
                {
                    if (match == null) return;
                    CurrentRound.Value = match.currentRound;
                },
                onError: err => Debug.LogError(err)
            );

            await LoadShopInfoAsync();
        }
        catch (Exception e) { Debug.LogException(e); }
    }

    private async System.Threading.Tasks.Task LoadShopInfoAsync()
    {
        var response = await _repository.GetShopInfo(_playerId);

        if (!response.isSuccess)
        {
            ErrorMsg.Value = response.error.message;
            return;
        }

        Refresh(response.data);
    }

    private void Refresh(GetShopInfoResponse data)
    {
        HandLevel.Value   = data.handLevel;
        Money.Value       = data.money;
        UpgradeCost.Value = data.upgradeCost;
        BeforeStat.Value  = data.currentStat;
        AfterStat.Value   = data.nextStat;
        CanUpgrade.Value  = data.money >= data.upgradeCost && data.handLevel < 5;
    }

    private void Refresh(UpgradeResponse data)
    {
        HandLevel.Value   = data.handLevel;
        Money.Value       = data.money;
        UpgradeCost.Value = data.upgradeCost;
        BeforeStat.Value  = data.currentStat;
        AfterStat.Value   = data.nextStat;
        CanUpgrade.Value  = data.money >= data.upgradeCost && data.handLevel < 5;
    }

    public async void OnUpgrade()
    {
        if (!CanUpgrade.Value) return;

        try
        {
            CanUpgrade.Value = false;

            var response = await _repository.PostUpgrade(_playerId);

            if (!response.isSuccess)
            {
                ErrorMsg.Value   = response.error.message;
                CanUpgrade.Value = true;
                return;
            }

            Refresh(response.data);
        }
        catch (Exception e)
        {
            ErrorMsg.Value   = e.Message;
            CanUpgrade.Value = true;
            Debug.LogException(e);
        }
    }
}