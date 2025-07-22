using SpotifyClone.Maui.ViewModels;

namespace SpotifyClone.Maui.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;
    public HomePage(HomeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _viewModel = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Always execute the command on appearing to get the latest songs.
        _viewModel.LoadSongsCommand.Execute(null);
    }
}