using UnityEngine;
using UnityEngine.UIElements;

public class PerkUIController : MonoBehaviour
{
    private PerkViewModel _viewModel;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var perk1Btn   = root.Q<Button>("Perk1");
        var perk2Btn   = root.Q<Button>("Perk2");
        var perk3Btn   = root.Q<Button>("Perk3");

        var perk1Title = root.Q<Label>("Perk1Title");
        var perk1Exp   = root.Q<Label>("Perk1Exp");
        var perk2Title = root.Q<Label>("Perk2Title");
        var perk2Exp   = root.Q<Label>("Perk2Exp");
        var perk3Title = root.Q<Label>("Perk3Title");
        var perk3Exp   = root.Q<Label>("Perk3Exp");

        var idVm = ViewModelLocator.Instance.Get<LoginViewModel>();
        _viewModel = new PerkViewModel(idVm.PlayerId.Value, idVm.LobbyId.Value);
        _viewModel.Initialize();

        _viewModel.Perk1Title.Subscribe(title => perk1Title.text = title ?? "");
        _viewModel.Perk1Desc.Subscribe(desc   => perk1Exp.text   = desc  ?? "");
        _viewModel.Perk2Title.Subscribe(title => perk2Title.text = title ?? "");
        _viewModel.Perk2Desc.Subscribe(desc   => perk2Exp.text   = desc  ?? "");
        _viewModel.Perk3Title.Subscribe(title => perk3Title.text = title ?? "");
        _viewModel.Perk3Desc.Subscribe(desc   => perk3Exp.text   = desc  ?? "");

        _viewModel.CanSelect.Subscribe(can =>
        {
            perk1Btn.SetEnabled(can);
            perk2Btn.SetEnabled(can);
            perk3Btn.SetEnabled(can);
        });

        perk1Btn.clicked += () => _viewModel.OnSelectPerk(1);
        perk2Btn.clicked += () => _viewModel.OnSelectPerk(2);
        perk3Btn.clicked += () => _viewModel.OnSelectPerk(3);
    }

    private void OnDestroy() => _viewModel?.Dispose();
}