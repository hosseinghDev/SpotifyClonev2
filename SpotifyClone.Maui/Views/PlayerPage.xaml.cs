using SpotifyClone.Maui.ViewModels;

namespace SpotifyClone.Maui.Views;

public partial class PlayerPage : ContentPage
{
    private readonly PlayerViewModel _viewModel;
    public PlayerPage(PlayerViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _viewModel = vm;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.StopAudio();
    }
}