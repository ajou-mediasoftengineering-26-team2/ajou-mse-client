using System;
using UnityEngine;

public class PerkViewModel : ViewModelBase
{
    private readonly IPerkRepository _repository;
    private readonly string _playerId;
    private readonly string _lobbyId;

    public Observable<string> Perk1Title { get; } = new Observable<string>();
    public Observable<string> Perk1Desc  { get; } = new Observable<string>();
    public Observable<string> Perk2Title { get; } = new Observable<string>();
    public Observable<string> Perk2Desc  { get; } = new Observable<string>();
    public Observable<string> Perk3Title { get; } = new Observable<string>();
    public Observable<string> Perk3Desc  { get; } = new Observable<string>();
    public Observable<bool>   CanSelect  { get; } = new Observable<bool>(true);
    public Observable<string> ErrorMsg   { get; } = new Observable<string>();

    private int _perk1Id, _perk2Id, _perk3Id;

    public PerkViewModel(string playerId, string lobbyId)
    {
        _playerId = playerId;
        _lobbyId  = lobbyId;

        RepositoryFactory.Instance.Register<IPerkRepository, PerkRepository>();
        _repository = RepositoryFactory.Instance.Get<IPerkRepository>();
    }

    public override async void Initialize()
    {
        base.Initialize();
        try
        {
            await LoadPerkChoicesAsync();
        }
        catch (Exception e) { Debug.LogException(e); }
    }

    private async System.Threading.Tasks.Task LoadPerkChoicesAsync()
    {
        var response = await _repository.GetPerkChoices(_playerId);

        if (!response.isSuccess)
        {
            ErrorMsg.Value = response.error.message;
            return;
        }

        _perk1Id = response.data.perk1.id;
        _perk2Id = response.data.perk2.id;
        _perk3Id = response.data.perk3.id;

        Perk1Title.Value = response.data.perk1.title;
        Perk1Desc.Value  = response.data.perk1.description;
        Perk2Title.Value = response.data.perk2.title;
        Perk2Desc.Value  = response.data.perk2.description;
        Perk3Title.Value = response.data.perk3.title;
        Perk3Desc.Value  = response.data.perk3.description;

        CanSelect.Value = true;
    }

    public async void OnSelectPerk(int slot)
    {
        if (!CanSelect.Value) return;

        int perkId = slot switch { 1 => _perk1Id, 2 => _perk2Id, 3 => _perk3Id, _ => -1 };
        if (perkId < 0) return;

        try
        {
            CanSelect.Value = false;

            var response = await _repository.PostSelectPerk(_playerId, perkId);

            if (!response.isSuccess)
            {
                ErrorMsg.Value  = response.error.message;
                CanSelect.Value = true;
                return;
            }
        }
        catch (Exception e)
        {
            ErrorMsg.Value  = e.Message;
            CanSelect.Value = true;
            Debug.LogException(e);
        }
    }
}