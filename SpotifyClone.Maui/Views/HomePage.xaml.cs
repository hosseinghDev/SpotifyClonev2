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
        if (_viewModel.Songs.Count == 0)
        {
            _viewModel.LoadSongsCommand.Execute(null);
        }
    }
}