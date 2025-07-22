using SpotifyClone.Maui.ViewModels;

namespace SpotifyClone.Maui.Views;

public partial class PlaylistPage : ContentPage
{
    private readonly PlaylistViewModel _viewModel;
    public PlaylistPage(PlaylistViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _viewModel = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadPlaylistsCommand.Execute(null);
    }
}