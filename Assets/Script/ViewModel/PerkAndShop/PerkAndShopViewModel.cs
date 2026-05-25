using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

//202422170 주형준
public class PerkAndShopViewModel : ViewModelBase
{
    private readonly IPerkAndShopRepository _perkAndShopRepo;
    private string _playerId;
    private string _lobbyId;

    private string _matchSubId;
    private string _playerSubId;
    private List<string> _perkChoices = new List<string>();

    public Observable<bool>   IsVisible   { get; } = new Observable<bool>();
    public Observable<string> Perk1Title  { get; } = new Observable<string>();
    public Observable<string> Perk1Desc   { get; } = new Observable<string>();
    public Observable<string> Perk1Raw    { get; } = new Observable<string>();
    public Observable<string> Perk2Title  { get; } = new Observable<string>();
    public Observable<string> Perk2Desc   { get; } = new Observable<string>();
    public Observable<string> Perk2Raw    { get; } = new Observable<string>();
    public Observable<string> Perk3Title  { get; } = new Observable<string>();
    public Observable<string> Perk3Desc   { get; } = new Observable<string>();
    public Observable<string> Perk3Raw    { get; } = new Observable<string>();
    public Observable<bool>   CanSelect   { get; } = new Observable<bool>(false);
    //public Observable<bool>   CanUpgrade  { get; } = new Observable<bool>(false);
    //public Observable<string> BeforeStat  { get; } = new Observable<string>();
    //public Observable<string> AfterStat   { get; } = new Observable<string>();
    //public Observable<string> UpgradeCost { get; } = new Observable<string>();
    public Observable<string> ErrorMsg    { get; } = new Observable<string>();

    public PerkAndShopViewModel()
    {
        RepositoryFactory.Instance.Register<IPerkAndShopRepository, PerkAndShopRepository>();
        _perkAndShopRepo = RepositoryFactory.Instance.Get<IPerkAndShopRepository>();
    }

    public void SetPlayerInfo(string playerId, string lobbyId)
    {
        _playerId = playerId;
        _lobbyId  = lobbyId;
    }

    public override async void Initialize()
    {
        base.Initialize();
        //테스트용 -> 나중에 지우기
        /*
        IsVisible.Value = true;
        _perkChoices = new List<string> { "UPGRADE", "IRON_FIST", "VAMPIRISM", "GRIT" };
        RefreshPerkCards();
        CanSelect.Value = true;
        */
        //테스트용 -> 나중에 지우기
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
                bool isPerkPhase = match.state == "GAME_PERK_CHOICE";
                IsVisible.Value = isPerkPhase;
                CanSelect.Value = isPerkPhase && _perkChoices.Count > 0;
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
                if (player?.perkChoiceList == null || player.perkChoiceList.Count == 0) return;
                _perkChoices = player.perkChoiceList;
                RefreshPerkCards();

                // 서버팀 확인 후 추가
                // BeforeStat.Value  = player.currentStat;
                // AfterStat.Value   = player.nextStat;
                // UpgradeCost.Value = player.upgradeCost.ToString();
                // CanUpgrade.Value  = player.money >= player.upgradeCost;
            },
            onError: err => Debug.LogError(err)
        );
    }

    private void RefreshPerkCards()
    {
        SetPerkCard(_perkChoices, 1, Perk1Title, Perk1Desc, Perk1Raw);
        SetPerkCard(_perkChoices, 2, Perk2Title, Perk2Desc, Perk2Raw);
        SetPerkCard(_perkChoices, 3, Perk3Title, Perk3Desc, Perk3Raw);
    }

    private void SetPerkCard(List<string> choices, int index,
        Observable<string> title, Observable<string> desc, Observable<string> raw)
    {
        if (choices.Count <= index) return;
        if (!Enum.TryParse<PerkType>(choices[index], out var perkType)) return;

        title.Value = PerkInfoProvider.GetDisplayName(perkType);
        desc.Value  = PerkInfoProvider.GetDescription(perkType);
        raw.Value   = choices[index];
    }

    public async void OnSelectPerk(int slot)
    {
        if (!CanSelect.Value) return;
        if (_perkChoices.Count <= slot) return;

        string selectedPerk = _perkChoices[slot];
        CanSelect.Value = false;

        try
        {
            var res = await _perkAndShopRepo.PutChoice(_playerId, selectedPerk);
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
/*
    public async void OnUpgrade()
    {
        if (!CanUpgrade.Value) return;
        if (_perkChoices.Count == 0) return;

        CanUpgrade.Value = false;
        try
        {
            var res = await _perkAndShopRepo.PutChoice(_playerId, _perkChoices[0]);
            if (!res.isSuccess)
            {
                ErrorMsg.Value   = res.error.message;
                CanUpgrade.Value = true;
                return;
            }
            EventBus.Publish(new PlaySfxEvent(SfxType.ButtonClick));
        }
        catch (Exception e)
        {
            ErrorMsg.Value   = e.Message;
            CanUpgrade.Value = true;
            Debug.LogException(e);
        }
    }
*/

    public override void Dispose()
    {
        if (!string.IsNullOrEmpty(_matchSubId))
            FirebaseClient.Instance.Unsubscribe(_matchSubId);
        if (!string.IsNullOrEmpty(_playerSubId))
            FirebaseClient.Instance.Unsubscribe(_playerSubId);
        base.Dispose();
    }
}