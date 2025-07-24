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

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Asynchronously call the new PlayAudio method
        await _viewModel.PlayAudio();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Stop the audio when leaving the page
        _viewModel.StopAudio();
    }
}