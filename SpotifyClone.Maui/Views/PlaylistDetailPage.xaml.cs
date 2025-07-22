using SpotifyClone.Maui.ViewModels;

namespace SpotifyClone.Maui.Views;

public partial class PlaylistDetailPage : ContentPage
{
    private readonly PlaylistDetailViewModel _viewModel;
    public PlaylistDetailPage(PlaylistDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _viewModel = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadPlaylistDetails();
    }
}