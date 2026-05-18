using System;
using System.Threading.Tasks;
using UnityEngine;

// 202422170 주형준
public class PerkAndShopViewModel : ViewModelBase
{
    private readonly IPerkAndShopRepository _repo;

    private string _playerId;
    private string _lobbyId;

    // Shop
    public Observable<int>    CurrentRound { get; } = new Observable<int>();
    public Observable<int>    Money        { get; } = new Observable<int>();
    public Observable<int>    HandLevel    { get; } = new Observable<int>(1);
    public Observable<int>    UpgradeCost  { get; } = new Observable<int>();
    public Observable<string> BeforeStat   { get; } = new Observable<string>();
    public Observable<string> AfterStat    { get; } = new Observable<string>();
    public Observable<bool>   CanUpgrade   { get; } = new Observable<bool>();

    // Perk
    public Observable<string> Perk1Title { get; } = new Observable<string>();
    public Observable<string> Perk1Desc  { get; } = new Observable<string>();
    public Observable<string> Perk2Title { get; } = new Observable<string>();
    public Observable<string> Perk2Desc  { get; } = new Observable<string>();
    public Observable<string> Perk3Title { get; } = new Observable<string>();
    public Observable<string> Perk3Desc  { get; } = new Observable<string>();
    public Observable<bool>   CanSelect  { get; } = new Observable<bool>(true);

    public Observable<string> ErrorMsg { get; } = new Observable<string>();

    private int _perk1Id, _perk2Id, _perk3Id;

    /// <summary>
    /// Default constructor. Repository is retrieved from the factory.
    /// </summary>
    public PerkAndShopViewModel()
    {
        _repo = RepositoryFactory.Instance.Get<IPerkAndShopRepository>();
    }

    /// <summary>
    /// Called from the View's OnEnable to inject player and lobby info before Initialize.
    /// </summary>
    public void SetPlayerInfo(string playerId, string lobbyId)
    {
        _playerId = playerId;
        _lobbyId  = lobbyId;
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

            await LoadInfoAsync();
        }
        catch (Exception e) { Debug.LogException(e); }
    }

    private async Task LoadInfoAsync()
    {
        var res = await _repo.GetInfo(_playerId);
        if (!res.isSuccess) { ErrorMsg.Value = res.error.message; return; }
        Refresh(res.data);
    }

    private void Refresh(GetPerkAndShopInfoResponse data)
    {
        // Shop
        Money.Value       = data.money;
        HandLevel.Value   = data.handLevel;
        UpgradeCost.Value = data.upgradeCost;
        BeforeStat.Value  = data.currentStat;
        AfterStat.Value   = data.nextStat;
        CanUpgrade.Value  = data.money >= data.upgradeCost;

        // Perk
        _perk1Id = data.perk1.id;
        _perk2Id = data.perk2.id;
        _perk3Id = data.perk3.id;

        Perk1Title.Value = data.perk1.title;
        Perk1Desc.Value  = data.perk1.description;
        Perk2Title.Value = data.perk2.title;
        Perk2Desc.Value  = data.perk2.description;
        Perk3Title.Value = data.perk3.title;
        Perk3Desc.Value  = data.perk3.description;
        CanSelect.Value  = true;
    }

    private void Refresh(PerkAndShopUpgradeResponse data)
    {
        Money.Value       = data.money;
        HandLevel.Value   = data.handLevel;
        UpgradeCost.Value = data.upgradeCost;
        BeforeStat.Value  = data.currentStat;
        AfterStat.Value   = data.nextStat;
        CanUpgrade.Value  = data.money >= data.upgradeCost;
    }

    /// <summary>
    /// Sends upgrade request to server and refreshes on success.
    /// </summary>
    public async void OnUpgrade()
    {
        if (!CanUpgrade.Value) return;
        CanUpgrade.Value = false;
        try
        {
            var res = await _repo.PostUpgrade(_playerId);
            if (!res.isSuccess)
            {
                ErrorMsg.Value   = res.error.message;
                CanUpgrade.Value = true;
                return;
            }
            Refresh(res.data);
            EventBus.Publish(new PlaySfxEvent(SfxType.ButtonClick));
        }
        catch (Exception e)
        {
            ErrorMsg.Value   = e.Message;
            CanUpgrade.Value = true;
            Debug.LogException(e);
        }
    }

    /// <summary>
    /// Sends selected perk to server based on slot number (1, 2, or 3).
    /// </summary>
    public async void OnSelectPerk(int slot)
    {
        if (!CanSelect.Value) return;
        int perkId = slot switch { 1 => _perk1Id, 2 => _perk2Id, 3 => _perk3Id, _ => -1 };
        if (perkId < 0) return;
        CanSelect.Value = false;
        try
        {
            var res = await _repo.PostSelectPerk(_playerId, perkId);
            if (!res.isSuccess)
            {
                ErrorMsg.Value  = res.error.message;
                CanSelect.Value = true;
                return;
            }
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